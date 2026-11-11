namespace Bitacora.Domain;

public class Registro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public Fotografia? Fotografia { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}