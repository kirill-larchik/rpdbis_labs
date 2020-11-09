using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class ShowService
    {
        private readonly TvChannelContext db;
        private readonly IMemoryCache cache;
        private const string key = "shows";

        public ShowService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public async Task<IEnumerable<Show>> GetShows()
        {
            IEnumerable<Show> shows = null;
            if (!cache.TryGetValue(key, out shows))
            {
                shows = await db.Shows.Include(s => s.Genre).ToListAsync();
                if (shows != null)
                {
                    cache.Set(key, shows);
                }
            }

            return shows;
        }

        public async Task<bool> AddShow(Show show)
        {
            db.Shows.Add(show);
            int n = await db.SaveChangesAsync();
            if (n > 0)
            {
                cache.Remove(key);
                return true;
            }

            return false;
        }

        public async Task<Show> GetShow(int id)
        {
            Show show = null;
            show = await db.Shows.Include(s => s.Genre).FirstOrDefaultAsync(g => g.ShowId == id);

            return show;
        }

        public async Task<Show> EditShow(Show tempShow)
        {
            Show show = null;
            show = await db.Shows.FirstOrDefaultAsync(s => s.ShowId == tempShow.ShowId);

            if (show != null)
            {
                show.Name = tempShow.Name;
                show.ReleaseDate = tempShow.ReleaseDate;
                show.Duration = tempShow.Duration;
                show.Mark = tempShow.Mark;
                show.MarkMonth = tempShow.MarkMonth;
                show.MarkYear = tempShow.MarkYear;
                show.GenreId = tempShow.GenreId;
                show.Description = tempShow.Description;

                db.Shows.Update(show);
                await db.SaveChangesAsync();

                cache.Remove(key);
            }

            return show;
        }

        public async Task DeleteShow(int id)
        {
            Show show = null;
            show = await db.Shows.FirstOrDefaultAsync(s => s.ShowId == id);

            if (show != null)
            {
                db.Shows.Remove(show);
                await db.SaveChangesAsync();

                cache.Remove(key);
            }
        }
    }
}
