using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using iText.Pdfa;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PDFRest.API.Models.Errors;
using PDFRest.Tests.Factories;
using Xunit;

namespace PDFRest.Tests.Functional
{
    public sealed class ConvertPdfTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ConvertPdfTest()
        {
            _server = new TestServer(new Program().CreateWebHostBuilder());
            _client = _server.CreateClient();
        }

        [Theory]
        [InlineData("PDF_A_2B", "2", "B")]
        [InlineData("PDF_A_2U", "2", "U")]
        [InlineData("PDF_A_3B", "3", "B")]
        public async Task ShouldConvertToPdfa(string level, string part, string conformance)
        {
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);

            var formData = new MultipartFormDataContent()
                .WithConformanceLevel(level)
                .Upload(pdfFile);

            var response = await _client.PostAsync("/pdfa", formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));
            var xmpMetadataBytes = pdfa.GetXmpMetadata();
            var xmpMetadata = XMPMetaFactory.ParseFromBuffer(xmpMetadataBytes);
            var conformanceLevel = PdfAConformanceLevel.GetConformanceLevel(xmpMetadata);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(part, conformanceLevel.GetPart());
            Assert.Equal(conformance, conformanceLevel.GetConformance());
        }

        [Fact]
        public async Task ShouldReplaceTitle()
        {
            var title = Guid.NewGuid().ToString("N");
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);

            var formData = new MultipartFormDataContent()
                .WithConformanceLevel("PDF_A_2B")
                .ReplaceTitle(title)
                .Upload(pdfFile);

            var response = await _client.PostAsync("/pdfa", formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(title, pdfa.GetDocumentInfo().GetTitle());
        }

        [Fact]
        public async Task ShouldReplaceAuthor()
        {
            var author = Guid.NewGuid().ToString("N");
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);

            var formData = new MultipartFormDataContent()
                .WithConformanceLevel("PDF_A_2B")
                .ReplaceAuthor(author)
                .Upload(pdfFile);

            var response = await _client.PostAsync("/pdfa", formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(author, pdfa.GetDocumentInfo().GetAuthor());
        }

        [Fact]
        public async Task ShouldReplaceCreationDate()
        {
            var creationDate = new DateTime(1980, 10, 10);
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);

            var formData = new MultipartFormDataContent()
                .WithConformanceLevel("PDF_A_2B")
                .ReplaceCreationDate(creationDate)
                .Upload(pdfFile);

            var response = await _client.PostAsync("/pdfa", formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));
            var creationDateProperty = PdfName.CreationDate.GetValue();
            var pdfaCreationDate = pdfa.GetDocumentInfo().GetMoreInfo(creationDateProperty);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(creationDate, Convert.ToDateTime(pdfaCreationDate));
        }

        [Fact]
        public async Task ShouldAddCustomProperties()
        {
            var property1 = Guid.NewGuid().ToString("N");
            var property2 = Guid.NewGuid().ToString("N");
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);

            var formData = new MultipartFormDataContent().WithConformanceLevel("PDF_A_2B")
                .AddCustomProperty(index: 0, nameof(property1), property1)
                .AddCustomProperty(index: 1, nameof(property2), property2)
                .Upload(pdfFile);

            var response = await _client.PostAsync("/pdfa", formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(property1, pdfa.GetDocumentInfo().GetMoreInfo(nameof(property1)));
            Assert.Equal(property2, pdfa.GetDocumentInfo().GetMoreInfo(nameof(property2)));
        }

        [Fact]
        public async Task ShouldRespond400ForRequiredFields()
        {
            var formData = new MultipartFormDataContent().Upload(new byte[0]);
            var response = await _client.PostAsync("/pdfa", formData);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<BadRequestError>(responseContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(responseJson.Errors, error => error == "The ConformanceLevel field is required.");
        }

        [Fact]
        public async Task ShouldRespond400ForSizeExceeded()
        {
            var size11MB = 11534336;
            var pdfFile = new byte[size11MB];

            var formData = new MultipartFormDataContent()
                .WithConformanceLevel("PDF_A_2B")
                .Upload(pdfFile);

            var response = await _client.PostAsync("/pdfa", formData);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<BadRequestError>(responseContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(responseJson.Errors, error => error == "Document must be up to 10485760 bytes.");
        }
    }
}
