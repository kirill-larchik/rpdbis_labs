using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;
using WebApplication.ViewModels.Entities;

namespace WebApplication.Controllers
{
    [Authorize]
    public class TimetablesController : Controller
    {
        private readonly TvChannelContext db;
        private readonly CachingService<TimetablesViewModel, Timetable> caching;

        public TimetablesController(TvChannelContext context, CachingService<TimetablesViewModel, Timetable> cachingService)
        {
            db = context;
            caching = cachingService;
        }

        public IActionResult Index([FromQuery(Name = "page")] int page = 1)
        {
            TimetablesViewModel model = null;
            if (caching.HasEntity(page))
                model = caching.GetEntity(page);
            else
            {
                model = new TimetablesViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };

                int count = db.Timetables.Count();
                int pageSize = 10;
                model.PageViewModel.SetPages(count, pageSize);

                IQueryable<Timetable> timetables = db.Timetables.Include(t => t.Show).AsQueryable();
                model.Entities = timetables.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();

                caching.AddEntity(model);
            }

            return View(model);
        }

        public ActionResult Create(int page)
        {
            TimetablesViewModel model = new TimetablesViewModel();
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.SelectList = new SelectList(db.Shows.ToList(), "ShowId", "Name");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TimetablesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Show show = await db.Shows.FindAsync(model.Entity.ShowId);
                if (model.Entity.Year > show.ReleaseDate.Year ||
                        (model.Entity.Year == show.ReleaseDate.Year && model.Entity.Month >= show.ReleaseDate.Month))
                {
                    model.Entity.EndTime = model.Entity.StartTime + show.Duration;

                    await db.Timetables.AddAsync(model.Entity);
                    await db.SaveChangesAsync();

                    int page = db.Timetables.Count();
                    caching.Clear(page);

                    return RedirectToAction("Index", "Timetables", new { page = page });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Mark year(month) must be more then release date ({show.ReleaseDate.ToString("d")})");
                }
            }

            model.SelectList = new SelectList(db.Genres.ToList(), "GenreId", "GenreName");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Timetable timetable = await db.Timetables.Include(t => t.Show).FirstOrDefaultAsync(t => t.TimetableId == id);
            if (timetable != null)
            {
                TimetablesViewModel model = new TimetablesViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = timetable;
                model.SelectList = new SelectList(db.Shows.ToList(), "ShowId", "Name");

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TimetablesViewModel model)
        {
            if (ModelState.IsValid)
            {
                Show show = await db.Shows.FindAsync(model.Entity.ShowId);
                Timetable timetable = await db.Timetables.FindAsync(model.Entity.TimetableId);
                if (timetable != null)
                {
                    if (model.Entity.Year > show.ReleaseDate.Year ||
                        (model.Entity.Year == show.ReleaseDate.Year && model.Entity.Month >= show.ReleaseDate.Month))
                    {
                        timetable.DayOfWeek = model.Entity.DayOfWeek;
                        timetable.Month = model.Entity.Month;
                        timetable.Year = model.Entity.Year;
                        timetable.ShowId = model.Entity.ShowId;
                        timetable.StartTime = model.Entity.StartTime;
                        timetable.EndTime = timetable.StartTime + show.Duration;

                        db.Timetables.Update(timetable);
                        await db.SaveChangesAsync();
                        caching.Clear(model.PageViewModel.CurrentPage);

                        return RedirectToAction("Index", "Timetables", new { page = model.PageViewModel.CurrentPage });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Mark year(month) must be more then release date ({show.ReleaseDate.ToString("d")})");
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            model.SelectList = new SelectList(db.Shows.ToList(), "ShowId", "Name");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            Timetable timetable = await db.Timetables.FindAsync(id);
            if (timetable == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Timetables.Any(t => t.TimetableId == timetable.TimetableId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            TimetablesViewModel model = new TimetablesViewModel();
            model.Entity = timetable;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TimetablesViewModel model)
        {
            Timetable timetable = await db.Timetables.FindAsync(model.Entity.TimetableId);
            if (timetable == null)
                return NotFound();

            db.Timetables.Remove(timetable);
            await db.SaveChangesAsync();
            caching.Clear(model.PageViewModel.CurrentPage);

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }
    }
}
