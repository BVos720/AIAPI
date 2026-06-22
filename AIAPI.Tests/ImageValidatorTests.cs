using AIAPI.Validation;

namespace AIAPI.Tests;

public class ImageValidatorTests
{
    [Theory]
    [InlineData("image/jpeg", "foto.jpg")]
    [InlineData("image/jpeg", "foto.jpeg")]
    [InlineData("image/png", "foto.png")]
    [InlineData("image/heic", "foto.heic")]
    [InlineData("image/heif", "foto.heif")]
    public void GeldigeAfbeelding_WordtGeaccepteerd(string contentType, string naam)
    {
        var (ok, fout) = ImageValidator.Valideer(contentType, naam, 1024);

        Assert.True(ok);
        Assert.Null(fout);
    }

    [Theory]
    [InlineData("application/pdf", "document.pdf")]
    [InlineData("text/plain", "tekst.txt")]
    [InlineData("image/gif", "animatie.gif")]
    [InlineData("image/jpeg", "verkeerd.exe")]   // juiste content-type, foute extensie
    public void VerkeerdType_WordtGeweigerd(string contentType, string naam)
    {
        var (ok, fout) = ImageValidator.Valideer(contentType, naam, 1024);

        Assert.False(ok);
        Assert.Contains("JPG, PNG en HEIC", fout);
    }

    [Fact]
    public void TeGroot_WordtGeweigerd()
    {
        var (ok, fout) = ImageValidator.Valideer("image/jpeg", "groot.jpg", ImageValidator.MaxBytes + 1);

        Assert.False(ok);
        Assert.Contains("te groot", fout);
    }

    [Fact]
    public void PreciesOpDeGrens_WordtGeaccepteerd()
    {
        var (ok, _) = ImageValidator.Valideer("image/jpeg", "rand.jpg", ImageValidator.MaxBytes);

        Assert.True(ok);
    }

    [Fact]
    public void LeegBestand_WordtGeweigerd()
    {
        var (ok, fout) = ImageValidator.Valideer("image/jpeg", "leeg.jpg", 0);

        Assert.False(ok);
        Assert.Contains("Geen afbeelding", fout);
    }
}
