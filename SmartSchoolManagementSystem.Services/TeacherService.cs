using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class TeacherService : BaseService<Teacher, TeacherDto, CreateTeacherDto, UpdateTeacherDto>, ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ISubjectRepository _subjectRepository;

    public TeacherService(
        ITeacherRepository teacherRepository,
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(teacherRepository, unitOfWork, mapper)
    {
        _teacherRepository = teacherRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<TeacherDto> GetTeacherByTeacherIdAsync(string teacherId)
    {
        var teacher = await _teacherRepository.GetTeacherByTeacherIdAsync(teacherId);
        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {teacherId} was not found.");

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<IReadOnlyList<TeacherDto>> SearchTeachersAsync(string searchTerm)
    {
        var teachers = await _teacherRepository.SearchTeachersAsync(searchTerm);
        return _mapper.Map<IReadOnlyList<TeacherDto>>(teachers);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
    {
        return await _teacherRepository.IsEmailUniqueAsync(email, excludeId);
    }

    public async Task<bool> IsTeacherIdUniqueAsync(string teacherId, Guid? excludeId = null)
    {
        return await _teacherRepository.IsTeacherIdUniqueAsync(teacherId, excludeId);
    }

    public async Task<IReadOnlyList<ClassDto>> GetTeacherClassesAsync(Guid teacherId)
    {
        var classes = await _teacherRepository.GetTeacherClassesAsync(teacherId);
        return _mapper.Map<IReadOnlyList<ClassDto>>(classes);
    }

    public async Task<IReadOnlyList<SubjectDto>> GetTeacherSubjectsAsync(Guid teacherId)
    {
        var subjects = await _teacherRepository.GetTeacherSubjectsAsync(teacherId);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(subjects);
    }

    public async Task<TeacherSummaryDto> GetTeacherSummaryAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();

        var summary = new TeacherSummaryDto
        {
            TotalTeachers = teachers.Count,
            TeachersBySpecialization = teachers
                .GroupBy(t => t.Specialization)
                .ToDictionary(g => g.Key, g => g.Count()),
            TeachersBySubject = await GetTeachersBySubjectCountAsync(),
            AverageClassesPerTeacher = await CalculateAverageClassesPerTeacherAsync(),
            AverageSubjectsPerTeacher = await CalculateAverageSubjectsPerTeacherAsync()
        };

        return summary;
    }

    private async Task<Dictionary<string, int>> GetTeachersBySubjectCountAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();
        return subjects
            .GroupBy(s => s.Name)
            .ToDictionary(g => g.Key, g => g.Select(s => s.TeacherId).Distinct().Count());
    }

    private async Task<int> CalculateAverageClassesPerTeacherAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();
        if (!teachers.Any()) return 0;

        var totalClasses = teachers.Sum(t => t.Classes.Count);
        return totalClasses / teachers.Count;
    }

    private async Task<int> CalculateAverageSubjectsPerTeacherAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();
        if (!teachers.Any()) return 0;

        var totalSubjects = teachers.Sum(t => t.Subjects.Count);
        return totalSubjects / teachers.Count;
    }

    public async Task<string> GenerateTeacherIdAsync()
    {
        var year = DateTime.UtcNow.Year.ToString().Substring(2);
        var lastTeacher = await _teacherRepository.GetAllAsync();
        var count = lastTeacher.Count + 1;

        return $"TCH{year}{count:D4}";
    }

    public async Task<bool> AssignClassAsync(Guid teacherId, Guid classId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {teacherId} was not found.");

            if (teacher.Classes.Any(c => c.Id == classId))
                return false; // Class already assigned

            var workload = await GetTeacherWorkloadAsync(teacherId);
            if (workload >= 5) // Maximum 5 classes per teacher
                throw new InvalidOperationException("Teacher has reached maximum class assignment limit.");

            teacher.Classes.Add(new Class { Id = classId });
            await _teacherRepository.UpdateAsync(teacher);
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

    public async Task<bool> AssignSubjectAsync(Guid teacherId, Guid subjectId)
    {
        if (!await IsTeacherQualifiedForSubjectAsync(teacherId, subjectId))
            throw new InvalidOperationException("Teacher is not qualified for this subject.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {teacherId} was not found.");

            if (teacher.Subjects.Any(s => s.Id == subjectId))
                return false; // Subject already assigned

            teacher.Subjects.Add(new Subject { Id = subjectId });
            await _teacherRepository.UpdateAsync(teacher);
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

    public async Task<bool> RemoveClassAssignmentAsync(Guid teacherId, Guid classId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
            return false;

        var classToRemove = teacher.Classes.FirstOrDefault(c => c.Id == classId);
        if (classToRemove == null)
            return false;

        teacher.Classes.Remove(classToRemove);
        await _teacherRepository.UpdateAsync(teacher);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> RemoveSubjectAssignmentAsync(Guid teacherId, Guid subjectId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
            return false;

        var subjectToRemove = teacher.Subjects.FirstOrDefault(s => s.Id == subjectId);
        if (subjectToRemove == null)
            return false;

        teacher.Subjects.Remove(subjectToRemove);
        await _teacherRepository.UpdateAsync(teacher);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> IsTeacherQualifiedForSubjectAsync(Guid teacherId, Guid subjectId)
    {
        return await _subjectRepository.IsTeacherQualifiedForSubjectAsync(teacherId, subjectId);
    }

    public async Task<int> GetTeacherWorkloadAsync(Guid teacherId)
    {
        var classCount = await _teacherRepository.GetTeacherClassCountAsync(teacherId);
        var subjectCount = await _teacherRepository.GetTeacherSubjectCountAsync(teacherId);
        return Math.Max(classCount, subjectCount);
    }

    public override async Task<TeacherDto> CreateAsync(CreateTeacherDto createDto)
    {
        if (!await IsEmailUniqueAsync(createDto.Email))
            throw new InvalidOperationException($"Email {createDto.Email} is already in use.");

        var teacher = _mapper.Map<Teacher>(createDto);
        teacher.TeacherId = await GenerateTeacherIdAsync();

        await _teacherRepository.AddAsync(teacher);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<TeacherDto>(teacher);
    }

    public override async Task<TeacherDto> UpdateAsync(Guid id, UpdateTeacherDto updateDto)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {id} was not found.");

        if (!await IsEmailUniqueAsync(updateDto.Email, id))
            throw new InvalidOperationException($"Email {updateDto.Email} is already in use.");

        _mapper.Map(updateDto, teacher);
        await _teacherRepository.UpdateAsync(teacher);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<TeacherDto>(teacher);
    }
}
