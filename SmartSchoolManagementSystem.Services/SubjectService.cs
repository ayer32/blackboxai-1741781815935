using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class SubjectService : BaseService<Subject, SubjectDto, CreateSubjectDto, UpdateSubjectDto>, ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly ITeacherRepository _teacherRepository;

    public SubjectService(
        ISubjectRepository subjectRepository,
        ITeacherRepository teacherRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(subjectRepository, unitOfWork, mapper)
    {
        _subjectRepository = subjectRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<SubjectDto> GetSubjectByCodeAsync(string code)
    {
        var subject = await _subjectRepository.GetSubjectByCodeAsync(code);
        if (subject == null)
            throw new KeyNotFoundException($"Subject with code {code} was not found.");

        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<IReadOnlyList<SubjectDto>> GetSubjectsByTeacherAsync(Guid teacherId)
    {
        var subjects = await _subjectRepository.GetSubjectsByTeacherAsync(teacherId);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(subjects);
    }

    public async Task<IReadOnlyList<SubjectDto>> GetSubjectsByClassAsync(Guid classId)
    {
        var subjects = await _subjectRepository.GetSubjectsByClassAsync(classId);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(subjects);
    }

    public async Task<bool> IsSubjectCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        return await _subjectRepository.IsSubjectCodeUniqueAsync(code, excludeId);
    }

    public async Task<IReadOnlyList<SubjectDto>> SearchSubjectsAsync(string searchTerm)
    {
        var subjects = await _subjectRepository.SearchSubjectsAsync(searchTerm);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(subjects);
    }

    public async Task<IReadOnlyList<ClassDto>> GetSubjectClassesAsync(Guid subjectId)
    {
        var classes = await _subjectRepository.GetSubjectClassesAsync(subjectId);
        return _mapper.Map<IReadOnlyList<ClassDto>>(classes);
    }

    public async Task<int> GetSubjectStudentCountAsync(Guid subjectId)
    {
        return await _subjectRepository.GetSubjectStudentCountAsync(subjectId);
    }

    public async Task<SubjectSummaryDto> GetSubjectSummaryAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();

        var summary = new SubjectSummaryDto
        {
            TotalSubjects = subjects.Count,
            SubjectsByCredits = subjects
                .GroupBy(s => s.Credits)
                .ToDictionary(g => g.Key, g => g.Count()),
            SubjectsByTeacher = await GetSubjectsByTeacherCountAsync(),
            AverageStudentsPerSubject = await CalculateAverageStudentsPerSubjectAsync(subjects),
            TopEnrolledSubjects = await GetTopEnrolledSubjectsAsync()
        };

        return summary;
    }

    private async Task<Dictionary<string, int>> GetSubjectsByTeacherCountAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();
        return teachers.ToDictionary(
            t => $"{t.FirstName} {t.LastName}",
            t => t.Subjects.Count);
    }

    private async Task<double> CalculateAverageStudentsPerSubjectAsync(IReadOnlyList<Subject> subjects)
    {
        if (!subjects.Any()) return 0;

        var totalStudents = 0;
        foreach (var subject in subjects)
        {
            totalStudents += await GetSubjectStudentCountAsync(subject.Id);
        }

        return (double)totalStudents / subjects.Count;
    }

    public async Task<bool> AssignTeacherToSubjectAsync(Guid subjectId, Guid teacherId)
    {
        if (!await IsTeacherQualifiedForSubjectAsync(teacherId, subjectId))
            throw new InvalidOperationException("Teacher is not qualified for this subject.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} was not found.");

            subject.TeacherId = teacherId;
            await _subjectRepository.UpdateAsync(subject);
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

    public async Task<bool> RemoveTeacherFromSubjectAsync(Guid subjectId)
    {
        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        if (subject == null)
            return false;

        subject.TeacherId = null;
        await _subjectRepository.UpdateAsync(subject);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<IReadOnlyList<SubjectEnrollmentDto>> GetTopEnrolledSubjectsAsync(int count = 10)
    {
        var subjects = await _subjectRepository.GetAllAsync();
        var enrollments = new List<SubjectEnrollmentDto>();

        foreach (var subject in subjects)
        {
            var studentCount = await GetSubjectStudentCountAsync(subject.Id);
            enrollments.Add(new SubjectEnrollmentDto
            {
                SubjectId = subject.Id,
                SubjectName = subject.Name,
                SubjectCode = subject.Code,
                EnrolledStudents = studentCount,
                AssignedClasses = subject.Classes.Count
            });
        }

        return enrollments
            .OrderByDescending(e => e.EnrolledStudents)
            .Take(count)
            .ToList();
    }

    public async Task<bool> IsTeacherQualifiedForSubjectAsync(Guid teacherId, Guid subjectId)
    {
        return await _subjectRepository.IsTeacherQualifiedForSubjectAsync(teacherId, subjectId);
    }

    public override async Task<SubjectDto> CreateAsync(CreateSubjectDto createDto)
    {
        if (!await IsSubjectCodeUniqueAsync(createDto.Code))
            throw new InvalidOperationException($"Subject with code {createDto.Code} already exists.");

        if (createDto.TeacherId.HasValue && 
            !await IsTeacherQualifiedForSubjectAsync(createDto.TeacherId.Value, Guid.Empty))
            throw new InvalidOperationException("Assigned teacher is not qualified for this subject.");

        var subject = _mapper.Map<Subject>(createDto);
        await _subjectRepository.AddAsync(subject);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<SubjectDto>(subject);
    }

    public override async Task<SubjectDto> UpdateAsync(Guid id, UpdateSubjectDto updateDto)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject == null)
            throw new KeyNotFoundException($"Subject with ID {id} was not found.");

        if (updateDto.TeacherId.HasValue && 
            !await IsTeacherQualifiedForSubjectAsync(updateDto.TeacherId.Value, id))
            throw new InvalidOperationException("Assigned teacher is not qualified for this subject.");

        _mapper.Map(updateDto, subject);
        await _subjectRepository.UpdateAsync(subject);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<SubjectDto>(subject);
    }
}
