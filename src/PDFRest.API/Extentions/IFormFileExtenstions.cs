using System.IO;
using Microsoft.AspNetCore.Http;

namespace PDFRest.API.Extentions
{
    public static class IFormFileExtenstions
    {
        public static byte[] ToByteArray(this IFormFile formFile)
        {
            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}
