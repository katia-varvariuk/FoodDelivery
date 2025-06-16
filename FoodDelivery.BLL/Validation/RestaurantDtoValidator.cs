using FluentValidation;
using FoodDelivery.BLL.DTOs;

namespace FoodDelivery.BLL.Validation
{
    public class RestaurantDtoValidator : AbstractValidator<RestaurantDto>
    {
        public RestaurantDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Назва ресторану є обов'язковою")
                .MaximumLength(100).WithMessage("Назва ресторану не може бути довшою за 100 символів");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Опис ресторану є обов'язковим")
                .MaximumLength(500).WithMessage("Опис ресторану не може бути довшим за 500 символів");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адреса ресторану є обов'язковою")
                .MaximumLength(200).WithMessage("Адреса ресторану не може бути довшою за 200 символів");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Телефон ресторану є обов'язковим")
                .MaximumLength(20).WithMessage("Телефон ресторану не може бути довшим за 20 символів")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Некоректний формат номера телефону");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email ресторану є обов'язковим")
                .EmailAddress().WithMessage("Некоректний формат email")
                .MaximumLength(100).WithMessage("Email ресторану не може бути довшим за 100 символів");

            RuleFor(x => x.LogoUrl)
                .NotEmpty().WithMessage("URL логотипу ресторану є обов'язковим")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Некоректний URL логотипу");

            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 5).WithMessage("Рейтинг повинен бути в межах від 0 до 5");
        }
    }
}