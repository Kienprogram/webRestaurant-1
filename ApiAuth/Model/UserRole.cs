using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAuth.Model
{
    [Table("userroles")]
    public class UserRole
    {
        [Key]
        [Column("userid")]
        public int UserId { get; set; }

        [Column("roleid")]
        public int RoleId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
