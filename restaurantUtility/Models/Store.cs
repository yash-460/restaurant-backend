using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AuthService.Models
{
    [Table("store")]
    public partial class Store
    {
        public Store()
        {
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
        }

        [Key]
        [Column("store_id", TypeName = "smallint(6)")]
        public short StoreId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(25)]
        public string Name { get; set; }
        [Column("registration_number", TypeName = "int(9)")]
        public int RegistrationNumber { get; set; }
        [Column("tax_rate", TypeName = "decimal(4,2)")]
        public decimal TaxRate { get; set; }
        [Required]
        [Column("street_name")]
        [StringLength(50)]
        public string StreetName { get; set; }
        [Required]
        [Column("city")]
        [StringLength(25)]
        public string City { get; set; }
        [Required]
        [Column("province")]
        [StringLength(20)]
        public string Province { get; set; }
        [Required]
        [Column("zip")]
        [StringLength(6)]
        public string Zip { get; set; }
        [Column("rating", TypeName = "int(1)")]
        public int? Rating { get; set; }
        [Column("img_loc")]
        [StringLength(200)]
        public string ImgLoc { get; set; }

        [InverseProperty(nameof(Order.Store))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Product.Store))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
