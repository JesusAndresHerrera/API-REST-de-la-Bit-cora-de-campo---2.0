using System.ComponentModel.DataAnnotations;
using Bitacora.Domain;

namespace Bitacora.Application;

public class GuardarRegistroDto : IValidatableObject
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    public string? FotoBase64 { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errores = RegistroReglas.Validar(Titulo, Descripcion, FotoBase64);
        foreach (var error in errores)
        {
            foreach (var mensaje in error.Value)
            {
                yield return new ValidationResult(mensaje, new[] { error.Key });
            }
        }
    }
}

public class RegistroListadoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public class RegistroDetalleDto : RegistroListadoDto
{
    public string? FotoBase64 { get; set; }
    public int UsuarioId { get; set; } 
}


