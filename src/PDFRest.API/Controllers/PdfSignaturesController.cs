using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDFRest.API.Extentions;
using PDFRest.API.Features.Signature;
using PDFRest.API.Models;
using PDFRest.API.Models.Errors;

namespace PDFRest.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Sign PDF")]
    public class PdfSignaturesController : Controller
    {
        private readonly PDFRestOptions _options;

        public PdfSignaturesController(IWebHostEnvironment env, IOptions<PDFRestOptions> options)
        {
            _options = options.Value;
        }

        [HttpPost, Route("/signatures")]
        [Consumes("multipart/form-data")]
        [Produces(MediaTypeNames.Application.Pdf)]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(statusCode: 400, type: typeof(BadRequestError))]
        public IActionResult Sign([FromForm] PdfSignatureFormData formData)
        {
            if (formData.File.Length > _options.MaxPdfFileSize)
            {
                return new PdfSizeExceededError(_options.MaxPdfFileSize);
            }

            if (formData.Certificate.Length > _options.MaxCertificateFileSize)
            {
                return new CertificateSizeExceededError(_options.MaxCertificateFileSize);
            }

            var pdfSignature = new PdfSignature();

            pdfSignature.SignDate = formData.SignDate ?? DateTime.Now;
            pdfSignature.Location = formData.Location;
            pdfSignature.Reason = formData.Reason;

            var pdf = formData.File.ToByteArray();
            var certificate = formData.Certificate.ToByteArray();
            var signedPdf = pdfSignature.Sign(pdf, certificate, formData.Password);

            return File(signedPdf, MediaTypeNames.Application.Pdf, $"{formData.File.Name}.pdf");
        }

        [HttpPost, Route("/signatures/validation")]
        [Consumes("multipart/form-data")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(statusCode: 400, type: typeof(BadRequestError))]
        public IActionResult Validate([FromForm] PdfSignatureAuthFormData formData)
        {
            if (formData.File.Length > _options.MaxPdfFileSize)
            {
                return new PdfSizeExceededError(_options.MaxPdfFileSize);
            }

            var pdfSignature = new PdfSignature();
            var pdf = formData.File.ToByteArray();
            var validation = pdfSignature.Validate(pdf);

            return Ok(validation);
        }
    }
}
