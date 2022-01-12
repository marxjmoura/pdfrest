using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PDFRest.API.Models
{
    public class PdfSignatureFormData
    {
        [Display(Name = "file"), Required]
        public IFormFile File { get; set; }

        [Display(Name = "certificate"), Required]
        public IFormFile Certificate { get; set; }

        [Required, MaxLength(255), DataType(DataType.Password)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Location { get; set; }

        [MaxLength(80)]
        public string Reason { get; set; }

        public DateTime? SignDate { get; set; }
    }
}
