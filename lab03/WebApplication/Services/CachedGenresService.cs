using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Data;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication.Services
{
    public class CachedGenresService : ICachedGenres
    {
        private TvChannelContext db;
        private IMemoryCache cache;

        public CachedGenresService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void AddGenres(string cacheKey, int rowCount = 20)
        {
            IEnumerable<Genre> genres = null;
            genres = db.Genres.Take(rowCount).ToList();

            if (genres != null)
            {
                cache.Set(cacheKey, genres, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(262)
                });
            }

        }

        public IEnumerable<Genre> GetGenres(int rowCount = 20)
        {
            return db.Genres.Take(rowCount).ToList();
        }

        public IEnumerable<Genre> GetGenres(string cacheKey, int rowsCount = 20)
        {
            IEnumerable<Genre> genres = null;

            if (!cache.TryGetValue(cacheKey, out genres))
            {
                genres = db.Genres.Take(rowsCount).ToList();

                if (genres != null)
                {
                    cache.Set(cacheKey, genres, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(262)
                    });
                }
            }

            return genres;
        }
    }
}
