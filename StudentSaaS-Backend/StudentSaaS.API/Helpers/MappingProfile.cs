using AutoMapper;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Batch, BatchDto>()
            .ForMember(d => d.TeacherName, opt => opt.MapFrom(s => s.Teacher != null ? s.Teacher.Name : ""))
            .ForMember(d => d.CourseName, opt => opt.MapFrom(s => s.Course != null ? s.Course.CourseName : ""));
        CreateMap<BatchDto, Batch>();

        CreateMap<FeePlan, FeePlanDto>()
            .ForMember(d => d.CourseName, opt => opt.MapFrom(s => s.Course != null ? s.Course.CourseName : ""));
        CreateMap<FeePlanDto, FeePlan>();

        CreateMap<Enquiry, EnquiryDto>()
            .ForMember(d => d.CourseName, opt => opt.MapFrom(s => s.Course != null ? s.Course.CourseName : ""));
        CreateMap<EnquiryDto, Enquiry>();

        CreateMap<Contact, ContactDto>();
        CreateMap<ContactDto, Contact>();

        CreateMap<Notification, NotificationDto>();
        CreateMap<NotificationDto, Notification>();

        CreateMap<Testimonial, TestimonialDto>();
        CreateMap<TestimonialDto, Testimonial>();
    }
}
