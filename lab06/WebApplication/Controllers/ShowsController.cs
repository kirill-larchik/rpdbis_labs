using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly TvChannelContext _db;

        public ShowsController(TvChannelContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowViewModel>>> Get()
        {
            return await _db.Shows.Include(s => s.Genre).Select(s => new ShowViewModel
            {
                Id = s.ShowId,
                Name = s.Name,
                ReleaseDate = s.ReleaseDate,
                Duration = s.Duration.ToString(@"hh\:mm\:ss"),
                Mark = s.Mark,
                MarkMonth = s.MarkMonth,
                MarkYear = s.MarkYear,
                GenreId = s.GenreId,
                Genre = s.Genre.GenreName,
                Description = s.Description
            })
                .Take(20).ToListAsync();
        }

        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            return await _db.Genres.OrderBy(g => g.GenreId).ToListAsync();
        }


        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowViewModel>> Get(int id)
        {
            Show show = await _db.Shows.Include(s => s.Genre).FirstOrDefaultAsync(s => s.ShowId == id);
            if (show == null)
                return NotFound();

            return new ObjectResult(new ShowViewModel
            {
                Id = show.ShowId,
                Name = show.Name,
                ReleaseDate = show.ReleaseDate,
                Duration = show.Duration.ToString(@"hh\:mm\:ss"),
                Mark = show.Mark,
                MarkMonth = show.MarkMonth,
                MarkYear = show.MarkYear,
                GenreId = show.GenreId,
                Genre = show.Genre.GenreName,
                Description = show.Description
            });
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<ActionResult<ShowViewModel>> Post(ShowViewModel model)
        {
            if (model == null)
                return BadRequest();

            Show show = new Show
            {
                Name = model.Name,
                ReleaseDate = model.ReleaseDate,
                Duration = TimeSpan.Parse(model.Duration),
                Mark = model.Mark,
                MarkMonth = model.MarkMonth,
                MarkYear = model.MarkYear,
                GenreId = model.GenreId + 1,
                Description = model.Description
            };

            _db.Shows.Add(show);
            await _db.SaveChangesAsync();

            model.Id = _db.Shows.ToList().LastOrDefault().ShowId;
            model.Genre = _db.Genres.FirstOrDefault(g => g.GenreId == model.GenreId + 1).GenreName;
            return Ok(model);
        }

        // PUT api/<ValuesController>/5
        [HttpPut]
        public async Task<ActionResult<ShowViewModel>> Put(ShowViewModel model)
        {
            if (model == null)
                return BadRequest();
            Show show = _db.Shows.FirstOrDefault(s => s.ShowId == model.Id);
            if (show == null)
                return NotFound();

            show.ShowId = model.Id;
            show.Name = model.Name;
            show.ReleaseDate = model.ReleaseDate;
            show.Duration = TimeSpan.Parse(model.Duration);
            show.Mark = model.Mark;
            show.MarkMonth = model.MarkMonth;
            show.MarkYear = model.MarkYear;
            show.GenreId = model.GenreId + 1;
            show.Description = model.Description;

            _db.Update(show);
            await _db.SaveChangesAsync();

            model.Genre = _db.Genres.FirstOrDefault(g => g.GenreId == show.GenreId).GenreName;
            return Ok(model);
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ShowViewModel>> Delete(int id)
        {
            Show show = _db.Shows.Include(s => s.Genre).FirstOrDefault(s => s.ShowId == id);
            if (show == null)
                return NotFound();

            _db.Shows.Remove(show);
            await _db.SaveChangesAsync();

            return Ok(new ShowViewModel
            {
                Id = show.ShowId,
                Name = show.Name,
                ReleaseDate = show.ReleaseDate,
                Duration = show.Duration.ToString(@"hh\:mm\:ss"),
                Mark = show.Mark,
                MarkMonth = show.MarkMonth,
                MarkYear = show.MarkYear,
                GenreId = show.GenreId,
                Genre = show.Genre.GenreName,
                Description = show.Description
            });
        }
    }
}
