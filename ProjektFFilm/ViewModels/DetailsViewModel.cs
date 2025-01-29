using ProjektFFilm.Models;
using FilmList.Models;
using System.Collections.Generic;

namespace ProjektFFilm.ViewModels
{
    public class DetailsViewModel
    {
        public int MovieId { get; set; }
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public Character Character { get; set; }
        public List<Comment> Comments { get; set; }

        // Dodajemy pole średniej oceny
        public double? AverageRating { get; set; }
    }
}
