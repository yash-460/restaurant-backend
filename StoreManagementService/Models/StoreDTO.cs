using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagementService.Models
{
    public class StoreDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short StoreId { get; set; }
        [Required]
        [StringLength(25)]
        public string? Name { get; set; }
        [Column("registration_number", TypeName = "int(9)")]
        public int RegistrationNumber { get; set; }
        [Column("tax_rate", TypeName = "decimal(4,2)")]
        public decimal TaxRate { get; set; }
        [Required]
        [StringLength(50)]
        public string? StreetName { get; set; }
        [Required]
        [StringLength(25)]
        public string? City { get; set; }
        [Required]
        [StringLength(20)]
        public string? Province { get; set; }
        [Required]
        [StringLength(7)]
        public string? Zip { get; set; }
        public byte[]? ImgLoc { get; set; }
    }
}
