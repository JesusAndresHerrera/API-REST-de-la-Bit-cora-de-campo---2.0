namespace Bitacora.Domain;

public class Usuario
{
    public int Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Usuario"; 
    
    public ICollection<Registro> Registros { get; set; } = new List<Registro>();
}