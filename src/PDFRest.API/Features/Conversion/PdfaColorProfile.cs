using System.IO;
using iText.Kernel.Pdf;

namespace PDFRest.API.Features.Conversion
{
    public sealed class PdfaColorProfile
    {
        private readonly string _colorProfileFilePath;

        public PdfaColorProfile(string colorProfileFilePath)
        {
            _colorProfileFilePath = colorProfileFilePath;
        }

        public PdfOutputIntent ToPdfOutputIntent()
        {
            var identifier = "Custom";
            var condition = string.Empty;
            var registryName = "https://www.color.org";
            var info = "sRGB IEC61966-2.1";
            var colorBytes = File.ReadAllBytes(_colorProfileFilePath);
            var colorStream = new MemoryStream(colorBytes);

            return new PdfOutputIntent(identifier, condition, registryName, info, colorStream);
        }
    }
}
