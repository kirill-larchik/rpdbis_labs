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
    public class TimetablesController : Controller
    {
        private readonly TvChannelContext db;
        private readonly TimetableService timetableService;
        private readonly ShowService showService;

        public TimetablesController(TvChannelContext context, TimetableService timetableService, ShowService showService)
        {
            db = context;
            this.timetableService = timetableService;
            this.showService = showService;
        }

        #region Index
        public async Task<IActionResult> Index([FromQuery(Name = "page")] int page = 1)
        {
            IEnumerable<Timetable> timetables = await timetableService.GetTimetables();

            int pageSize = 10;

            PageViewModel pageViewModel = new PageViewModel(timetables.Count(), page, pageSize);
            timetables = timetables.Skip((pageViewModel.PageNumber - 1) * pageSize).Take(pageSize).ToList();

            TimetablesViewModel model = new TimetablesViewModel
            {
                Timetables = timetables,
                PageViewModel = pageViewModel
            };

            return View(model);
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create(int page)
        {
            IEnumerable<Show> shows = await showService.GetShows();

            TimetablesViewModel model = new TimetablesViewModel
            {
                Shows = shows,
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] TimetablesViewModel model)
        {
            model.Shows = await showService.GetShows();
            Show show = await showService.GetShow(model.Timetable.ShowId);

            if (ModelState.IsValid)
            {
                // Можно ли по-другому?
                if (model.Timetable.Year > show.ReleaseDate.Year ||
                    (model.Timetable.Year == show.ReleaseDate.Year && model.Timetable.Month >= show.ReleaseDate.Month))
                {
                    model.Timetable.EndTime = model.Timetable.StartTime + show.Duration;

                    await timetableService.AddTimetable(model.Timetable);

                    IEnumerable<Timetable> timetables = await timetableService.GetTimetables();
                    int page = timetables.Count();

                    return RedirectToAction("Index", "Timetables", new { page = page });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Month and(or) year must be more then release date ({show.ReleaseDate.ToString("d")})");
                }
            }

           
            return View(model);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int id, int page)
        {
            Timetable timetable = await timetableService.GetTimetable(id);
            IEnumerable<Show> shows = await showService.GetShows();

            TimetablesViewModel model = new TimetablesViewModel
            {
                Timetable = timetable,
                Shows = shows,
                CurrentHomePage = page
            };

            if (timetable == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] TimetablesViewModel model)
        {
            model.Shows = await showService.GetShows();
            Show show = await showService.GetShow(model.Timetable.ShowId);

            if (ModelState.IsValid)
            {
                // Можно ли по-другому?
                if (model.Timetable.Year > show.ReleaseDate.Year ||
                   (model.Timetable.Year == show.ReleaseDate.Year && model.Timetable.Month >= show.ReleaseDate.Month))
                {
                    model.Timetable.EndTime = model.Timetable.StartTime + show.Duration;

                    Timetable timetable = await timetableService.EditTimetable(model.Timetable);

                    if (timetable == null)
                        return NotFound();

                    return RedirectToAction("Index", "Timetables", new { page = model.CurrentHomePage });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Month and(or) year must be more then release date ({show.ReleaseDate.ToString("d")})");
                }
            }

            return View(model);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id, int page)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";

            Timetable timetable = await timetableService.GetTimetable(id);
            if (timetable == null)
            {
                message = "Error. The entity not founded.";
                deleteFlag = false;
            }

            //TODO: Add table "Staff"
            TimetablesViewModel model = new TimetablesViewModel
            {
                DeleteViewModel = new DeleteViewModel
                {
                    Message = message,
                    IsForDelete = deleteFlag
                },

                Timetable = new Timetable { TimetableId = id },
                CurrentHomePage = page
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int page, [FromForm] TimetablesViewModel model)
        {
            int id = model.Timetable.TimetableId;
            await timetableService.DeleteTimetable(id);

            model.DeleteViewModel = new DeleteViewModel
            {
                Message = $"The entity was successfully deleted.",
                IsForDelete = false
            };

            model.CurrentHomePage = page;

            return View(model);
        }
        #endregion 
    }
}
