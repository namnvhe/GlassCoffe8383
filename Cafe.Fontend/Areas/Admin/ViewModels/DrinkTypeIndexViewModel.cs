using Cafe.BusinessObjects.Models.Response;

namespace Cafe.Fontend.Areas.Admin.ViewModels
{
    public class DrinkTypeIndexViewModel
    {
        public List<DrinkTypeResponse> DrinkTypes { get; set; } = new List<DrinkTypeResponse>();
        public List<PopularDrinkTypeResponse> PopularDrinkTypes { get; set; } = new List<PopularDrinkTypeResponse>();
        public int TotalDrinkTypes { get; set; }
        public int ActiveDrinkTypes { get; set; }
        public int InactiveDrinkTypes { get; set; }
        public int TotalMenuItems { get; set; }
        public double AverageMenuItemsPerType { get; set; }
    }
}
