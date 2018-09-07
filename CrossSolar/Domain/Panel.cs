using System.ComponentModel.DataAnnotations;

namespace CrossSolar.Domain
{
    public class Panel
    {
        public int Id { get; set; }

        [Required] public string Latitude { get; set; }

        public string Longitude { get; set; }

        [Required] public string Serial { get; set; }

        public string Brand { get; set; }
    }
}