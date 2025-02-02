using AutoMapper;
using ITnetworkProjekt.Models;

namespace ITnetworkProjekt
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<InsuredPerson, InsuredPersonViewModel>();
            CreateMap<InsuredPersonViewModel, InsuredPerson>();
            CreateMap<Insurance, InsuranceViewModel>();
            CreateMap<InsuranceViewModel, Insurance>();
        }
    }
}
