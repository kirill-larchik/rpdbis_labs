using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class GenresController : Controller
    {
        private readonly TvChannelContext db;
        private readonly GenreService genreService;
        private readonly ShowService showService;

        public GenresController(TvChannelContext context, GenreService genreService, ShowService showService)
        {
            db = context;
            this.genreService = genreService;
            this.showService = showService;
        }

        #region Index
        public async Task<IActionResult> Index([FromQuery(Name = "page")] int page = 1)
        {
            IEnumerable<Genre> genres = await genreService.GetGenres();

            int pageSize = 10;
            PageViewModel pageViewModel = new PageViewModel(genres.Count(), page, pageSize);
            genres = genres.Skip((pageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();

            GenresViewModel model = new GenresViewModel
            {
                PageViewModel = pageViewModel,
                Genres = genres
            };

            return View(model);
        }
        #endregion

        #region Create
        public IActionResult Create(int page)
        {
            GenresViewModel model = new GenresViewModel
            {
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] GenresViewModel model)
        {
            if (ModelState.IsValid & await CheckUniqueValues(model.Genre))
            {
                await genreService.AddGenre(model.Genre);

                IEnumerable<Genre> genres = await genreService.GetGenres();
                int lastPage = genres.Count();
                 
                return RedirectToAction("Index", "Genres", new { page = lastPage });  
            }

            return View(model);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int id, int page)
        {
            Genre genre = await genreService.GetGenre(id);

            if (genre == null)
                return NotFound();

            GenresViewModel model = new GenresViewModel
            {
                Genre = genre,
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] GenresViewModel model)
        {       
            if (ModelState.IsValid & await CheckUniqueValues(model.Genre))
            {
                Genre tempGenre = new Genre
                {
                    GenreName = model.Genre.GenreName,
                    GenreDescription = model.Genre.GenreDescription
                };

                Genre genre = await genreService.EditGenre(tempGenre);

                if (genre == null)
                    return NotFound();

                return RedirectToAction("Index", "Genres", new { page = model.CurrentHomePage });
            }

            return View(model);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id, int page)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";

            Genre genre = await genreService.GetGenre(id);
            if (genre == null)
            {
                message = "Error. The entity not founded.";
                deleteFlag = false;
            }

            IEnumerable<Show> shows = await showService.GetShows();
            if (shows.Any(s => s.GenreId == genre.GenreId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            GenresViewModel model = new GenresViewModel
            {
                DeleteViewModel = new DeleteViewModel
                {
                    Message = message,
                    IsForDelete = deleteFlag
                },
                
                Genre = new Genre { GenreId = id },
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int page, [FromForm] GenresViewModel model)
        {
            int id = model.Genre.GenreId;
            await genreService.DeleteGenre(id);

            model.DeleteViewModel = new DeleteViewModel
            {
                Message = $"The entity was successfully deleted.",
                IsForDelete = false
            };

            model.CurrentHomePage = page;

            return View(model);
        }
        #endregion

        private async Task<bool> CheckUniqueValues(Genre genre)
        {
            bool firstFlag = true;
            bool secondFlag = true;

            IEnumerable<Genre> genres = await genreService.GetGenres();

            Genre tempGenre = genres.FirstOrDefault(g => g.GenreName == genre.GenreName);
            if (tempGenre != null & tempGenre.GenreId != tempGenre.GenreId)
                if (db.Genres.FirstOrDefault(m => m.GenreName == genre.GenreName) != null)
            {
                ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                firstFlag = false;
            }

            tempGenre = genres.FirstOrDefault(g => g.GenreDescription == genre.GenreDescription);
            if (tempGenre != null & tempGenre.GenreId != tempGenre.GenreId)
            {
                ModelState.AddModelError(string.Empty, "Another entity have this description. Please replace this to another.");
                secondFlag = false;
            }

            if (firstFlag && secondFlag)
                return true;
            else
                return false;
        }
    }
}
