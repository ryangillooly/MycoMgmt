using AutoMapper;
using MycoMgmt.Core.Models.DTO;
using MycoMgmt.Core.Models.Mushrooms;

namespace MycoMgmt.Core.Profiles;

public class CultureProfile : Profile
{
    public CultureProfile()
    {
        CreateMap<Culture, CultureDto>();
    }
}