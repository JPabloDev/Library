using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public UsersController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet("ObtenerTodos")]
        public async Task<IActionResult> GetAll() => Ok(await _context.Users.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            return Ok(await _context.Users.FindAsync(Id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            user.PasswordHash = user.PasswordHash;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updated)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) 
                return NotFound();

            user.Fullname = updated.Fullname;
            user.Username = updated.Username;
            user.Role = updated.Role;
            user.PasswordHash = updated.PasswordHash;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("/change-state{id}")]
        public async Task<IActionResult> ChangeState(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) 
                return NotFound();

            user.Active = !user.Active;
            await _context.SaveChangesAsync();
            return Ok("Usuario desactivado.");
        }
    }
}