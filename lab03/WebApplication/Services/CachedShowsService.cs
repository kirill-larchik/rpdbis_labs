using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class CachedShowsService : ICachedShows
    {
        private TvChannelContext db;
        private IMemoryCache cache;

        public CachedShowsService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void AddShows(string cacheKey, int rowCount = 20)
        {
            IEnumerable<Show> shows = null;
            shows = db.Shows.Take(rowCount).ToList();

            if (shows != null)
            {
                cache.Set(cacheKey, shows, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(262)
                });
            }
        }

        public IEnumerable<Show> GetShows(int rowsCount = 20)
        {
            return db.Shows.Take(rowsCount).ToList();
        }

        public IEnumerable<Show> GetShows(string cacheKey, int rowCount = 20)
        {
            IEnumerable<Show> shows = null;

            if (!cache.TryGetValue(cacheKey, out shows))
            {
                shows = db.Shows.Take(rowCount).ToList();

                if (shows != null)
                {
                    cache.Set(cacheKey, shows, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(262)
                    });
                }
            }

            return shows;
        }
    }
}
