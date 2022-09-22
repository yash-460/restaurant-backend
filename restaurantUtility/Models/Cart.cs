using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AuthService.Models
{
    [Table("cart")]
    [Index(nameof(ProductId), Name = "product_id")]
    public partial class Cart
    {
        [Key]
        [Column("user_name")]
        [StringLength(20)]
        public string UserName { get; set; }
        [Key]
        [Column("product_id", TypeName = "mediumint(9)")]
        public int ProductId { get; set; }
        [Column("instruction")]
        [StringLength(100)]
        public string Instruction { get; set; }
        [Column("quantity", TypeName = "tinyint(4)")]
        public byte Quantity { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("Carts")]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(UserName))]
        [InverseProperty(nameof(User.Carts))]
        public virtual User UserNameNavigation { get; set; }
    }
}
