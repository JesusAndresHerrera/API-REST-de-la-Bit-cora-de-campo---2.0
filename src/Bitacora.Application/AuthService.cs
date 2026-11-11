using Bitacora.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bitacora.Application;

public class AuthService
{
    private readonly IAuthRepository _repository;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(IAuthRepository repository, string jwtKey, string jwtIssuer, string jwtAudience)
    {
        _repository = repository;
        _jwtKey = jwtKey;
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
    }

    public async Task<string> RegistrarAsync(RegistroUsuarioDto dto, CancellationToken ct = default)
    {
        if (await _repository.ExisteCorreoAsync(dto.Correo, ct))
            throw new Exception("El correo ya está registrado.");

        // Hashear contraseña con BCrypt
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        
        var usuario = new Usuario
        {
            Correo = dto.Correo,
            PasswordHash = hash,
            Rol = dto.Rol == "Admin" ? "Admin" : "Usuario"
        };

        await _repository.CrearAsync(usuario, ct);
        return "Usuario registrado exitosamente.";
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var usuario = await _repository.ObtenerPorCorreoAsync(dto.Correo, ct);
        
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var token = GenerarToken(usuario);
        
        return new AuthResponseDto
        {
            Token = token,
            Expiracion = DateTime.UtcNow.AddMinutes(10),
            Rol = usuario.Rol
        };
    }

    private string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddMinutes(10);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: expiracion,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public interface IAuthRepository
{
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct = default);
    Task CrearAsync(Usuario usuario, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default);
}