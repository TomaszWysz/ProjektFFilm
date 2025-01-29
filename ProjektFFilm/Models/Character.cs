using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektFFilm.Models
{
    public class Character
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Imię i nazwisko")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Szczegóły")]
        public string Description { get; set; }

        [Display(Name = "Obraz")]
        public string AvatarUrl { get; set; }
    }
}
