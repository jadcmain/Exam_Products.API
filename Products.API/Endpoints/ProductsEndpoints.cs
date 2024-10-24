using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Products.API.DTOs;
using Products.API.Entities;
using Products.API.Repositories;

namespace Products.API.Endpoints;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetProducts).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get")).RequireAuthorization();
        group.MapGet("/{id:int}", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))).RequireAuthorization();
        group.MapPost("/", Create).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        group.MapPut("/{id:int}", Update).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        group.MapDelete("/{id:int}", Delete).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        return group;
    }

    static async Task<Ok<List<ProductDTO>>> GetProducts(IProductRepository repository, IMapper mapper)
    {
        var products = await repository.GetAll();
        var productsDTO = mapper.Map<List<ProductDTO>>(products);
        return TypedResults.Ok(productsDTO);
    }

    static async Task<Results<Ok<ProductDTO>, NotFound>> GetById(int id, IProductRepository repository, IMapper mapper)
    {
        var product = await repository.GetById(id);
        var productDTO = mapper.Map<ProductDTO>(product);
        if (product is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(productDTO);
    }

    static async Task<Results<Created<ProductDTO>, ValidationProblem>> Create(CreateProductDTO createProductDTO,
        IProductRepository repository,
        IOutputCacheStore outputCacheStore,
        IMapper mapper,
        IValidator<CreateProductDTO> validator)
    {
        var validationResult = await validator.ValidateAsync(createProductDTO);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var product = mapper.Map<Product>(createProductDTO);
        var id = await repository.Create(product);

        // clear cache
        await outputCacheStore.EvictByTagAsync("products-get", default);

        var productDTO = mapper.Map<ProductDTO>(product);
        return TypedResults.Created($"/product/{id}", productDTO);
    }

    static async Task<Results<NotFound, NoContent, ValidationProblem>> Update(int id,
        UpdateProductDTO updateProductDTO,
        IProductRepository repository,
        IOutputCacheStore outputCashStore,
        IMapper mapper,
        IValidator<UpdateProductDTO> validator)
    {
        var validationResult = await validator.ValidateAsync(updateProductDTO);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var exists = await repository.Exists(id);
        if (!exists)
            return TypedResults.NotFound();

        var product = mapper.Map<Product>(updateProductDTO);
        product.Id = id;

        await repository.Update(product);

        // clear cache
        await outputCashStore.EvictByTagAsync("products-get", default);

        // nothing to return here
        return TypedResults.NoContent();
    }

    static async Task<Results<NoContent, NotFound>> Delete(int id, IProductRepository repository, IOutputCacheStore outputCashStore)
    {
        var exists = await repository.Exists(id);
        if (!exists)
            return TypedResults.NotFound();

        await repository.Delete(id);

        // clear cache
        await outputCashStore.EvictByTagAsync("products-get", default);

        // nothing to return here
        return TypedResults.NoContent();
    }
}
