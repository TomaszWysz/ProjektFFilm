using ProjektFFilm.Data;
using ProjektFFilm.Models;
using ProjektFFilm.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ProjektFFilm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            
                // Pobierz 3 najnowsze filmy
                var newestFilm = _context.Movie.OrderByDescending(m => m.ReleaseDate).Take(3).ToList();

                // Pobierz 3 trenduj¹ce filmy wed³ug œredniej oceny
                var top5Film = _context.Favorites
                .GroupBy(f => f.MovieId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.First().Movie) // Mo¿esz wybraæ pierwszy element z grupy, zak³adaj¹c, ¿e dla ka¿dego MovieId obiekt Movie jest ten sam
                .ToList();

      

            var newsList = _context.News.OrderByDescending(n => n.DatePosted).Take(3).ToList();
           
            // Przeka¿ dane do widoku
            var viewModel = new HomeIndexViewModel
                {
                    NewestFilm = newestFilm,
                    NewsList = newsList,
                    TrendList = top5Film,
                };

                return View(viewModel);
            }
            
    [HttpPost]
        public IActionResult AddNews(string newsTitle, string newsContent)
        {
            var news = new News
            {
                Title = newsTitle,
                Content = newsContent,

                DatePosted = DateTime.Now
            };

            _context.News.Add(news);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNews(int newsId)
        {
            var news = await _context.News.FindAsync(newsId);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


    }

}
