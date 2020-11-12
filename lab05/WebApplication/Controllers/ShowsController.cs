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
    public class ShowsController : Controller
    {
        private readonly TvChannelContext db;

        private readonly CachingService<ShowsViewModel, Show> caching;

        public ShowsController(TvChannelContext context, CachingService<ShowsViewModel, Show> cachingService)
        {
            db = context;
            caching = cachingService;
        }

        public IActionResult Index([FromQuery(Name = "page")] int page = 1)
        {
            ShowsViewModel model = null;
            if (caching.HasEntity(page))
                model = caching.GetEntity(page);
            else
            {
                model = new ShowsViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };

                int count = db.Shows.Count();
                int pageSize = 10;
                model.PageViewModel.SetPages(count, pageSize);

                IQueryable<Show> shows = db.Shows.Include(s => s.Genre).AsQueryable();
                model.Entities = shows.Skip((model.PageViewModel.CurrentPage - 1) * pageSize).Take(pageSize).ToList();

                caching.AddEntity(model);
            }

            return View(model);
        }

        public IActionResult Create(int page)
        {
            ShowsViewModel model = new ShowsViewModel();
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.SelectList = new SelectList(db.Genres.ToList(), "GenreId", "GenreName");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShowsViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                await db.Shows.AddAsync(model.Entity);
                await db.SaveChangesAsync();

                int page = db.Shows.Count();
                caching.Clear(page);

                return RedirectToAction("Index", "Shows", new { page = page });
            }

            model.SelectList = new SelectList(db.Genres.ToList(), "GenreId", "GenreName");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int page)
        {
            Show show = await db.Shows.Include(s => s.Genre).FirstOrDefaultAsync(s => s.ShowId == id);
            if (show != null)
            {
                ShowsViewModel model = new ShowsViewModel();
                model.PageViewModel = new PageViewModel { CurrentPage = page };
                model.Entity = show;
                model.SelectList = new SelectList(db.Genres.ToList(), "GenreId", "GenreName");

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ShowsViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.Entity))
            {
                Show show = await db.Shows.FindAsync(model.Entity.ShowId);
                if (show != null)
                {
                    if (model.Entity.MarkYear > model.Entity.ReleaseDate.Year ||
                        (model.Entity.MarkYear == model.Entity.ReleaseDate.Year && model.Entity.MarkMonth >= model.Entity.ReleaseDate.Month))
                    {
                        show.Name = model.Entity.Name;
                        show.ReleaseDate = model.Entity.ReleaseDate;
                        show.Duration = model.Entity.Duration;
                        show.Mark = model.Entity.Mark;
                        show.MarkMonth = model.Entity.MarkMonth;
                        show.MarkYear = model.Entity.MarkYear;
                        show.GenreId = model.Entity.GenreId;
                        show.Description = model.Entity.Description;

                        db.Shows.Update(show);
                        await db.SaveChangesAsync();
                        caching.Clear(model.PageViewModel.CurrentPage);

                        return RedirectToAction("Index", "Shows", new { page = model.PageViewModel.CurrentPage });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Mark year(month) must be more then release date.");
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            model.SelectList = new SelectList(db.Genres.ToList(), "GenreId", "GenreName");
            return View(model);
        }

        public async Task<IActionResult> Details(int id, int page)
        {
            Show show = await db.Shows.Include(s => s.Genre).FirstOrDefaultAsync(s => s.ShowId == id);
            if (show == null)
                return NotFound();

            ShowsViewModel model = new ShowsViewModel();
            model.Entity = show;
            model.PageViewModel = new PageViewModel { CurrentPage = page };

            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int page)
        {
            Show show = await db.Shows.FindAsync(id);
            if (show == null)
                return NotFound();

            bool deleteFlag = false;
            string message = "Do you want to delete this entity";

            if (db.Timetables.Any(s => s.ShowId == show.ShowId))
                message = "This entity has entities, which dependents from this. Do you want to delete this entity and other, which dependents from this?";

            ShowsViewModel model = new ShowsViewModel();
            model.Entity = show;
            model.PageViewModel = new PageViewModel { CurrentPage = page };
            model.DeleteViewModel = new DeleteViewModel { Message = message, IsDeleted = deleteFlag };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ShowsViewModel model)
        {
            Show show = await db.Shows.FindAsync(model.Entity.ShowId);
            if (show == null)
                return NotFound();

            db.Shows.Remove(show);
            await db.SaveChangesAsync();
            caching.Clear(model.PageViewModel.CurrentPage);

            model.DeleteViewModel = new DeleteViewModel { Message = "The entity was successfully deleted.", IsDeleted = true };

            return View(model);
        }

        private bool CheckUniqueValues(Show show)
        {
            bool firstFlag = true;
            bool secondFlag = true;

            Show tempShow = db.Shows.FirstOrDefault(s => s.Name == show.Name);
            if (tempShow != null)
            {
                if (tempShow.ShowId != show.ShowId)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this name. Please replace this to another.");
                    firstFlag = false;
                }
            }

            tempShow = db.Shows.FirstOrDefault(s => s.Description == show.Description);
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
