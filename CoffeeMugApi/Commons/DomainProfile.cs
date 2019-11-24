using AutoMapper;

namespace CoffeeMugApi.Commons
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<DA.DtoModels.Product, Models.Product>();
            CreateMap<Models.Product, DA.DtoModels.Product>();
        }

    }
}