using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace restaurantUtility.Models
{
    [Table("orders")]
    [Index(nameof(StoreId), Name = "order_storeId_constraint")]
    [Index(nameof(UserName), Name = "order_user_constraint")]
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        [Column("order_id", TypeName = "int(11)")]
        public int OrderId { get; set; }
        [Required]
        [Column("user_name")]
        [StringLength(20)]
        public string UserName { get; set; }
        [Column("store_id", TypeName = "smallint(6)")]
        public short StoreId { get; set; }
        [Required]
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; }
        [Column("tax", TypeName = "decimal(4,2)")]
        public decimal Tax { get; set; }

        [Column("ordered_time",TypeName = "timestamp")]
        public DateTime OrderedTime { get; set; }

        [ForeignKey(nameof(StoreId))]
        [InverseProperty("Orders")]
        public virtual Store Store { get; set; }
        [ForeignKey(nameof(UserName))]
        [InverseProperty(nameof(User.Orders))]
        public virtual User UserNameNavigation { get; set; }
        [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
