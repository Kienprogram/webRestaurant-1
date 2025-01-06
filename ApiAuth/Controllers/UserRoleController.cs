using ApiAuth.Data;
using ApiAuth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> AssignRoleToUser([FromBody] UserRole userRole)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userRole.UserId);
            if (!userExists)
                return NotFound("User not found.");

            var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == userRole.RoleId);
            if (!roleExists)
                return NotFound("Role not found.");

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveRoleFromUser([FromBody] UserRole userRole)
        {
            var userRoleToRemove = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId);
            if (userRoleToRemove == null)
                return NotFound("User-Role association not found.");

            _context.UserRoles.Remove(userRoleToRemove);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
