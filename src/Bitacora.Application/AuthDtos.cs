namespace Bitacora.Application;

public class RegistroUsuarioDto
{
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = "Usuario"; 
}

public class LoginDto
{
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
    public string Rol { get; set; } = string.Empty;
}