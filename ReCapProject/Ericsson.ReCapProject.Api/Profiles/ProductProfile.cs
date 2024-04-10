using AutoMapper;
using Ericsson.ReCapProject.Api.ViewModels;
using Ericsson.ReCapProject.Core.Entitites;

namespace Ericsson.ReCapProject.Api.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductViewModel>()
                .ReverseMap();
        }
    }
}
