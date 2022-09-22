using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AuthService.Models
{
    [Table("favourite")]
    [Index(nameof(ProductId), Name = "product_id")]
    public partial class Favourite
    {
        [Key]
        [Column("user_name")]
        [StringLength(20)]
        public string UserName { get; set; }
        [Key]
        [Column("product_id", TypeName = "mediumint(9)")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("Favourites")]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(UserName))]
        [InverseProperty(nameof(User.Favourites))]
        public virtual User UserNameNavigation { get; set; }
    }
}
