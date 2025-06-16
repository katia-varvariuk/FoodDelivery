namespace FoodDelivery.BLL.DTOs
{
    public class RestaurantFilterDto
    {
        public string SearchTerm { get; set; }
        public double? MinRating { get; set; }
        public double? MaxRating { get; set; }
        public string SortBy { get; set; } = "Name";
        public bool SortDesc { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}