using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs;
using SmartSchoolManagementSystem.Core.DTOs.Donation;
using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities;
using SmartSchoolManagementSystem.Core.Entities.Donation;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Base mappings
        CreateMap<BaseEntity, BaseDto>();

        // Donation mappings
        CreateMap<Donation, DonationDto>();
        CreateMap<CreateDonationDto, Donation>();
        CreateMap<UpdateDonationDto, Donation>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Book mappings
        CreateMap<Book, BookDto>();
        CreateMap<CreateBookDto, Book>();
        CreateMap<UpdateBookDto, Book>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // BookLending mappings
        CreateMap<BookLending, BookLendingDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN));
        CreateMap<CreateBookLendingDto, BookLending>();
        CreateMap<UpdateBookLendingDto, BookLending>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Student mappings
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.Name));
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Teacher mappings
        CreateMap<Teacher, TeacherDto>();
        CreateMap<CreateTeacherDto, Teacher>();
        CreateMap<UpdateTeacherDto, Teacher>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Class mappings
        CreateMap<Class, ClassDto>()
            .ForMember(dest => dest.ClassTeacherName, 
                opt => opt.MapFrom(src => $"{src.ClassTeacher.FirstName} {src.ClassTeacher.LastName}"))
            .ForMember(dest => dest.StudentCount, 
                opt => opt.MapFrom(src => src.Students.Count));
        CreateMap<CreateClassDto, Class>();
        CreateMap<UpdateClassDto, Class>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Subject mappings
        CreateMap<Subject, SubjectDto>()
            .ForMember(dest => dest.TeacherName, 
                opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
            .ForMember(dest => dest.AssignedClasses, 
                opt => opt.MapFrom(src => src.Classes.Select(c => c.Name)));
        CreateMap<CreateSubjectDto, Subject>();
        CreateMap<UpdateSubjectDto, Subject>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Attendance mappings
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(dest => dest.StudentName, 
                opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
            .ForMember(dest => dest.StudentClass, 
                opt => opt.MapFrom(src => src.Student.Class.Name));
        CreateMap<CreateAttendanceDto, Attendance>();
        CreateMap<UpdateAttendanceDto, Attendance>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
