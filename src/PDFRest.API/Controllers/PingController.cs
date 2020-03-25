using Microsoft.AspNetCore.Mvc;

namespace PDFRest.API.Controllers
{
    public sealed class PingController : Controller
    {
        [HttpGet, Route("/")]
        public IActionResult Ping()
        {
            return Ok("PDF Rest - Create, edit and convert PDF files.");
        }
    }
}
