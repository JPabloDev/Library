using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models.Entities
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public bool Admin { get; set; }
        public string Contrasena { get; set; }
        public string Usuario { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime Fecha_Actualizacion { get; set; }

        public ICollection<Loans> Prestamos { get; set; }
    }
}
