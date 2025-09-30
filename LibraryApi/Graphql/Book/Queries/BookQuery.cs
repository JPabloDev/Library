using LibraryApi.Data;
using LibraryApi.Models.Entities;

namespace LibraryApi.Graphql.Book.Queries
{
    [ExtendObjectType("Query")]
    public class BookQuery
    {
        // Obtener todos los libros
        [UseDbContext(typeof(LibraryDbContext))]
        [UseFiltering]   // opcional
        [UseSorting]     // opcional
        public IQueryable<Books> GetBooks([ScopedService] LibraryDbContext context) =>
            context.Libros;

        // Obtener un libro por ID
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<Books?> GetBookById([ScopedService] LibraryDbContext context, int id) =>
             context.Libros.FirstOrDefault(b => b.Id == id);

    }
}
