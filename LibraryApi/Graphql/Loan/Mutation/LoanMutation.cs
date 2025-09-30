using LibraryApi.Data;
using LibraryApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Graphql.Loan.Mutation
{
    [ExtendObjectType("Mutation")]
    public class LoanMutation
    {
        // Crear préstamo
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<Loans?> CreateLoan(
            [ScopedService] LibraryDbContext context,
            int idLibro,
            int cedulaUsuario)
        {
            var book = await context.Libros.FirstOrDefaultAsync(x => x.Id == idLibro);
            if (book == null || book.Cantidad_Disponible < 1)
                throw new GraphQLException("El libro no está disponible.");

            var user = await context.Usuarios.FirstOrDefaultAsync(y => y.Cedula == cedulaUsuario);
            if (user == null || !user.Activo)
                throw new GraphQLException("El usuario no está activo.");

            book.Cantidad_Disponible -= 1;

            var loan = new Loans
            {
                Fecha_Prestamo = DateTime.Now,
                Finalizado = false,
                Id_Libros = book.Id,
                Id_Usuario = user.Id
            };

            await context.Prestamos.AddAsync(loan);
            await context.SaveChangesAsync();

            return loan;
        }

        // Devolver libro
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<string> ReturnBook(
            [ScopedService] LibraryDbContext context,
            int id)
        {
            var loan = await context.Prestamos.FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null)
                throw new GraphQLException("Préstamo no encontrado.");

            var book = await context.Libros.FirstOrDefaultAsync(v => v.Id == loan.Id_Libros);
            if (book != null)
                book.Cantidad_Disponible += 1;

            loan.Fecha_Devolucion = DateTime.Now;
            loan.Finalizado = true;

            await context.SaveChangesAsync();
            return "Libro devuelto.";
        }
    }
}
