using iText.Kernel.Pdf;
using iText.Pdfa;

namespace PDFRest.API.Features.Conversion
{
    public sealed class PdfaPages
    {
        private readonly PdfADocument _pdfa;

        public PdfaPages(PdfADocument pdfa)
        {
            _pdfa = pdfa;
        }

        public void Copy(PdfDocument pdf)
        {
            var numberOfPages = pdf.GetNumberOfPages();

            for (var pageNumber = 1; pageNumber <= numberOfPages; pageNumber++)
            {
                var page = pdf.GetPage(pageNumber);
                var copiedPage = page.CopyTo(_pdfa);

                _pdfa.AddPage(copiedPage);
            }
        }
    }
}
