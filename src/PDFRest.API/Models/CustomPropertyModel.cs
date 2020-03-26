using System.ComponentModel.DataAnnotations;

namespace PDFRest.API.Models
{
    public sealed class CustomPropertyModel
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Value { get; set; }
    }
}
