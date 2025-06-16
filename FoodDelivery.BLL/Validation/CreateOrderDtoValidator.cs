using FluentValidation;
using FoodDelivery.BLL.DTOs;

namespace FoodDelivery.BLL.Validation
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.RestaurantId)
                .NotEmpty().WithMessage("Ідентифікатор ресторану є обов'язковим");

            RuleFor(x => x.DeliveryAddress)
                .NotEmpty().WithMessage("Адреса доставки є обов'язковою")
                .MaximumLength(200).WithMessage("Адреса доставки не може бути довшою за 200 символів");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Контактний телефон є обов'язковим")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Некоректний формат номера телефону");

            RuleFor(x => x.OrderItems)
                .NotEmpty().WithMessage("Замовлення повинно містити хоча б одну страву");

            RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());
        }
    }

    public class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto>
    {
        public OrderItemCreateDtoValidator()
        {
            RuleFor(x => x.MenuItemId)
                .NotEmpty().WithMessage("Ідентифікатор страви є обов'язковим");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Кількість є обов'язковою")
                .InclusiveBetween(1, 100).WithMessage("Кількість повинна бути в межах від 1 до 100");
        }
    }
}