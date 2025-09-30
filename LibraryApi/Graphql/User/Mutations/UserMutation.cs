using LibraryApi.Data;
using LibraryApi.Models.DTOs;
using LibraryApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Graphql.User.Mutations
{
    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
        // Crear usuario
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<Users> CreateUsuarioAsync([ScopedService] LibraryDbContext context,UsersDto User)
        {
            var user = new Users()
            {
                Usuario = User.Usuario,
                Activo = User.Activo,
                Cedula = User.Cedula,
                Contrasena = User.Contrasena,
                Admin = false,
                Nombre = User.Nombre,
                Fecha_Actualizacion = DateTime.Now
            };

            context.Usuarios.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        // Actualizar usuario
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<Users?> UpdateUsuarioAsync([ScopedService] LibraryDbContext context, int cedula, UsersDto updated)
        {

            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == updated.Cedula);
            if (usuario == null) return null;

            await context.SaveChangesAsync();
            return usuario;
        }

        // Eliminar usuario
        [UseDbContext(typeof(LibraryDbContext))]
        public async Task<bool> ChangeStatusUsuarioAsync([ScopedService] LibraryDbContext context,int cedula)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == cedula);
            if (usuario == null) return false;

            usuario.Fecha_Actualizacion = DateTime.Now;
            usuario.Activo = false;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
