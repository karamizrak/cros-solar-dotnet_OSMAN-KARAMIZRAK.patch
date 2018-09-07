using System.ComponentModel.DataAnnotations;

namespace CrossSolar.Models
{
    public class PanelModel
    {
        public int Id { get; set; }

        [Required]
        
        [RegularExpression(@"^-?([1-8]?[1-9]|[1-9]0)\.{1}\d{1,6}$")]
        public string Latitude { get; set; }

        [RegularExpression(@"^-?([1]?[1-7][1-9]|[1]?[1-8][0]|[1-9]?[0-9])\.{1}\d{1,6}")]
        
        public string Longitude { get; set; }

        [Required]
        [MinLength(16,ErrorMessage = "Serial Number is Min and Max characters length 16")]
        [MaxLength(16, ErrorMessage = "Serial Number is Min and Max characters length 16")]
        public string Serial { get; set; }

        public string Brand { get; set; }
    }
}