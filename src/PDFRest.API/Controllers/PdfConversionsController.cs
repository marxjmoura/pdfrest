using System.IO;
using System.Net.Mime;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Pdfa;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PDFRest.API.Models;
using PDFRest.API.Services;

namespace PDFRest.API.Controllers
{
    [ApiExplorerSettings(GroupName = "PDF conversions")]
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
        public IActionResult CreatePdfa([FromForm] PdfFormData formData)
        {
            if (formData.File.Length > _options.MaxFileSize)
            {
                return new FileSizeExceededError(_options.MaxFileSize);
            }

            using (var stream = formData.File.OpenReadStream())
            {
                var output = new MemoryStream();
                var intent = new PdfaColorProfile().ToPdfOutputIntent(_env.ContentRootPath);
                var conformance = formData.ToPdfAConformanceLevel();
                var reader = new PdfReader(stream);
                var writer = new PdfWriter(output);

                using (var pdf = new PdfDocument(reader))
                using (var pdfa = new PdfADocument(writer, conformance, intent))
                {
                    var pages = new PdfaPages(pdfa);
                    var metadata = new PdfaMetadata(pdfa);

                    pages.Copy(pdf);

                    metadata.CopyCustomProperties(pdf);
                    metadata.AddOrReplaceCustomProperties(formData.CustomPropertiesAsDictionary());
                    metadata.AddOrReplaceTitle(formData.Title);
                    metadata.AddOrReplaceAuthor(formData.Author);
                    metadata.AddOrReplaceCreationDate(formData.CreationDate);

                    new Document(pdfa).Close();

                    return File(output.ToArray(), MediaTypeNames.Application.Pdf);
                }
            }
        }
    }
}
