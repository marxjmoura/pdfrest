using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Http;

namespace PDFRest.API.Models
{
    public sealed class PdfConversionFormData
    {
        [Display(Name = "file"), Required]
        public IFormFile File { get; set; }

        [Required, RegularExpression("^PDF_A_2B|PDF_A_2U|PDF_A_3B$")]
        public string ConformanceLevel { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Author { get; set; }

        public DateTime? CreationDate { get; set; }

        public ICollection<CustomPropertyModel> CustomProperties { get; set; }

        public PdfAConformanceLevel GetPdfAConformanceLevel()
        {
            return ConformanceLevel switch
            {
                "PDF_A_2B" => PdfAConformanceLevel.PDF_A_2B,
                "PDF_A_2U" => PdfAConformanceLevel.PDF_A_2U,
                "PDF_A_3B" => PdfAConformanceLevel.PDF_A_3B,
                _ => null
            };
        }

        public IDictionary<string, string> CustomPropertiesAsDictionary()
        {
            return (CustomProperties ?? new List<CustomPropertyModel>())
                .ToDictionary(property => property.Name, property => property.Value);
        }
    }
}
