using FluentValidation;
using Products.API.DTOs;
using Products.API.Repositories;

namespace Products.API.Validations;

public class UpdateProductDTOValidator : AbstractValidator<UpdateProductDTO>
{
    private readonly IProductRepository _repository;

    public UpdateProductDTOValidator(IProductRepository repository)
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name must not be empty.")
            .MaximumLength(150).WithMessage("Name length must be less than or equal to 150 characters only.");

        RuleFor(dTO => dTO).MustAsync(NewName).WithMessage("Name must not be existing.");

        RuleFor(p => p.Price).GreaterThan(-1).WithMessage("Price must not be less than 0.00");
        this._repository = repository;
    }

    private async Task<bool> NewName(UpdateProductDTO dTO, CancellationToken cancellation)
    {
        return await _repository.NameExists(dTO.Name, dTO.Id) == false;
    }
}
