using FluentValidation;
using FoodDelivery.BLL.DTOs;

namespace FoodDelivery.BLL.Validation
{
    public class MenuItemDtoValidator : AbstractValidator<MenuItemDto>
    {
        public MenuItemDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Назва страви є обов'язковою")
                .MaximumLength(100).WithMessage("Назва страви не може бути довшою за 100 символів");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Опис страви є обов'язковим")
                .MaximumLength(500).WithMessage("Опис страви не може бути довшим за 500 символів");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Ціна страви є обов'язковою")
                .GreaterThan(0).WithMessage("Ціна повинна бути більше 0");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("URL зображення страви є обов'язковим")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Некоректний URL зображення");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Ідентифікатор категорії є обов'язковим");

            RuleFor(x => x.RestaurantId)
                .NotEmpty().WithMessage("Ідентифікатор ресторану є обов'язковим");
        }
    }
}