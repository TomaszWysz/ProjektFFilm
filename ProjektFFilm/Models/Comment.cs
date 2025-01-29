using ProjektFFilm.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FilmList.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Treść")]
        public string Content { get; set; }

        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Display(Name = "Data dodania")]
        public DateTime PostedOn { get; set; }

        [Display(Name = "Id filmu")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
