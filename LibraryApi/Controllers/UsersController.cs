using LibraryApi.Data;
using LibraryApi.Models.DTOs;
using LibraryApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SoloAdmins")]
    public class UsersController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public UsersController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet("ObtenerTodos")]
        public async Task<IActionResult> GetAll() => Ok(await _context.Usuarios.ToListAsync());

        [HttpGet("{Cedula}")]
        public async Task<IActionResult> GetByCedula(int Cedula)
        {
            return Ok(await _context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == Cedula));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsersDto dto)
        {
            var user = new Users()
            {
                Usuario = dto.Usuario,
                Activo = dto.Activo,
                Cedula = dto.Cedula,
                Contrasena = dto.Contrasena,
                Admin = false,
                Nombre = dto.Nombre,
                Fecha_Actualizacion = DateTime.Now
            };
            await _context.Usuarios.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("{cedula}")]
        public async Task<IActionResult> Update(int cedula, [FromBody] UsersDto updated)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == updated.Cedula);
            if (user == null)
                return NotFound();

            user.Nombre = updated.Nombre;
            user.Cedula = updated.Cedula;
            user.Contrasena = updated.Contrasena;
            user.Usuario = updated.Usuario;
            user.Activo = updated.Activo;
            user.Fecha_Actualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("ChangeStatusToInActive/{cedula}")]
        public async Task<IActionResult> ChangeStateToInActive(int cedula)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == cedula);
            if (user == null)
                return NotFound();

            user.Fecha_Actualizacion = DateTime.Now;
            user.Activo =false;
            await _context.SaveChangesAsync();
            return Ok("Usuario desactivado.");
        }
    }
}