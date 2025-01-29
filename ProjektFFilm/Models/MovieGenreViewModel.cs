using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace ProjektFFilm.Models;

public class MovieGenreViewModel
{
    public IPagedList<Movie> Movies { get; set; }
    public SelectList? Genres { get; set; }
    public string? MovieGenre { get; set; }
    public string? SearchString { get; set; }
}
