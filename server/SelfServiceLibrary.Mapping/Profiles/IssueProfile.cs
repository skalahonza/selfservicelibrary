using AutoMapper;

using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Service.DTO.Issue;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<IssueCreateDTO, Issue>()
                .ForMember(x => x.IssueDate, x => x.MapFrom(y => y.IssueDate))
                .ForMember(x => x.ExpiryDate, x => x.MapFrom(y => y.ExpiryDate))
                .ForAllOtherMembers(X => X.Ignore());
            CreateMap<Issue, IssueDetailDTO>();
            CreateMap<Issue, IssueListlDTO>();
            CreateMap<Book, Issue>()
                .ForMember(x => x.BookDepartmentNumber, x => x.MapFrom(y => y.DepartmentNumber))
                .ForMember(x => x.BookName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.ISBN, x => x.MapFrom(y => y.ISBN))
                .ForAllOtherMembers(X => X.Ignore());
        }
    }
}
