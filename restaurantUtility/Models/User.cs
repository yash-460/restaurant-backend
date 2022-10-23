using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace restaurantUtility.Models
{
    [Table("user")]
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Favourites = new HashSet<Favourite>();
            Orders = new HashSet<Order>();
            UserRoles = new HashSet<UserRole>();
        }

        [Key]
        [Column("user_name")]
        [StringLength(20)]
        public string UserName { get; set; }
        [Required]
        [Column("password")]
        [StringLength(256)]
        public string Password { get; set; }
        [Column("first_name")]
        [StringLength(40)]
        public string? FirstName { get; set; }
        [Column("last_name")]
        [StringLength(40)]
        public string? LastName { get; set; }
        [Column("email")]
        [StringLength(254)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Column("phone_number", TypeName = "int(10)")]
        public int? PhoneNumber { get; set; }
        [Column("store_id", TypeName = "int(4)")]
        public int? StoreId { get; set; }

        [InverseProperty(nameof(Cart.UserNameNavigation))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Favourite.UserNameNavigation))]
        public virtual ICollection<Favourite> Favourites { get; set; }
        [InverseProperty(nameof(Order.UserNameNavigation))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(UserRole.UserNameNavigation))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
