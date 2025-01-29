using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjektFFilm.Models;
using X.PagedList;
using ProjektFFilm.Data;
using ProjektFFilm.ViewModels;
using FilmList.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ProjektFFilm.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies

        public async Task<IActionResult> Index(string movieGenre, string searchString, int? page)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            var genres = _context.Movie.Select(m => m.Genre).Distinct().ToList();
            var movies = _context.Movie.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }

            // Sort movies alphabetically by title
            movies = movies.OrderBy(m => m.Title);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pagedMovies = movies.ToPagedList(pageNumber, pageSize);

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(genres),
                Movies = pagedMovies
            };

            ViewBag.UserName = username;
            return View(movieGenreVM);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Characters)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Where(c => c.MovieId == id)
                .ToListAsync();

            var averageRating = await _context.Favorites
     .Where(f => f.MovieId == id && f.Rating.HasValue)
     .AverageAsync(f => (double?)f.Rating.Value);


            if (averageRating.HasValue)
            {
                // Obsługa, gdy średnia ocena istnieje
            }
            else
            {
                // Obsługa, gdy średnia ocena nie istnieje
            }

            var viewModel = new DetailsViewModel
            {
                Movie = movie,
                Comments = comments,
                AverageRating = averageRating
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RateMovie(int movieId, int rating)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.MovieId == movieId && f.UserName == username);

            if (favorite != null)
            {
                favorite.Rating = rating;
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Message"] = "Dodaj film do obejrzanych zanim go ocenisz.";
            }

            return RedirectToAction("Details", new { id = movieId });
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int movieId, string content, string name)
        {
            if (string.IsNullOrEmpty(content))
            {
                return RedirectToAction("Details", new { id = movieId });
            }

            var user = User.FindFirstValue(ClaimTypes.Name);
            if (user == null)
            {
                return Challenge();
            }

            var comment = new Comment
            {
                Content = content,
                PostedOn = DateTime.Now,
                Name = user,
                MovieId = movieId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = movieId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound(); // Komentarz nie został znaleziony
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = comment.MovieId });
        }
        // GET: Characters/DetailsCharacter/5
        public async Task<IActionResult> DetailsCharacter(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }


        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Rating,YouTubeVideoId,ImageUrl,Characters")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }


        // GET: Movies/Edit/5
        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Characters)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Rating,YouTubeVideoId,ImageUrl,Characters")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update characters
                    foreach (var character in movie.Characters)
                    {
                        if (character.Id == 0)
                        {
                            // New character
                            _context.Characters.Add(character);
                        }
                        else
                        {
                            // Existing character
                            _context.Characters.Update(character);
                        }
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }


        // GET: Movies/Delete/5
        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Characters) // Include characters related to the movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie
                .Include(m => m.Characters) // Include characters related to the movie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Remove characters related to the movie
            _context.Characters.RemoveRange(movie.Characters);

            // Remove the movie itself
            _context.Movie.Remove(movie);

            // Save changes
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int movieId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.MovieId == movieId && f.UserName == username);

            if (existingFavorite == null)
            {
                var favorite = new Favorite { MovieId = movieId, UserName = username };
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Message"] = "Ten film jest już w twoich obejrzanych.";
            }

            return RedirectToAction("Details", new { id = movieId });
        }



        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int movieId)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.MovieId == movieId && f.UserName == username);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = movieId });
        }


        // Action to view user's favorite
        public async Task<IActionResult> Favorites()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var favorites = await _context.Favorites
                .Where(f => f.UserName == username)
                .Include(f => f.Movie)
                .ToListAsync();
            return View(favorites);
        }


        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost]
        public async Task<IActionResult> ReportComment(int commentId, string reportReason)
        {

            if (_context == null)
            {
                throw new NullReferenceException("_context is null");
            }
            var username = User.FindFirstValue(ClaimTypes.Name);
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                // Handle case where comment with commentId is not found
                return NotFound();
            }
            var report = new CommentReport
            {
                CommentId = comment.Id,
                ReportReason = reportReason,
                ReportedOn = DateTime.UtcNow,
                ReportedBy = User.Identity.Name  // Assuming you have authentication and user is logged in
            };

            _context.CommentReports.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ListCommentReports()
        {
            var reports = _context.CommentReports
                                  .Include(cr => cr.Comment) // Include related comment if needed
                                  .ToList(); // Retrieve all comment reports

            return View(reports); // Pass reports to the view for display
        }

        public async Task<IActionResult> AdminReports()
        {
            var reports = await _context.CommentReports
                .Include(cr => cr.Comment)
                .ThenInclude(c => c.Movie)
                .ToListAsync();

            return View(reports);
        }

        [HttpPost]

        public async Task<IActionResult> DeleteReport(int reportId)
        {
            var report = await _context.CommentReports.FindAsync(reportId);
            if (report != null)
            {
                _context.CommentReports.Remove(report);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AdminReports");
        }
        public IActionResult DeleteCommentA(int commentId)
        {
            var comment = _context.Comments.Find(commentId);

            if (comment == null)
            {
                return NotFound(); // Handle case where comment with given ID is not found
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirect to appropriate action after deletion
        }

    }
}
