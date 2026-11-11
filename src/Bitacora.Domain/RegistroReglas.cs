namespace Bitacora.Domain;

public static class RegistroReglas
{
    public const int TituloMinimo = 3;
    public const int TituloMaximo = 100;
    public const int DescripcionMaxima = 500;
    public const int FotoMaximaBytes = 2 * 1024 * 1024;

    public static IReadOnlyDictionary<string, string[]> Validar(
        string? titulo, string? descripcion, string? fotoBase64)
    {
        var errores = new Dictionary<string, string[]>();
        ValidarTexto(titulo, "Titulo", "El título", TituloMinimo, TituloMaximo, errores);
        ValidarTexto(descripcion, "Descripcion", "La descripción", 1, DescripcionMaxima, errores);

        if (fotoBase64 is not null)
        {
            var errorFoto = ValidarFoto(fotoBase64);
            if (errorFoto is not null)
            {
                errores["FotoBase64"] = new[] { errorFoto };
            }
        }

        return errores;
    }

    private static void ValidarTexto(
        string? valor, string campo, string etiqueta, int minimo, int maximo,
        IDictionary<string, string[]> errores)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            errores[campo] = new[] { $"{etiqueta} es un dato obligatorio." };
            return;
        }

        if (valor.Contains((char)0))
        {
            errores[campo] = new[] { $"{etiqueta} contiene un carácter no permitido." };
            return;
        }

        var cantidad = 0;
        foreach (var caracter in valor.Trim().EnumerateRunes())
        {
            cantidad++;
        }

        if (cantidad < minimo || cantidad > maximo)
        {
            errores[campo] = new[]
            {
                $"{etiqueta} debe tener entre {minimo} y {maximo} caracteres."
            };
        }
    }

    private static string? ValidarFoto(string fotoBase64)
    {
        if (string.IsNullOrWhiteSpace(fotoBase64))
        {
            return "La fotografía no puede estar vacía; usa null si no hay fotografía.";
        }

        var caracteresBase64 = fotoBase64.Count(caracter => !char.IsWhiteSpace(caracter));
        var longitudMaxima = 4 * ((FotoMaximaBytes + 2) / 3);
        if (caracteresBase64 > longitudMaxima)
        {
            return "La fotografía no puede superar 2 MB de contenido decodificado.";
        }

        try
        {
            var bytes = Convert.FromBase64String(fotoBase64);
            if (bytes.Length > FotoMaximaBytes)
            {
                return "La fotografía no puede superar 2 MB de contenido decodificado.";
            }
        }
        catch (FormatException)
        {
            return "FotoBase64 debe contener Base64 válido, sin prefijos.";
        }

        return null;
    }
}
