﻿using AutoMapper;

using SelfServiceLibrary.BL.DTO.Issue;
using SelfServiceLibrary.BL.Entities;

namespace SelfServiceLibrary.BL.Mapping
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
                .ForMember(x => x.BookId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.BookName, x => x.MapFrom(y => y.Name))
                .ForMember(x => x.ISBN, x => x.MapFrom(y => y.ISBN))
                .ForAllOtherMembers(X => X.Ignore());
        }
    }
}
