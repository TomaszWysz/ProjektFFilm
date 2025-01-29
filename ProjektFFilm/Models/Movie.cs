using FilmList.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektFFilm.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Tytuł")]  
        public string Title { get; set; }

        [Display(Name = "Data wydania"), DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$"), Required, StringLength(50)]
        [Display(Name = "Gatunek")]
        public string Genre { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$"), StringLength(6)]
        [Display(Name = "Kategoria wiekowa")]
        public string Rating { get; set; }

        [Display(Name = "Link do zwiastunu na YouTube")]
        public string YouTubeVideoId { get; set; }

        [Display(Name = "URL obrazu")]
        public string ImageUrl { get; set; }

        [Display(Name = "Obsada")]
        public List<Character> Characters { get; set; }
    }
}
