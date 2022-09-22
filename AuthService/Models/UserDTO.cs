using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class UserDTO
    {
        [StringLength(20)]
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
