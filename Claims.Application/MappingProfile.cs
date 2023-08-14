using AutoMapper;
using Claims.Application.DataTransferObjects;
using Claims.Domain.Entities;

namespace Claims.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Claim, ClaimDataTransferObject>().ReverseMap();
            CreateMap<ClaimAudit, ClaimAuditDataTransferObject>().ReverseMap();
            CreateMap<Cover, CoverDataTransferObject>().ReverseMap();
            CreateMap<CoverAudit, CoverAuditDataTransferObject>().ReverseMap();
        }
    }
}
