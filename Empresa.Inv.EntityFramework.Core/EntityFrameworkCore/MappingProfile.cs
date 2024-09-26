using AutoMapper;
using Empresa.Inv.Application.Shared.Dtos;
using Empresa.Inv.Core.Entities;

namespace Empresa.Inv.EntityFramework.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Configuración de mapeo para Product
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();
            

            CreateMap<UserDTO, User>().ReverseMap();

          

            // Puedes agregar más configuraciones de mapeo aquí
            // Ejemplo:
            // CreateMap<OtherEntity, OtherDTO>();
            // CreateMap<OtherDTO, OtherEntity>();
        }


    }
}
