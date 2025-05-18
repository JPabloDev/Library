namespace LibraryApi.Models.DTOs
{
    public class BooksDto
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int Ano_Publicacion { get; set; }
        public int Cantidad_total { get; set; }
        public int Cantidad_Disponible { get; set; }
        public bool Activo { get; set; } = true;

    }
}
