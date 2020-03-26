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
using PDFRest.API.Models;
using PDFRest.Tests.Factories;
using Xunit;

namespace PDFRest.Tests.Functional
{
    public sealed class PdfConversionsTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public PdfConversionsTest()
        {
            _server = new TestServer(new Program().CreateWebHostBuilder());
            _client = _server.CreateClient();
        }

        [Theory]
        [InlineData("PDF_A_2B", "2", "B")]
        [InlineData("PDF_A_2U", "2", "U")]
        [InlineData("PDF_A_3B", "3", "B")]
        public async Task ShouldConvertToPdfA2B(string level, string part, string conformance)
        {
            var path = $"/pdfa";
            var fixture = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(fixture);
            var formData = new PdfFormData().WithConformanceLevel(level).Upload(pdfFile);
            var response = await _client.PostAsync(path, formData);
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
            var path = $"/pdfa";
            var title = Guid.NewGuid().ToString("N");
            var fixture = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(fixture);

            var formData = new PdfFormData()
                .WithConformanceLevel("PDF_A_2B")
                .WithTitle(title)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(title, pdfa.GetDocumentInfo().GetTitle());
        }

        [Fact]
        public async Task ShouldReplaceAuthor()
        {
            var path = $"/pdfa";
            var author = Guid.NewGuid().ToString("N");
            var fixture = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(fixture);

            var formData = new PdfFormData()
                .WithConformanceLevel("PDF_A_2B")
                .WithAuthor(author)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(author, pdfa.GetDocumentInfo().GetAuthor());
        }

        [Fact]
        public async Task ShouldReplaceCreationDate()
        {
            var path = $"/pdfa";
            var creationDate = new DateTime(1980, 10, 10);
            var fixture = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(fixture);

            var formData = new PdfFormData()
                .WithConformanceLevel("PDF_A_2B")
                .WithCreationDate(creationDate)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
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
            var path = $"/pdfa";
            var prop1 = Guid.NewGuid().ToString("N");
            var prop2 = Guid.NewGuid().ToString("N");
            var fixture = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pdfFile = await File.ReadAllBytesAsync(fixture);

            var formData = new PdfFormData().WithConformanceLevel("PDF_A_2B")
                .AddCustomProperty(nameof(prop1), prop1)
                .AddCustomProperty(nameof(prop2), prop2)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfADocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(prop1, pdfa.GetDocumentInfo().GetMoreInfo(nameof(prop1)));
            Assert.Equal(prop2, pdfa.GetDocumentInfo().GetMoreInfo(nameof(prop2)));
        }

        [Fact]
        public async Task ShouldRespond400ForRequiredFields()
        {
            var path = $"/pdfa";
            var formData = new PdfFormData().Upload(new byte[0]);
            var response = await _client.PostAsync(path, formData);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<BadRequestError>(responseContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(responseJson.Errors, error => error == "The ConformanceLevel field is required.");
        }

        [Fact]
        public async Task ShouldRespond400ForSizeExceeded()
        {
            var path = $"/pdfa";
            var size10MB = 10485760;
            var pdfFile = new byte[size10MB];
            var formData = new PdfFormData().WithConformanceLevel("PDF_A_2B").Upload(pdfFile);
            var response = await _client.PostAsync(path, formData);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<BadRequestError>(responseContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(responseJson.Errors, error => error == "File must be up to 5242880 bytes.");
        }
    }
}
