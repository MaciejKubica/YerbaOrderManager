using System.ComponentModel.DataAnnotations;

namespace App.ViewModels
{
    public class YerbaViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public int Gentian { get; set; }

        public int Size { get; set; }

        public string Mark { get; set; }

        public decimal Cost { get; set; }

        public string Country { get; set; }

        public string Components { get; set; }

        public string Producent { get; set; }

        public string Description { get; set; }
    }
}
