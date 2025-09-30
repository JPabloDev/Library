using LibraryApi.Data;
using LibraryApi.Models.Entities;
using System;

namespace LibraryApi.Graphql.Book.Mutations
{
    [ExtendObjectType("Mutation")]
    public class BookMutation
    {
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<Books> AddBookAsync([ScopedService] LibraryDbContext context, string title, string author,int cantidad_disponible,int ano_publicacion,bool activo,int cantidad_total)
        {
            var book = new Books {
                Titulo = title,
                Autor = author,
                Activo = activo,
                Ano_Publicacion = ano_publicacion,
                Cantidad_Disponible = cantidad_disponible,
                Cantidad_total = cantidad_total,
                Fecha_Actualizacion = DateTime.Now
            };
            context.Libros.Add(book);
            await context.SaveChangesAsync();
            return book;
        }

        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<Books?> UpdateBookAsync([ScopedService] LibraryDbContext context, int id, string title, string author)
        {
            var book = await context.Libros.FindAsync(id);
            if (book == null) return null;

            book.Titulo = title;
            book.Autor = author;
            await context.SaveChangesAsync();
            return book;
        }

        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<bool> DeleteBookAsync([ScopedService] LibraryDbContext context, int id)
        {
            var book = await context.Libros.FindAsync(id);
            if (book == null) return false;

            context.Libros.Remove(book);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
