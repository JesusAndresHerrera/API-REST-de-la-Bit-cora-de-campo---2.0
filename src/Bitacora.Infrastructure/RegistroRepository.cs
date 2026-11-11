using Bitacora.Application;
using Bitacora.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bitacora.Infrastructure;

public class RegistroRepository : IRegistroRepository
{
    private readonly BitacoraDbContext _context;

    public RegistroRepository(BitacoraDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Registro>> ListarAsync(int usuarioId, bool esAdmin, CancellationToken cancellationToken = default)
    {
        var query = _context.Registros.AsNoTracking();

        if (!esAdmin)
        {
         query = query.Where(r => r.UsuarioId == usuarioId);
        }

        return await query
            .OrderByDescending(r => r.FechaCreacion)
            .ThenByDescending(r => r.Id)
            .Select(r => new Registro
         {
              Id = r.Id,
             Titulo = r.Titulo,
              Descripcion = r.Descripcion,
             FechaCreacion = r.FechaCreacion,
              UsuarioId = r.UsuarioId
         })
            .ToListAsync(cancellationToken);
    }

    public async Task<Registro?> ObtenerPorIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _context.Registros
            .AsNoTracking()
            .Include(r => r.Fotografia)
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Registro> CrearAsync(
        Registro registro, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registro);

        _context.Registros.Add(registro);
        await _context.SaveChangesAsync(cancellationToken);
        return registro;
    }

    public async Task<bool> ActualizarAsync(
        Registro registro, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registro);

        var existente = await _context.Registros
            .Include(r => r.Fotografia)
            .SingleOrDefaultAsync(r => r.Id == registro.Id, cancellationToken);

        if (existente is null)
        {
            return false;
        }

        existente.Titulo = registro.Titulo;
        existente.Descripcion = registro.Descripcion;

        if (registro.Fotografia is null)
        {
            if (existente.Fotografia is not null)
            {
                _context.Fotografias.Remove(existente.Fotografia);
                existente.Fotografia = null;
            }
        }
        else if (existente.Fotografia is null)
        {
            existente.Fotografia = new Fotografia
            {
                RegistroId = existente.Id,
                FotoBase64 = registro.Fotografia.FotoBase64
            };
        }
        else
        {
            existente.Fotografia.FotoBase64 = registro.Fotografia.FotoBase64;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var registro = await _context.Registros
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (registro is null)
        {
            return false;
        }

        _context.Registros.Remove(registro);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
