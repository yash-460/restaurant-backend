using Microsoft.Build.Framework;

namespace StoreManagementService.Models
{
    public class RateDTO
    {
        [Required]
        public short OrderId { get; set; }
        [Required]
        public List<Ratings> ratings { get; set; }
    }

    public class Ratings {
        public int ProductId { get; set; }
        public double? Rating { get; set; }
    }
}
