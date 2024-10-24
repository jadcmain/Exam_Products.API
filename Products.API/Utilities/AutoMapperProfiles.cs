using AutoMapper;
using Products.API.DTOs;
using Products.API.Entities;

namespace Products.API.Utilities;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<CreateProductDTO, Product>();
        CreateMap<UpdateProductDTO, Product>();
    }
}
