using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PDFRest.API.Models
{
    public class PdfSignatureAuthFormData
    {
        [Display(Name = "file"), Required]
        public IFormFile File { get; set; }
    }
}
