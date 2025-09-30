using LibraryApi.Data;
using LibraryApi.Models.Entities;

namespace LibraryApi.Graphql.User.Queries
{
    [ExtendObjectType("Query")]
    public class UserQuery
    {
        // Obtener todos los usuarios
        [UseDbContext(typeof(LibraryDbContext))]
        [UseFiltering] // opcional
        [UseSorting]   // opcional
        public IQueryable<Users> GetUsuarios([ScopedService] LibraryDbContext context) =>
            context.Usuarios;

        // Obtener un usuario por Id
        [UseDbContext(typeof(LibraryDbContext))]
        public Users? GetUsuarioById([ScopedService] LibraryDbContext context, int id) =>
            context.Usuarios.FirstOrDefault(u => u.Id == id);

        // Obtener un usuario por username
        [UseDbContext(typeof(LibraryDbContext))]
        public Users? GetUsuarioByUsername([ScopedService] LibraryDbContext context, string username) =>
            context.Usuarios.FirstOrDefault(u => u.Usuario == username);
    }
}
