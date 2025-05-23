using LibraryApi.Data;
using LibraryApi.Models.DTOs;
using LibraryApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SoloAdmins")]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoansController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var prestamos = await _context.Prestamos
                    .Include(l => l.Libro)
                    .Include(x => x.Usuario)
                    .ToListAsync();

                return Ok(prestamos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoansDto loanDto)
        {
            try{
                var book = await _context.Libros.FirstOrDefaultAsync(x => x.Id == loanDto.IdLibro);
                if (book == null || book.Cantidad_Disponible < 1)
                    return BadRequest("El libro no está disponible.");

                var user = await _context.Usuarios.FirstOrDefaultAsync(y => y.Cedula == loanDto.CedulaUsuario);
                if (user == null || !user.Activo)
                    return BadRequest("El Usuario no esta activo.");

                book.Cantidad_Disponible -= 1;

                var loan = new Loans()
                {
                    Fecha_Prestamo = DateTime.Now,
                    Finalizado = false,
                    Id_Libros = book.Id,
                    Id_Usuario = user.Id,
                };

                await _context.Prestamos.AddAsync(loan);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    PrestamoId = loan.Id,
                    Usuario = user.Usuario,
                    Libro = book.Titulo,
                    FechaPrestamo = loan.Fecha_Prestamo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }


        [HttpGet("GetLoanByCedula/{cedula}")]
        public async Task<IActionResult> GetLoanByCedula(int cedula)
        {
            try
            {
                var user = await _context.Usuarios.FirstOrDefaultAsync(y => y.Cedula == cedula);
                if (user == null)
                    return NotFound("Usuario no encontrado");

                var loans = await _context.Prestamos
                    .Include(l => l.Libro)
                    .Include(l => l.Usuario)
                    .Where(u => u.Id_Usuario == user.Id && u.Finalizado == false).ToListAsync();

                if (!loans.Any())
                    return NotFound("No hay Préstamos con esa cédula");

                var result = loans.Select(loan => new
                {
                    loan.Id,
                    loan.Fecha_Prestamo,
                    loan.Fecha_Devolucion,
                    loan.Finalizado,
                    Libro = new { loan.Libro.Titulo, loan.Libro.Autor },
                    Usuario = new { loan.Usuario.Nombre, loan.Usuario.Cedula }
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var loan = await _context.Prestamos.FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null) 
                return NotFound();

            var book = await _context.Libros.FirstOrDefaultAsync(v => v.Id == loan.Id_Libros);
            if (book != null) 
                book.Cantidad_Disponible += 1;

            loan.Fecha_Devolucion = DateTime.Now;
            loan.Finalizado = true;
               
            await _context.SaveChangesAsync();
            return Ok("Libro devuelto.");
        }
    }
}
