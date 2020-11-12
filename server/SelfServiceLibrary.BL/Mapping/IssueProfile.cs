using AutoMapper;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Entities;

namespace SelfServiceLibrary.BL.Mapping
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<IssueCreateDTO, Issue>();
            CreateMap<Issue, IssueDetailDTO>();
            CreateMap<Issue, IssueListlDTO>();
        }
    }
}
