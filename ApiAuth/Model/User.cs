using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ApiAuth.Model
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("userid")]
        public int UserId { get; set; }
        [Column("username")]
        public string Username { get; set; } = string.Empty;
        [Column("passwordhash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("fullname")]
        public string FullName { get; set; } = string.Empty;
    }
}
