using FluentValidation;
using FoodDelivery.BLL.DTOs;

namespace FoodDelivery.BLL.Validation
{
    public class RestaurantFilterDtoValidator : AbstractValidator<RestaurantFilterDto>
    {
        public RestaurantFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Номер сторінки повинен бути більше 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50).WithMessage("Розмір сторінки повинен бути в межах від 1 до 50");

            RuleFor(x => x.MinRating)
                .InclusiveBetween(0, 5).WithMessage("Мінімальний рейтинг повинен бути в межах від 0 до 5")
                .When(x => x.MinRating.HasValue);

            RuleFor(x => x.MaxRating)
                .InclusiveBetween(0, 5).WithMessage("Максимальний рейтинг повинен бути в межах від 0 до 5")
                .When(x => x.MaxRating.HasValue);

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) || sortBy.ToLower() == "name" || sortBy.ToLower() == "rating")
                .WithMessage("Сортування може бути лише за полями 'name' або 'rating'");
        }
    }
}