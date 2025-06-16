using System;

namespace FoodDelivery.Common.Interfaces
{
    public interface IDateTimeConverter
    {
        DateTime ConvertToUtc(DateTime dateTime);
        DateTime? ConvertToUtc(DateTime? dateTime);
    }
}