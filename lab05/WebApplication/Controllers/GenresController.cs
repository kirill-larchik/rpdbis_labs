using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;
using WebApplication.ViewModels.Entities;

namespace WebApplication.Controllers
{
    [Authorize]
    public class GenresController : Controller
    {
        private readonly TvChannelContext db;

        private readonly CachingService<GenresViewModel, Genre> caching;

        public GenresController(TvChannelContext context, CachingService<GenresViewModel, Genre> cacheService)
        {
            db = context;
            caching = cacheService;
        }

        public IActionResult Index([FromQuery(Name = "page")] int page = 1)
        {
            GenresViewModel model = null;
            if (caching.HasEntity(page))
                model = caching.GetEntity(page);
            else
            {
                model = new GenresViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };

                int count = db.Genres.Count();
                int pageSize = 10;
                model.PageViewModel.SetPages(count, pageSize);

                IQueryable<Genre> genres = db.Genres.AsQueryable();
                model.Entities = genres.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();

                caching.AddEntity(model);
            }
            
            return View(model);
        }

        public IActionResult Create(int page)
        {
            GenresViewModel model = new GenresViewModel();
            model.PageViewModel = new PageViewModel { CurrentPage = page };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GenresViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                await db.Genres.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                int page = db.Genres.Count();
                caching.Clear(page);

                return RedirectToAction("Index", "Genres", new { page = page });
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Genre genre = await db.Genres.FindAsync(id);
            if (genre != null)
            {
                GenresViewModel model = new GenresViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = genre;

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GenresViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                Genre genre = db.Genres.Find(model.Entity.GenreId);
                if (genre != null)
                {
                    genre.GenreName = model.Entity.GenreName;
                    genre.GenreDescription = model.Entity.GenreDescription;

                    db.Genres.Update(genre);
                    await db.SaveChangesAsync();
                    caching.Clear(model.PageViewModel.CurrentPage);

                    return RedirectToAction("Index", "Genres", new { page = model.PageViewModel.CurrentPage });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            Genre genre = await db.Genres.FindAsync(id);
            if (genre == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Shows.Any(s => s.GenreId == genre.GenreId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            GenresViewModel model = new GenresViewModel();
            model.Entity = genre;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(GenresViewModel model)
        {
            Genre genre = await db.Genres.FindAsync(model.Entity.GenreId);
            if (genre == null)
                return NotFound();

            db.Genres.Remove(genre);
            await db.SaveChangesAsync();
            caching.Clear(model.PageViewModel.CurrentPage);

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }

        private bool CheckUniqueValues(Genre genre)
        {
            bool firstFlag = true;
            bool secondFlag = true;

            Genre tempGenre = db.Genres.FirstOrDefault(g => g.GenreName == genre.GenreName);
            if (tempGenre != null)
            {
                if (tempGenre.GenreId != genre.GenreId)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                    firstFlag = false;
                }
            }

            tempGenre = db.Genres.FirstOrDefault(g => g.GenreDescription == genre.GenreDescription);
            if (tempGenre != null)
            {
                if (tempGenre.GenreId != genre.GenreId)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                    firstFlag = false;
                }
            }

            if (firstFlag && secondFlag)
                return true;
            else
                return false;
        }
    }
}
