using FluentValidation;
using FoodDelivery.BLL.DTOs;

namespace FoodDelivery.BLL.Validation
{
    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим")
                .EmailAddress().WithMessage("Некоректний формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим");
        }
    }
}