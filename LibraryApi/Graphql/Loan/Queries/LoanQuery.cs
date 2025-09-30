using LibraryApi.Data;
using LibraryApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Graphql.Loan.Queries
{
    [ExtendObjectType("Query")]
    public class LoanQuery
    {
        [UseDbContext(typeof(LibraryDbContext))]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Loans> GetLoans([ScopedService] LibraryDbContext context) =>
            context.Prestamos
                   .Include(l => l.Libro)
                   .Include(l => l.Usuario);

        // Obtener préstamos activos por cédula
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<IEnumerable<Loans>> GetLoansByCedulaAsync(
            [ScopedService] LibraryDbContext context,
            int cedula)
        {
            var user = await context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == cedula);
            if (user == null) return Enumerable.Empty<Loans>();

            return await context.Prestamos
                .Include(l => l.Libro)
                .Include(l => l.Usuario)
                .Where(l => l.Id_Usuario == user.Id && !l.Finalizado)
                .ToListAsync();
        }
    }
}
