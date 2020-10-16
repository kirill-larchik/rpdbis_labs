using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class CachedTimetablesService : ICachedTimetables
    {
        private TvChannelContext db;
        private IMemoryCache cache;

        public CachedTimetablesService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void AddTimetables(string cacheKey, int rowCount = 20)
        {
            IEnumerable<Timetable> timetables = null;
            timetables = db.Timetables.Take(rowCount).ToList();

            if (timetables != null)
            {
                cache.Set(cacheKey, timetables, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(262)
                });
            }
        }

        public IEnumerable<Timetable> GetTimetables(int rowCount = 20)
        {
            return db.Timetables.Take(rowCount).ToList();
        }

        public IEnumerable<Timetable> GetTimetables(string cacheKey, int rowCount = 20)
        {
            IEnumerable<Timetable> timetables = null;

            if (!cache.TryGetValue(cacheKey, out timetables))
            {
                timetables = db.Timetables.Take(rowCount).ToList();

                cache.Set(cacheKey, timetables, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(262)
                });
            }

            return timetables;
        }
    }
}
