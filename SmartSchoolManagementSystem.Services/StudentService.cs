using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class StudentService : BaseService<Student, StudentDto, CreateStudentDto, UpdateStudentDto>, IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassRepository _classRepository;

    public StudentService(
        IStudentRepository studentRepository,
        IClassRepository classRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(studentRepository, unitOfWork, mapper)
    {
        _studentRepository = studentRepository;
        _classRepository = classRepository;
    }

    public async Task<StudentDto> GetStudentByStudentIdAsync(string studentId)
    {
        var student = await _studentRepository.GetStudentByStudentIdAsync(studentId);
        if (student == null)
            throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<IReadOnlyList<StudentDto>> GetStudentsByClassAsync(Guid classId)
    {
        var students = await _studentRepository.GetStudentsByClassAsync(classId);
        return _mapper.Map<IReadOnlyList<StudentDto>>(students);
    }

    public async Task<IReadOnlyList<StudentDto>> SearchStudentsAsync(string searchTerm)
    {
        var students = await _studentRepository.SearchStudentsAsync(searchTerm);
        return _mapper.Map<IReadOnlyList<StudentDto>>(students);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
    {
        return await _studentRepository.IsEmailUniqueAsync(email, excludeId);
    }

    public async Task<bool> IsStudentIdUniqueAsync(string studentId, Guid? excludeId = null)
    {
        return await _studentRepository.IsStudentIdUniqueAsync(studentId, excludeId);
    }

    public async Task<IReadOnlyList<StudentDto>> GetStudentsWithOverdueBooksAsync()
    {
        var students = await _studentRepository.GetStudentsWithOverdueBooksAsync();
        return _mapper.Map<IReadOnlyList<StudentDto>>(students);
    }

    public async Task<IReadOnlyList<BookLendingDto>> GetStudentLendingHistoryAsync(Guid studentId)
    {
        var lendings = await _studentRepository.GetStudentLendingHistoryAsync(studentId);
        return _mapper.Map<IReadOnlyList<BookLendingDto>>(lendings);
    }

    public async Task<IReadOnlyList<AttendanceDto>> GetStudentAttendanceHistoryAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        var attendance = await _studentRepository.GetStudentAttendanceHistoryAsync(studentId, startDate, endDate);
        return _mapper.Map<IReadOnlyList<AttendanceDto>>(attendance);
    }

    public async Task<StudentSummaryDto> GetStudentSummaryAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        var classes = await _classRepository.GetAllAsync();

        var summary = new StudentSummaryDto
        {
            TotalStudents = students.Count,
            StudentsByGender = students
                .GroupBy(s => s.Gender)
                .ToDictionary(g => g.Key, g => g.Count()),
            StudentsByClass = classes
                .ToDictionary(
                    c => c.Name,
                    c => students.Count(s => s.ClassId == c.Id)),
            AverageAttendanceRate = await CalculateOverallAttendanceRateAsync(students),
            StudentsWithOverdueBooks = (await GetStudentsWithOverdueBooksAsync()).Count
        };

        return summary;
    }

    public async Task<double> GetStudentAttendancePercentageAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        var attendance = await _studentRepository.GetStudentAttendanceHistoryAsync(studentId, startDate, endDate);
        if (!attendance.Any()) return 0;

        var totalDays = attendance.Count;
        var presentDays = attendance.Count(a => a.IsPresent);

        return (double)presentDays / totalDays * 100;
    }

    private async Task<double> CalculateOverallAttendanceRateAsync(IReadOnlyList<Student> students)
    {
        if (!students.Any()) return 0;

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        double totalRate = 0;
        foreach (var student in students)
        {
            totalRate += await GetStudentAttendancePercentageAsync(student.Id, startDate, endDate);
        }

        return totalRate / students.Count;
    }

    public async Task<string> GenerateStudentIdAsync()
    {
        var year = DateTime.UtcNow.Year.ToString().Substring(2);
        var lastStudent = await _studentRepository.GetAllAsync();
        var count = lastStudent.Count + 1;

        return $"STU{year}{count:D4}";
    }

    public async Task<bool> TransferStudentToClassAsync(Guid studentId, Guid newClassId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

            var newClass = await _classRepository.GetByIdAsync(newClassId);
            if (newClass == null)
                throw new KeyNotFoundException($"Class with ID {newClassId} was not found.");

            student.ClassId = newClassId;
            await _studentRepository.UpdateAsync(student);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public override async Task<StudentDto> CreateAsync(CreateStudentDto createDto)
    {
        if (!await IsEmailUniqueAsync(createDto.Email))
            throw new InvalidOperationException($"Email {createDto.Email} is already in use.");

        var student = _mapper.Map<Student>(createDto);
        student.StudentId = await GenerateStudentIdAsync();

        await _studentRepository.AddAsync(student);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<StudentDto>(student);
    }

    public override async Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto updateDto)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with ID {id} was not found.");

        if (!await IsEmailUniqueAsync(updateDto.Email, id))
            throw new InvalidOperationException($"Email {updateDto.Email} is already in use.");

        _mapper.Map(updateDto, student);
        await _studentRepository.UpdateAsync(student);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<StudentDto>(student);
    }
}
