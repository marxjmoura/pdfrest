using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Pdfa;

namespace PDFRest.API.Services
{
    public sealed class PdfaMetadata
    {
        private readonly PdfADocument _pdfa;

        public PdfaMetadata(PdfADocument pdfa)
        {
            _pdfa = pdfa;
        }

        public void AddOrReplaceTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return;

            _pdfa.GetDocumentInfo().SetMoreInfo(PdfName.Title.GetValue(), title);
        }

        public void AddOrReplaceAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author)) return;

            _pdfa.GetDocumentInfo().SetMoreInfo(PdfName.Author.GetValue(), author);
        }

        public void AddOrReplaceCustomProperties(IDictionary<string, string> customProperties)
        {
            foreach (var property in customProperties)
            {
                _pdfa.GetDocumentInfo().SetMoreInfo(property.Key, property.Value);
            }
        }

        public void AddOrReplaceCreationDate(DateTime? creationDate)
        {
            if (creationDate == null) return;

            _pdfa.GetDocumentInfo().SetMoreInfo(PdfName.CreationDate.GetValue(), creationDate.ToString());
        }

        public void CopyCustomProperties(PdfDocument pdf)
        {
            var pdfCustomProperties = pdf.GetTrailer().GetAsDictionary(PdfName.Info);

            foreach (var customProperty in pdfCustomProperties.EntrySet())
            {
                var propertyName = customProperty.Key.GetValue();
                var propertyValue = customProperty.Value.ToString();

                _pdfa.GetDocumentInfo().SetMoreInfo(propertyName, propertyValue);
            }
        }
    }
}
