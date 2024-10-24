using FluentValidation;
using Products.API.DTOs;
using Products.API.Repositories;

namespace Products.API.Validations;

public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
{
    private readonly IProductRepository _repository;

    public CreateProductDTOValidator(IProductRepository repository)
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name must not be empty.")
            .MaximumLength(150).WithMessage("Name length must be less than or equal to 150 characters only.")
            .MustAsync(NewName).WithMessage("Name must not be existing.");

        RuleFor(p => p.Price).GreaterThan(-1).WithMessage("Price must not be less than 0.00");
        this._repository = repository;
    }

    private async Task<bool> NewName(string name, CancellationToken cancellation)
    {
        return await _repository.NameExists(name, 0) == false;
    }
}
