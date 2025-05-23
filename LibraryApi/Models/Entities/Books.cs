using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApi.Models.Entities
{
    public class Books
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int Ano_Publicacion { get; set; }
        public int Cantidad_total { get; set; }
        public int Cantidad_Disponible { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime Fecha_Actualizacion { get; set; }

        [JsonIgnore]
        public ICollection<Loans> Prestamos { get; set; }
    }
}
