using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjektFFilm.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        [Display(Name = "Id filmu")]
        public int MovieId { get; set; }

        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }

        public IdentityUser User { get; set; }

        public Movie Movie { get; set; }

        // Dodajemy pole oceny
        [Display(Name = "Ocena")]
        public int? Rating { get; set; }
    }
}
