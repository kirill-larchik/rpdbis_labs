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
    public class TimetablesController : Controller
    {
        private readonly TvChannelContext _context;

        public TimetablesController(TvChannelContext context)
        {
            _context = context;
        }

        #region Index
        public IActionResult Index(int page)
        {
            IEnumerable<Timetable> timetables = _context.Timetables.Include(t => t.Show).ToList();

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
        public IActionResult Create(int page)
        {
            IEnumerable<Show> shows = _context.Shows.ToList();

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
            if (ModelState.IsValid)
            {
                model.Timetable.Show = _context.Shows.Find(model.Timetable.ShowId);
                if (model.Timetable.Year > model.Timetable.Show.ReleaseDate.Year ||
                    (model.Timetable.Year == model.Timetable.Show.ReleaseDate.Year && model.Timetable.Month >= model.Timetable.Show.ReleaseDate.Month))
                {
                    model.Timetable.EndTime = model.Timetable.StartTime + model.Timetable.Show.Duration;

                    await _context.Timetables.AddAsync(model.Timetable);
                    await _context.SaveChangesAsync();

                    int page = _context.Timetables.Count();

                    return RedirectToAction("Index", "Timetables", new { page = page });
                }
                else
                {
                    Show show = _context.Shows.Find(model.Timetable.ShowId);
                    ModelState.AddModelError(string.Empty, $"Month and(or) year must be more then release date ({show.ReleaseDate.ToString("d")})");
                }
            }

            model.Shows = _context.Shows.ToList();
            return View(model);
        }
        #endregion

        #region Edit
        public IActionResult Edit(int id, int page)
        {
            Timetable timetable = _context.Timetables.Include(t => t.Show).FirstOrDefault(t => t.TimetableId == id);
            IEnumerable<Show> shows = _context.Shows.ToList();

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
            model.Shows = _context.Shows.ToList();
            model.Timetable.Show = _context.Shows.Find(model.Timetable.ShowId);

            if (model.Timetable.Show == null)
            {
                ModelState.AddModelError(string.Empty, "Please choose a show.");

                model.Timetable.ShowId = model.DefaultShowId;
                model.Timetable.Show = _context.Shows.Find(model.DefaultShowId);
            }

            if (ModelState.IsValid)
            {
                if (model.Timetable.Year > model.Timetable.Show.ReleaseDate.Year ||
                    (model.Timetable.Year == model.Timetable.Show.ReleaseDate.Year && model.Timetable.Month >= model.Timetable.Show.ReleaseDate.Month))
                {
                    Timetable timetable = _context.Timetables.Find(model.Timetable.TimetableId);

                    timetable.DayOfWeek = model.Timetable.DayOfWeek;
                    timetable.Month = model.Timetable.Month;
                    timetable.Year = model.Timetable.Year;
                    timetable.StartTime = model.Timetable.StartTime;
                    timetable.EndTime = model.Timetable.StartTime + model.Timetable.Show.Duration;
                    timetable.Show = model.Timetable.Show;
                    //TODO: Add a staff.

                    _context.Timetables.Update(timetable);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Timetables", new { page = model.CurrentHomePage });
                }
                else
                {
                    Show show = _context.Shows.Find(model.Timetable.ShowId);
                    ModelState.AddModelError(string.Empty, $"Month and(or) year must be more then release date ({show.ReleaseDate.ToString("d")})");
                }
            }

            return View(model);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id, int page)
        {
            bool deleteFlag = true;
            string message = "Do you want to delete this entity";

            Timetable timetable = _context.Timetables.Find(id);

            if (timetable == null)
            {
                message = "Error. The entity not founded.";
                deleteFlag = false;
            }

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
            Timetable timetable = _context.Timetables.Find(model.Timetable.TimetableId);

            _context.Timetables.Remove(timetable);
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
    }
}
