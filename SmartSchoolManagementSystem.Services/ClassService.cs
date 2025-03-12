using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class ClassService : BaseService<Class, ClassDto, CreateClassDto, UpdateClassDto>, IClassService
{
    private readonly IClassRepository _classRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IStudentRepository _studentRepository;

    public ClassService(
        IClassRepository classRepository,
        ITeacherRepository teacherRepository,
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(classRepository, unitOfWork, mapper)
    {
        _classRepository = classRepository;
        _teacherRepository = teacherRepository;
        _studentRepository = studentRepository;
    }

    public async Task<IReadOnlyList<ClassDto>> GetClassesByAcademicYearAsync(int academicYear)
    {
        var classes = await _classRepository.GetClassesByAcademicYearAsync(academicYear);
        return _mapper.Map<IReadOnlyList<ClassDto>>(classes);
    }

    public async Task<IReadOnlyList<StudentDto>> GetClassStudentsAsync(Guid classId)
    {
        var students = await _classRepository.GetClassStudentsAsync(classId);
        return _mapper.Map<IReadOnlyList<StudentDto>>(students);
    }

    public async Task<IReadOnlyList<SubjectDto>> GetClassSubjectsAsync(Guid classId)
    {
        var subjects = await _classRepository.GetClassSubjectsAsync(classId);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(subjects);
    }

    public async Task<int> GetStudentCountAsync(Guid classId)
    {
        return await _classRepository.GetStudentCountAsync(classId);
    }

    public async Task<TeacherDto?> GetClassTeacherAsync(Guid classId)
    {
        var teacher = await _classRepository.GetClassTeacherAsync(classId);
        return teacher != null ? _mapper.Map<TeacherDto>(teacher) : null;
    }

    public async Task<bool> IsClassNameUniqueAsync(string name, string section, int academicYear, Guid? excludeId = null)
    {
        return await _classRepository.IsClassNameUniqueAsync(name, section, academicYear, excludeId);
    }

    public async Task<IReadOnlyList<ClassDto>> SearchClassesAsync(string searchTerm)
    {
        var classes = await _classRepository.SearchClassesAsync(searchTerm);
        return _mapper.Map<IReadOnlyList<ClassDto>>(classes);
    }

    public async Task<Dictionary<Guid, int>> GetAttendanceStatisticsAsync(Guid classId, DateTime date)
    {
        return await _classRepository.GetAttendanceStatisticsAsync(classId, date);
    }

    public async Task<ClassSummaryDto> GetClassSummaryAsync(int? academicYear = null)
    {
        var classes = academicYear.HasValue
            ? await _classRepository.GetClassesByAcademicYearAsync(academicYear.Value)
            : await _classRepository.GetAllAsync();

        var summary = new ClassSummaryDto
        {
            TotalClasses = classes.Count,
            ClassesByAcademicYear = classes
                .GroupBy(c => c.AcademicYear)
                .ToDictionary(g => g.Key, g => g.Count()),
            StudentsByClass = await GetStudentsByClassCountAsync(classes),
            AverageStudentsPerClass = await CalculateAverageStudentsPerClassAsync(classes),
            AttendanceByClass = await GetAttendanceByClassAsync(classes)
        };

        return summary;
    }

    private async Task<Dictionary<string, int>> GetStudentsByClassCountAsync(IReadOnlyList<Class> classes)
    {
        return classes.ToDictionary(
            c => $"{c.Name} - {c.Section}",
            c => c.Students.Count);
    }

    private async Task<double> CalculateAverageStudentsPerClassAsync(IReadOnlyList<Class> classes)
    {
        if (!classes.Any()) return 0;

        var totalStudents = classes.Sum(c => c.Students.Count);
        return (double)totalStudents / classes.Count;
    }

    private async Task<Dictionary<string, double>> GetAttendanceByClassAsync(IReadOnlyList<Class> classes)
    {
        var result = new Dictionary<string, double>();
        var today = DateTime.UtcNow.Date;

        foreach (var @class in classes)
        {
            var stats = await GetAttendanceStatisticsAsync(@class.Id, today);
            if (stats.Any())
            {
                var presentCount = stats.Values.Sum();
                var totalStudents = stats.Count;
                result.Add($"{@class.Name} - {@class.Section}", 
                    totalStudents > 0 ? (double)presentCount / totalStudents * 100 : 0);
            }
        }

        return result;
    }

    public async Task<bool> AssignSubjectToClassAsync(Guid classId, Guid subjectId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var @class = await _classRepository.GetByIdAsync(classId);
            if (@class == null)
                throw new KeyNotFoundException($"Class with ID {classId} was not found.");

            if (@class.Subjects.Any(s => s.Id == subjectId))
                return false; // Subject already assigned

            @class.Subjects.Add(new Subject { Id = subjectId });
            await _classRepository.UpdateAsync(@class);
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

    public async Task<bool> RemoveSubjectFromClassAsync(Guid classId, Guid subjectId)
    {
        var @class = await _classRepository.GetByIdAsync(classId);
        if (@class == null)
            return false;

        var subjectToRemove = @class.Subjects.FirstOrDefault(s => s.Id == subjectId);
        if (subjectToRemove == null)
            return false;

        @class.Subjects.Remove(subjectToRemove);
        await _classRepository.UpdateAsync(@class);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> PromoteStudentsAsync(Guid sourceClassId, Guid targetClassId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var students = await _classRepository.GetClassStudentsAsync(sourceClassId);
            foreach (var student in students)
            {
                student.ClassId = targetClassId;
                await _studentRepository.UpdateAsync(student);
            }

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

    public async Task<ClassAttendanceDto> GetClassAttendanceAsync(Guid classId, DateTime date)
    {
        var @class = await _classRepository.GetByIdAsync(classId);
        if (@class == null)
            throw new KeyNotFoundException($"Class with ID {classId} was not found.");

        var stats = await GetAttendanceStatisticsAsync(classId, date);
        var totalStudents = stats.Count;
        var presentStudents = stats.Values.Sum();

        return new ClassAttendanceDto
        {
            ClassId = classId,
            ClassName = $"{@class.Name} - {@class.Section}",
            Date = date,
            TotalStudents = totalStudents,
            PresentStudents = presentStudents,
            AbsentStudents = totalStudents - presentStudents,
            AttendancePercentage = totalStudents > 0 
                ? (double)presentStudents / totalStudents * 100 
                : 0
        };
    }

    public override async Task<ClassDto> CreateAsync(CreateClassDto createDto)
    {
        if (!await IsClassNameUniqueAsync(createDto.Name, createDto.Section, createDto.AcademicYear))
            throw new InvalidOperationException($"A class with name {createDto.Name} - {createDto.Section} already exists for academic year {createDto.AcademicYear}.");

        var teacher = await _teacherRepository.GetByIdAsync(createDto.TeacherId);
        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {createDto.TeacherId} was not found.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var @class = _mapper.Map<Class>(createDto);
            await _classRepository.AddAsync(@class);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<ClassDto>(@class);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
