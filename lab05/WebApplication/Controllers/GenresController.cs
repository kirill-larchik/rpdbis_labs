using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class GenresController : Controller
    {
        private readonly TvChannelContext _context;

        public GenresController(TvChannelContext context)
        {
            _context = context;
        }

        #region Index
        public IActionResult Index(int page = 1)
        {
            IEnumerable<Genre> genres = _context.Genres.ToList();

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
                Genre = new Genre(),
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] GenresViewModel model)
        {
            if (ModelState.IsValid && CheckUniqueValues(model.Genre))
            {
                // This page for returing to last page of the view.

                await _context.AddAsync(model.Genre);
                await _context.SaveChangesAsync();

                int page = _context.Genres.Count() + 1;
                return RedirectToAction("Index", "Genres", new { page = page });  
            }

            return View(model);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id, int page)
        {
            Genre genre = await _context.Genres.FindAsync(id);

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
            if (ModelState.IsValid && CheckUniqueValues(model.Genre))
            {
                Genre genre = _context.Genres.Find(model.Genre.GenreId);

                genre.GenreName = model.Genre.GenreName;
                genre.GenreDescription = model.Genre.GenreDescription;

                _context.Update(genre);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Genres", new { page = model.CurrentHomePage });
            }

            return View(model);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id, int page)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";

            Genre genre = _context.Genres.Find(id);
            if (_context.Shows.Any(s => s.GenreId == genre.GenreId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            if (genre == null)
            {
                message = "Error. The entity not founded.";
                deleteFlag = false;
            }

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
            Genre genre = _context.Genres.Find(model.Genre.GenreId);

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            model.DeleteViewModel = new DeleteViewModel
            {
                Message = $"The entity was successfully deleted.",
                IsForDelete = false
            };

            model.CurrentHomePage = page;

            return View(model);
        }
        #endregion

        private bool CheckUniqueValues(Genre genre)
        {
            bool firstFlag = true;
            bool secondFlag = true;

            if (_context.Genres.FirstOrDefault(m => m.GenreName == genre.GenreName) != null)
            {
                ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                firstFlag = false;
            }

            if (_context.Genres.FirstOrDefault(m => m.GenreDescription == genre.GenreDescription) != null)
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
