using AutoMapper;
using MycoMgmt.Domain.Models.DTO;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Domain.Profiles;

public class CultureProfile : Profile
{
    public CultureProfile()
    {
        CreateMap<Culture, CultureDto>();
    }
}