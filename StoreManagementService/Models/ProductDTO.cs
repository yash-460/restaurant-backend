using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagementService.Models
{
    public class ProductDTO
    {
        [Key]
        public int ProductId { get; set; }
        public short StoreId { get; set; }
        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(100)]
        public string Description { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public byte[]? ImgLoc { get; set; }
        public bool Active { get; set; }

    }
}
