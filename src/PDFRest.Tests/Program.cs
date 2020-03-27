using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace PDFRest.Tests
{
    public sealed class Program
    {
        private readonly IConfiguration _configuration;

        public Program()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(TestProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        public static string BuildPath => Path.Combine("bin", "Debug", "netcoreapp3.1");
        public static string TestProjectPath => AppContext.BaseDirectory.Replace(BuildPath, string.Empty);
        public static string ContentPath => TestProjectPath.Replace("Tests", "API");

        public IWebHostBuilder CreateWebHostBuilder() => new WebHostBuilder()
            .UseStartup<Startup>()
            .UseEnvironment("Testing")
            .ConfigureAppConfiguration((builder, config) =>
            {
                builder.HostingEnvironment.ContentRootPath = ContentPath;
                builder.HostingEnvironment.WebRootPath = ContentPath;
                builder.HostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(ContentPath);
                builder.HostingEnvironment.WebRootFileProvider = new PhysicalFileProvider(ContentPath);

                config.AddConfiguration(_configuration);
            });
    }
}
