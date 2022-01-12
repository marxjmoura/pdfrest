using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDFRest.API.Features.Conversion;
using PDFRest.API.Models;
using PDFRest.API.Models.Errors;

namespace PDFRest.API.Controllers
{
    [ApiExplorerSettings(GroupName = "PDF to PDF/A")]
    public sealed class PdfConversionsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly PDFRestOptions _options;

        public PdfConversionsController(IWebHostEnvironment env, IOptions<PDFRestOptions> options)
        {
            _env = env;
            _options = options.Value;
        }

        [HttpPost, Route("/pdfa")]
        [Consumes("multipart/form-data")]
        [Produces(MediaTypeNames.Application.Pdf)]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(statusCode: 400, type: typeof(BadRequestError))]
        public IActionResult CreatePdfa([FromForm] PdfConversionFormData formData)
        {
            if (formData.File.Length > _options.MaxPdfFileSize)
            {
                return new PdfSizeExceededError(_options.MaxPdfFileSize);
            }

            using (var pdf = formData.File.OpenReadStream())
            {
                var colorProfile = new PdfaColorProfile($"{_env.ContentRootPath}/ColorProfiles/sRGB_CS_profile.icm");
                var pdfaConversion = new PdfaConversion(colorProfile);
                var conformance = formData.GetPdfAConformanceLevel();

                pdfaConversion.Title = formData.Title;
                pdfaConversion.Author = formData.Author;
                pdfaConversion.CreationDate = formData.CreationDate;
                pdfaConversion.CustomProperties = formData.CustomPropertiesAsDictionary();

                var pdfa = pdfaConversion.Convert(pdf, conformance);

                return File(pdfa, MediaTypeNames.Application.Pdf, $"{formData.File.Name}.pdf");
            }
        }
    }
}
