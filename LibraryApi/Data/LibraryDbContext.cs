using LibraryApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        { }
        public DbSet<Users> Usuarios { get; set; }
        public DbSet<Books> Libros { get; set; }
        public DbSet<Loans> Prestamos { get; set; }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loans>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(l => l.Id_Usuario);

            modelBuilder.Entity<Loans>()
                .HasOne(l => l.Libro)
                .WithMany(b => b.Prestamos)
                .HasForeignKey(l => l.Id_Libros);
        }
    }
}