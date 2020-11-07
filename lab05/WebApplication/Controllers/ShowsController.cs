using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class ShowsController : Controller
    {
        private readonly TvChannelContext _context;

        public ShowsController(TvChannelContext context)
        {
            _context = context;
        }

        #region Index
        public IActionResult Index(int page)
        {
            IEnumerable<Show> shows = _context.Shows.ToList();

            int pageSize = 10;

            PageViewModel pageViewModel = new PageViewModel(shows.Count(), page, pageSize);
            shows = shows.Skip((pageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();

            ShowsViewModel model = new ShowsViewModel
            {
                Shows = shows,
                PageViewModel = pageViewModel
            };

            return View(model);
        }
        #endregion

        #region Create
        public IActionResult Create(int page)
        {
            IEnumerable<Genre> genres = _context.Genres.ToList();

            ShowsViewModel model = new ShowsViewModel
            {
                Genres = genres,
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ShowsViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Show))
            { 
                model.Show.Genre = _context.Genres.Find(model.Show.GenreId);

                await _context.Shows.AddAsync(model.Show);
                await _context.SaveChangesAsync();

                int page = _context.Shows.Count();

                return RedirectToAction("Index", "Shows", new { page = page });
            }

            model.Genres = _context.Genres.ToList();
            return View(model);
        }
        #endregion

        #region Edit
        public IActionResult Edit(int id, int page)
        {
            Show show = _context.Shows.Include(s => s.Genre).FirstOrDefault(s => s.ShowId == id);
            IEnumerable<Genre> genres = _context.Genres.ToList();

            ShowsViewModel model = new ShowsViewModel
            {
                Show = show,
                Genres = genres,
                CurrentHomePage = page
            };

            if (show == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ShowsViewModel model)
        {
            model.Genres = _context.Genres.ToList();
            model.Show.Genre = _context.Genres.Find(model.Show.GenreId);

            if (model.Show.Genre == null)
            {
                ModelState.AddModelError(string.Empty, "Please choose a genre.");
                
                model.Show.GenreId = model.DefaultGerneId;
                model.Show.Genre = _context.Genres.Find(model.DefaultGerneId);
            }

            if (ModelState.IsValid && CheckUniqueValues(model.Show))
            {
                if (model.Show.MarkYear > model.Show.ReleaseDate.Year ||
                    (model.Show.MarkYear == model.Show.ReleaseDate.Year && model.Show.MarkMonth >= model.Show.ReleaseDate.Month))
                {
                    Show show = _context.Shows.Find(model.Show.ShowId);

                    show.Name = model.Show.Name;
                    show.ReleaseDate = model.Show.ReleaseDate;
                    show.Duration = model.Show.Duration;
                    show.Mark = model.Show.Mark;
                    show.MarkMonth = model.Show.MarkMonth;
                    show.MarkYear = model.Show.MarkYear;
                    show.GenreId = model.Show.GenreId;
                    show.Genre = model.Show.Genre;
                    show.Description = model.Show.Description;

                    _context.Shows.Update(show);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Shows", new { page = model.CurrentHomePage });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Mark year(month) must be more then release date.");
                }
            }

            return View(model);
        }
        #endregion

        #region Details
        public IActionResult Details(int id, int page)
        {
            Show show = _context.Shows.Include(s => s.Genre).FirstOrDefault(s => s.ShowId == id);

            if (show == null)
                return NotFound();

            ShowsViewModel model = new ShowsViewModel
            {
                Show = show,
                CurrentHomePage = page
            };

            return View(model);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id, int page)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";
            
            Show show = _context.Shows.Find(id);
            //TODO: Add table 'Appeals'
            if (_context.Timetables.Any(t => t.ShowId == show.ShowId))
            {
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";
            }

            if (show == null)
            {
                message = "Error. The entity not founded.";
                deleteFlag = false;
            }

            ShowsViewModel model = new ShowsViewModel
            {
                DeleteViewModel = new DeleteViewModel
                {
                    Message = message,
                    IsForDelete = deleteFlag
                },
                
                Show = new Show { ShowId = id },
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int page, [FromForm] ShowsViewModel model)
        {
            Show show = _context.Shows.Find(model.Show.ShowId);

            _context.Shows.Remove(show);
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

        private bool CheckUniqueValues(Show show)
        {
            bool firstFlag = true;
            bool secondFlag = true;

            if (_context.Shows.FirstOrDefault(s => s.Name == show.Name) != null)
            {
                ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                firstFlag = false;
            }

            if (_context.Shows.FirstOrDefault(s => s.Description == show.Description) != null)
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
