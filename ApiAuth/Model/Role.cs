using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAuth.Model
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("roleid")]
        public int RoleId { get; set; }
        [Column("rolename")]
        public string RoleName { get; set; } = string.Empty;
    }
}
