using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace PDFRest.Tests.Functional
{
    public sealed class PingTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public PingTest()
        {
            _server = new TestServer(new Program().CreateWebHostBuilder());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ShouldRespondWelcomeMessage()
        {
            var path = "/";
            var response = await _client.GetAsync(path);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("PDF Rest - Create, edit and convert PDF files.", responseContent);
        }
    }
}
