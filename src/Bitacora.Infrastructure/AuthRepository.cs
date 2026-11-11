using Bitacora.Application;
using Bitacora.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bitacora.Infrastructure;

public class AuthRepository : IAuthRepository
{
    private readonly BitacoraDbContext _context;

    public AuthRepository(BitacoraDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.Correo == correo, ct);
    }

    public async Task CrearAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default)
    {
        return await _context.Usuarios.SingleOrDefaultAsync(u => u.Correo == correo, ct);
    }
}