using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.User
{
    public class Users
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

    [Required]
    [StringLength(200)]
    // store hashed password - map to existing DB column 'Password' to avoid migration
    [Column("Password")]
    public required string PasswordHash { get; set; }


        [StringLength(100)]
        public required string FirstName { get; set; }

        [StringLength(100)]
        public required string LastName { get; set; }
    }
}




