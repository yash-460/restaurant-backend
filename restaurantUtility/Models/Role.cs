using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AuthService.Models
{
    [Table("roles")]
    public partial class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        [Key]
        [Column("role_id")]
        [StringLength(10)]
        public string RoleId { get; set; }
        [Required]
        [Column("description")]
        [StringLength(100)]
        public string Description { get; set; }

        [InverseProperty(nameof(UserRole.RoleNavigation))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
