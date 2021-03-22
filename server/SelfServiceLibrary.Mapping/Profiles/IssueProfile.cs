using AutoMapper;

using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.Service.DTO.Issue;

namespace SelfServiceLibrary.Mapping.Profiles
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<IssueCreateDTO, Issue>(MemberList.Source);
            CreateMap<Issue, IssueDetailDTO>();
            CreateMap<Issue, IssueListlDTO>();

            CreateMap<Book, Issue>()
                .ForMember(x => x.BookName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.DepartmentNumber, x => x.MapFrom(y => y.DepartmentNumber))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
