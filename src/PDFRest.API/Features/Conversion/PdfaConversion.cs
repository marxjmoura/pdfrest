using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Pdfa;

namespace PDFRest.API.Features.Conversion
{
    public class PdfaConversion
    {
        private readonly PdfaColorProfile _colorProfile;

        public PdfaConversion(PdfaColorProfile colorProfile)
        {
            _colorProfile = colorProfile;
        }

        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime? CreationDate { get; set; }

        public IDictionary<string, string> CustomProperties { get; set; }

        public byte[] Convert(Stream pdf, PdfAConformanceLevel conformance)
        {
            var output = new MemoryStream();
            var reader = new PdfReader(pdf);
            var writer = new PdfWriter(output);

            var intent = _colorProfile.ToPdfOutputIntent();

            using (var pdfDocument = new PdfDocument(reader))
            using (var pdfADocument = new PdfADocument(writer, conformance, intent))
            {
                var pdfaPages = new PdfaPages(pdfADocument);
                var pdfaMetadata = new PdfaMetadata(pdfADocument);

                pdfaPages.Copy(pdfDocument);

                pdfaMetadata.CopyCustomProperties(pdfDocument);
                pdfaMetadata.AddOrReplaceCustomProperties(CustomProperties);
                pdfaMetadata.AddOrReplaceTitle(Title);
                pdfaMetadata.AddOrReplaceAuthor(Author);
                pdfaMetadata.AddOrReplaceCreationDate(CreationDate);

                var document = new Document(pdfADocument);
                document.Close();

                return output.ToArray();
            }
        }
    }
}
