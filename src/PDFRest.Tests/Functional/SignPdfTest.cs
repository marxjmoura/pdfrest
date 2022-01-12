using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using iText.Signatures;
using Microsoft.AspNetCore.TestHost;
using PDFRest.Tests.Factories;
using Xunit;

namespace PDFRest.Tests.Functional
{
    public class SignPdfTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public SignPdfTest()
        {
            _server = new TestServer(new Program().CreateWebHostBuilder());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ShouldSignPdf()
        {
            var path = $"/signatures";
            var pfxPath = $"{Program.TestProjectPath}/Fixtures/certificate.pfx";
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy.pdf";
            var pfxFile = await File.ReadAllBytesAsync(pfxPath);
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);
            var password = "123456789";

            var formData = new MultipartFormDataContent()
                .AddSignature(pfxFile, password)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdf = new PdfDocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));
            var acroForm = PdfAcroForm.GetAcroForm(pdf, false);
            var acroFormKeys = acroForm.GetFormFields().Keys;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, acroFormKeys.Count);

            Assert.All(acroFormKeys, name =>
            {
                var formField = acroForm.GetField(name);
                var signatureUtil = new SignatureUtil(pdf);
                var signature = signatureUtil.ReadSignatureData(name);
                var integrityAndAuthenticityVerified = signature.VerifySignatureIntegrityAndAuthenticity();

                Assert.NotNull(signature);
                Assert.True(integrityAndAuthenticityVerified);
            });
        }

        [Fact]
        public async Task ShouldSignPdfa()
        {
            var path = $"/signatures";
            var pfxPath = $"{Program.TestProjectPath}/Fixtures/certificate.pfx";
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/dummy-a.pdf";
            var pfxFile = await File.ReadAllBytesAsync(pfxPath);
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);
            var password = "123456789";

            var formData = new MultipartFormDataContent()
                .AddSignature(pfxFile, password)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfDocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));
            var xmpMetadataBytes = pdfa.GetXmpMetadata();
            var xmpMetadata = XMPMetaFactory.ParseFromBuffer(xmpMetadataBytes);
            var conformanceLevel = PdfAConformanceLevel.GetConformanceLevel(xmpMetadata);
            var signatureUtil = new SignatureUtil(pdfa);
            var signatureNames = signatureUtil.GetSignatureNames();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, signatureNames.Count);

            Assert.All(signatureNames, name =>
            {
                var signature = signatureUtil.ReadSignatureData(name);
                var integrityAndAuthenticityVerified = signature.VerifySignatureIntegrityAndAuthenticity();

                Assert.NotNull(signature);
                Assert.True(integrityAndAuthenticityVerified);
            });
        }

        [Fact]
        public async Task ShouldSignPdfAlreadySigned()
        {
            var path = $"/signatures";
            var pfxPath = $"{Program.TestProjectPath}/Fixtures/certificate.pfx";
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/signed-dummy.pdf";
            var pfxFile = await File.ReadAllBytesAsync(pfxPath);
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);
            var password = "123456789";

            var formData = new MultipartFormDataContent()
                .AddSignature(pfxFile, password)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdf = new PdfDocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));
            var signatureUtil = new SignatureUtil(pdf);
            var signatureNames = signatureUtil.GetSignatureNames();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, signatureNames.Count);

            Assert.All(signatureNames, name =>
            {
                var signature = signatureUtil.ReadSignatureData(name);
                var integrityAndAuthenticityVerified = signature.VerifySignatureIntegrityAndAuthenticity();

                Assert.NotNull(signature);
                Assert.True(integrityAndAuthenticityVerified);
            });
        }

        [Fact]
        public async Task ShouldSignPdfaAlreadySigned()
        {
            var path = $"/signatures";
            var pfxPath = $"{Program.TestProjectPath}/Fixtures/certificate.pfx";
            var pdfPath = $"{Program.TestProjectPath}/Fixtures/signed-dummy-a.pdf";
            var pfxFile = await File.ReadAllBytesAsync(pfxPath);
            var pdfFile = await File.ReadAllBytesAsync(pdfPath);
            var password = "123456789";

            var formData = new MultipartFormDataContent()
                .AddSignature(pfxFile, password)
                .Upload(pdfFile);

            var response = await _client.PostAsync(path, formData);
            var stream = await response.Content.ReadAsStreamAsync();
            var pdfa = new PdfDocument(new PdfReader(stream), new PdfWriter(new MemoryStream()));
            var xmpMetadataBytes = pdfa.GetXmpMetadata();
            var xmpMetadata = XMPMetaFactory.ParseFromBuffer(xmpMetadataBytes);
            var conformanceLevel = PdfAConformanceLevel.GetConformanceLevel(xmpMetadata);
            var signatureUtil = new SignatureUtil(pdfa);
            var signatureNames = signatureUtil.GetSignatureNames();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, signatureNames.Count);

            Assert.All(signatureNames, name =>
            {
                var signature = signatureUtil.ReadSignatureData(name);
                var integrityAndAuthenticityVerified = signature.VerifySignatureIntegrityAndAuthenticity();

                Assert.NotNull(signature);
                Assert.True(integrityAndAuthenticityVerified);
            });
        }
    }
}
