using FoodDelivery.Common.Interfaces;
using System;

namespace FoodDelivery.Common.Services
{
    public class DateTimeConverter : IDateTimeConverter
    {
        public DateTime ConvertToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime;
            }

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public DateTime ? ConvertToUtc(DateTime ? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }

            return ConvertToUtc(dateTime.Value);
        }
    }
}