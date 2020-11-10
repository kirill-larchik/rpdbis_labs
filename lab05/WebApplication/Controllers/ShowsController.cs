using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Authorize]
    public class ShowsController : Controller
    {
        private readonly TvChannelContext db;

        private readonly GenreService genreService;
        private readonly ShowService showService;
        private readonly TimetableService timetableService;

        public ShowsController(TvChannelContext context, GenreService genreService, ShowService showService, TimetableService timetableService)
        {
            db = context;
            this.genreService = genreService;
            this.showService = showService;
            this.timetableService = timetableService;
        }

        #region Index
        public async Task<IActionResult> Index([FromQuery(Name = "page")] int page = 1)
        {
            IEnumerable<Show> shows = await showService.GetShows();

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
        public async Task<IActionResult> Create(int page)
        {
            IEnumerable<Genre> genres = await genreService.GetGenres();

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
            IEnumerable<Genre> genres = await genreService.GetGenres();
           
            if (ModelState.IsValid & await CheckUniqueValues(model.Show))
            {
                await showService.AddShow(model.Show);

                IEnumerable<Show> shows = await showService.GetShows();
                int page = shows.Count();

                return RedirectToAction("Index", "Shows", new { page = page });
            }

            model.Genres = genres;
            return View(model);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int id, int page)
        {
            Show show = await showService.GetShow(id);

            if (show == null)
                return NotFound();

            IEnumerable<Genre> genres = await genreService.GetGenres();

            ShowsViewModel model = new ShowsViewModel
            {
                Show = show,
                Genres = genres,
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ShowsViewModel model)
        {
            IEnumerable<Genre> genres = await genreService.GetGenres();
            model.Genres = genres;

            if (ModelState.IsValid & await CheckUniqueValues(model.Show))
            {
                // Можно ли по-другому?
                if (model.Show.MarkYear > model.Show.ReleaseDate.Year ||
                    (model.Show.MarkYear == model.Show.ReleaseDate.Year && model.Show.MarkMonth >= model.Show.ReleaseDate.Month))
                {
                    Show show = await showService.EditShow(model.Show);

                    if (show == null)
                        return NotFound();

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
        public async Task<IActionResult> Details(int id, int page)
        {
            Show show = await showService.GetShow(id);

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
        public async Task<IActionResult> Delete(int id, int page)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";

            Show show = await showService.GetShow(id);
            if (show == null)
            {
                message = "Error. The entity not founded.";
                deleteFlag = false;
            }

            //TODO: Add table 'Appeals'
            IEnumerable<Timetable> timetables = await timetableService.GetTimetables();
            if (timetables.Any(t => t.ShowId == show.ShowId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

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
            int id = model.Show.ShowId;
            await showService.DeleteShow(id);

            model.DeleteViewModel = new DeleteViewModel
            {
                Message = $"The entity was successfully deleted.",
                IsForDelete = false
            };

            model.CurrentHomePage = page;

            return View(model);
        }
        #endregion 

        private async Task<bool> CheckUniqueValues(Show show)
        {
            bool firstFlag = true;
            bool secondFlag = true;

            IEnumerable<Show> shows = await showService.GetShows();

            Show tempShow = shows.FirstOrDefault(s => s.Name == show.Name);
            if (tempShow != null)
            {
                if (tempShow.ShowId != show.ShowId)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                    firstFlag = false;
                }
            }

            tempShow = shows.FirstOrDefault(s => s.Description == show.Description);
            if (tempShow != null)
            {
                if (tempShow.ShowId != show.ShowId)
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
