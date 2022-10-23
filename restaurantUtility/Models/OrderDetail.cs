using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace restaurantUtility.Models
{
    [Table("order_details")]
    [Index(nameof(ProductId), Name = "product_id")]
    public partial class OrderDetail
    {
        [Key]
        [Column("order_id", TypeName = "int(11)")]
        public int OrderId { get; set; }
        [Key]
        [Column("product_id", TypeName = "mediumint(9)")]
        public int ProductId { get; set; }
        [Column("quantity", TypeName = "tinyint(4)")]
        public byte Quantity { get; set; }
        [Column("price", TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        [Column("instruction")]
        [StringLength(100)]
        public string Instruction { get; set; }
        [Column("rating", TypeName = "double(1,0)")]
        public double? Rating { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderDetails")]
        public virtual Order Order { get; set; }
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("OrderDetails")]
        public virtual Product Product { get; set; }
    }
}
