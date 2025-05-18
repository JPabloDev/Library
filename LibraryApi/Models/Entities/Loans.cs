using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApi.Models.Entities
{
    public class Loans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Id_Libros { get; set; }
        public int Id_Usuario { get; set; }
        public DateTime Fecha_Prestamo { get; set; }
        public DateTime? Fecha_Devolucion { get; set; }
        public bool Finalizado { get; set; }

        [ForeignKey(nameof(Id_Libros))]
        public Books? Libro { get; set; }

        [ForeignKey(nameof(Id_Usuario))]
        public Users? Usuario { get; set; }
    }
}
