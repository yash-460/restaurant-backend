using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace restaurantUtility.Models
{
    [Table("user_role")]
    [Index(nameof(Role), Name = "role")]
    public partial class UserRole
    {
        [Key]
        [Column("user_name")]
        [StringLength(20)]
        public string UserName { get; set; }
        [Key]
        [Column("role")]
        [StringLength(10)]
        public string Role { get; set; }

        [ForeignKey(nameof(Role))]
        [InverseProperty("UserRoles")]
        public virtual Role RoleNavigation { get; set; }
        [ForeignKey(nameof(UserName))]
        [InverseProperty(nameof(User.UserRoles))]
        public virtual User UserNameNavigation { get; set; }

    }
}
