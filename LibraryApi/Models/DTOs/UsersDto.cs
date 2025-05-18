namespace LibraryApi.Models.DTOs
{
    public class UsersDto
    {
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public string Contrasena { get; set; }
        public string Usuario { get; set; }
        public bool Activo { get; set; } = true;
    }
}
