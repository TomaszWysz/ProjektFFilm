using ProjektFFilm.ViewModels;
using System.Collections.Generic;

namespace ProjektFFilm.Models
{
    public class HomeIndexViewModel
    {
       
        public List<Movie> NewestFilm { get; set; }
        public List<News> NewsList { get; set; }
        public List<Movie> TrendList { get; set; }

  
    }
}
