using Bitacora.Domain;

namespace Bitacora.Application;

public interface IRegistroRepository
{
    Task<IReadOnlyList<Registro>> ListarAsync(int usuarioId, bool esAdmin, CancellationToken cancellationToken = default);

    Task<Registro?> ObtenerPorIdAsync(
        int id, CancellationToken cancellationToken = default);

    Task<Registro> CrearAsync(
        Registro registro, CancellationToken cancellationToken = default);

    Task<bool> ActualizarAsync(
        Registro registro, CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(
        int id, CancellationToken cancellationToken = default);
}
