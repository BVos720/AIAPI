namespace AIAPI.Validation;

// Pure validatielogica voor geüploade afbeeldingen — geen afhankelijkheden,
// zodat deze los (zonder mocks of database) getest kan worden.
public static class ImageValidator
{
    public const long MaxBytes = 15 * 1024 * 1024; // 15 MB

    private static readonly string[] ToegestaneTypes =
        { "image/jpeg", "image/png", "image/heic", "image/heif" };

    private static readonly string[] ToegestaneExtensies =
        { ".jpg", ".jpeg", ".png", ".heic", ".heif" };

    /// <summary>
    /// Controleert of een afbeelding geldig is qua grootte en type.
    /// Geeft (true, null) bij geldig, anders (false, foutmelding).
    /// </summary>
    public static (bool Ok, string? Fout) Valideer(string? contentType, string? fileName, long length)
    {
        if (length <= 0)
            return (false, "Geen afbeelding ontvangen.");

        if (length > MaxBytes)
            return (false, "Afbeelding is te groot. Maximum is 15 MB.");

        var ext = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();

        if (!ToegestaneTypes.Contains((contentType ?? string.Empty).ToLowerInvariant())
            || !ToegestaneExtensies.Contains(ext))
            return (false, "Alleen JPG, PNG en HEIC bestanden zijn toegestaan.");

        return (true, null);
    }
}
