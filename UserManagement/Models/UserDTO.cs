using System.ComponentModel.DataAnnotations;
using restaurantUtility.Models;

namespace UserManagement.Models
{
    public class UserDTO
    {
        [Required]
        [StringLength(20)]
        public string? UserName { get; set; }            
        [StringLength(40)]
        public string? FirstName { get; set; }
        [StringLength(40)]
        public string? LastName { get; set; }
        [StringLength(254)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        public int? PhoneNumber { get; set; }
        public int? StoreId { get; set; }

        public UserDTO(User user)
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            StoreId = user.StoreId;               
        }
    }
}
