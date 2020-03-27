using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Http;

namespace PDFRest.API.Models
{
    public sealed class PdfFormData
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

        public PdfAConformanceLevel ToPdfAConformanceLevel()
        {
            switch (ConformanceLevel)
            {
                case "PDF_A_2B": return PdfAConformanceLevel.PDF_A_2B;
                case "PDF_A_2U": return PdfAConformanceLevel.PDF_A_2U;
                case "PDF_A_3B": return PdfAConformanceLevel.PDF_A_3B;
            }

            return null;
        }

        public IDictionary<string, string> CustomPropertiesAsDictionary()
        {
            var dictionary = new Dictionary<string, string>();

            if (CustomProperties != null)
            {
                foreach (var property in CustomProperties)
                {
                    dictionary.Add(property.Name, property.Value);
                }
            }

            return dictionary;
        }
    }
}
