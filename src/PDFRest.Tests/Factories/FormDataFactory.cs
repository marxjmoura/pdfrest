using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using PDFRest.API.Models;

namespace PDFRest.Tests.Factories
{
    public static class FormDataFactory
    {
        public static MultipartFormDataContent WithConformanceLevel(this MultipartFormDataContent formData,
            string conformanceLevel)
        {
            formData.Add(new StringContent(conformanceLevel), nameof(PdfConversionFormData.ConformanceLevel));

            return formData;
        }

        public static MultipartFormDataContent ReplaceTitle(this MultipartFormDataContent formData, string title)
        {
            formData.Add(new StringContent(title), nameof(PdfConversionFormData.Title));

            return formData;
        }

        public static MultipartFormDataContent ReplaceAuthor(this MultipartFormDataContent formData, string author)
        {
            formData.Add(new StringContent(author), nameof(PdfConversionFormData.Author));

            return formData;
        }

        public static MultipartFormDataContent ReplaceCreationDate(this MultipartFormDataContent formData,
            DateTime creationDate)
        {
            var dateTimeIso8601 = creationDate.ToString("o");
            formData.Add(new StringContent(dateTimeIso8601), nameof(PdfConversionFormData.CreationDate));

            return formData;
        }

        public static MultipartFormDataContent AddCustomProperty(this MultipartFormDataContent formData,
            int index, string name, string value)
        {
            formData.Add(new StringContent(name), $"{nameof(PdfConversionFormData.CustomProperties)}[{index}][Name]");
            formData.Add(new StringContent(value), $"{nameof(PdfConversionFormData.CustomProperties)}[{index}][Value]");

            return formData;
        }
        public static MultipartFormDataContent AddSignature(this MultipartFormDataContent formData,
            byte[] pfx, string password)
        {
            var pfxContent = new ByteArrayContent(pfx);
            pfxContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);

            formData.Add(pfxContent, nameof(PdfSignatureFormData.Certificate), "certificate.pfx");
            formData.Add(new StringContent(password), "password");

            return formData;
        }

        public static MultipartFormDataContent AddSignatureLocation(this MultipartFormDataContent formData,
            string location)
        {
            formData.Add(new StringContent(location), nameof(PdfSignatureFormData.Location));

            return formData;
        }

        public static MultipartFormDataContent AddSignatureReason(this MultipartFormDataContent formData,
            string reason)
        {
            formData.Add(new StringContent(reason), nameof(PdfSignatureFormData.Reason));

            return formData;
        }

        public static MultipartFormDataContent AddSignatureDate(this MultipartFormDataContent formData,
            DateTime? signDate)
        {
            var dateTimeIso8601 = signDate.Value.ToString("o");
            formData.Add(new StringContent(dateTimeIso8601), nameof(PdfSignatureFormData.Reason));

            return formData;
        }

        public static MultipartFormDataContent Upload(this MultipartFormDataContent formData, byte[] pdf)
        {
            var pdfContent = new ByteArrayContent(pdf);
            pdfContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Pdf);

            formData.Add(pdfContent, "file", "dummy.pdf");

            return formData;
        }
    }
}
