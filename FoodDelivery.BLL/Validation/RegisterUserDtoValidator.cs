using FluentValidation;
using FoodDelivery.BLL.DTOs;
using System;

namespace FoodDelivery.BLL.Validation
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим")
                .EmailAddress().WithMessage("Некоректний формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим")
                .MinimumLength(6).WithMessage("Пароль повинен містити не менше 6 символів")
                .Matches("[A-Z]").WithMessage("Пароль повинен містити хоча б одну велику літеру")
                .Matches("[0-9]").WithMessage("Пароль повинен містити хоча б одну цифру")
                .Matches("[^a-zA-Z0-9]").WithMessage("Пароль повинен містити хоча б один спеціальний символ");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Паролі не співпадають");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим")
                .MaximumLength(50).WithMessage("Ім'я не може бути довшим за 50 символів");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим")
                .MaximumLength(50).WithMessage("Прізвище не може бути довшим за 50 символів");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адреса є обов'язковою")
                .MaximumLength(200).WithMessage("Адреса не може бути довшою за 200 символів");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Дата народження є обов'язковою")
                .LessThan(DateTime.UtcNow.AddYears(-18)).WithMessage("Вам повинно бути більше 18 років");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефону є обов'язковим")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Некоректний формат номера телефону");
        }
    }
}