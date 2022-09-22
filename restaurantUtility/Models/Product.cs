using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AuthService.Models
{
    [Table("product")]
    [Index(nameof(StoreId), Name = "product_store_constraint")]
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            Favourites = new HashSet<Favourite>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        [Column("product_id", TypeName = "mediumint(9)")]
        public int ProductId { get; set; }
        [Column("store_id", TypeName = "smallint(6)")]
        public short StoreId { get; set; }
        [Required]
        [Column("product_name")]
        [StringLength(40)]
        public string ProductName { get; set; }
        [Required]
        [Column("description")]
        [StringLength(100)]
        public string Description { get; set; }
        [Column("price", TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        [Required]
        [Column("img_loc")]
        [StringLength(200)]
        public string ImgLoc { get; set; }
        [Column("rating", TypeName = "double(1,0)")]
        public double Rating { get; set; }
        [Column("active")]
        public bool Active { get; set; }

        [ForeignKey(nameof(StoreId))]
        [InverseProperty("Products")]
        public virtual Store Store { get; set; }
        [InverseProperty(nameof(Cart.Product))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Favourite.Product))]
        public virtual ICollection<Favourite> Favourites { get; set; }
        [InverseProperty(nameof(OrderDetail.Product))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
