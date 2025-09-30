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
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Libros.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BooksDto dto)
        {
            try
            {
                var book = new Books()
                {
                    Titulo = dto.Titulo,
                    Activo = dto.Activo,
                    Ano_Publicacion = dto.Ano_Publicacion,
                    Autor = dto.Autor,
                    Fecha_Actualizacion = DateTime.Now,
                    Cantidad_Disponible = dto.Cantidad_Disponible,
                    Cantidad_total = dto.Cantidad_total,
                };
            await _context.Libros.AddAsync(book);
            await _context.SaveChangesAsync();
            return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BooksDto dto)
        {
            var book = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (book == null) 
                return NotFound();

            book.Titulo = dto.Titulo;
            book.Autor = dto.Autor;
            book.Ano_Publicacion = dto.Ano_Publicacion;
            book.Cantidad_total = dto.Cantidad_total;
            book.Cantidad_Disponible = dto.Cantidad_Disponible;
            book.Activo = dto.Activo;
            book.Fecha_Actualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(book);
        }

        [HttpPut("ChangeStatusToInActive/{id}")]
        public async Task<IActionResult> ChangeStatusToInActive(int id)
        {
            var book = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (book == null) 
                return NotFound();

            book.Activo = false;
            book.Fecha_Actualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok("El Libro ha Cambiado de estado Exitosamente.");
        }
    }
}