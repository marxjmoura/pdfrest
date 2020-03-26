using System.IO;
using iText.Kernel.Pdf;

namespace PDFRest.API.Services
{
    public sealed class PdfaColorProfile
    {
        const string ColorProfilePath = "/ColorProfiles/sRGB_CS_profile.icm";

        public PdfOutputIntent ToPdfOutputIntent(string contentPath)
        {
            var identifier = "Custom";
            var condition = string.Empty;
            var registryName = "https://www.color.org";
            var info = "sRGB IEC61966-2.1";
            var colorBytes = File.ReadAllBytes($"{contentPath}/{ColorProfilePath}");
            var colorStream = new MemoryStream(colorBytes);

            return new PdfOutputIntent(identifier, condition, registryName, info, colorStream);
        }
    }
}
