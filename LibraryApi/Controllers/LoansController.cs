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
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoansController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Loans.Include(l => l.Book).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Loan loan)
        {
            var book = await _context.Books.FindAsync(loan.BookId);
            if (book == null || book.Quantity < 1)
                return BadRequest("El libro no está disponible.");

            book.Quantity -= 1;
            loan.LoanDate = DateTime.Now;

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();
            return Ok(loan);
        }

        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null) book.Quantity += 1;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return Ok("Libro devuelto.");
        }
    }
}
