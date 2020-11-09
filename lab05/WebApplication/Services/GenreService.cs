using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class GenreService
    {
        private readonly TvChannelContext db;
        private readonly IMemoryCache cache;
        private const string key = "genres";
        
        public GenreService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            IEnumerable<Genre> genres = null;
            if (!cache.TryGetValue(key, out genres))
            {
                genres = await db.Genres.ToListAsync();
                if (genres != null)
                {
                    cache.Set(key, genres);
                }
            }

            return genres;
        }

        public async Task<bool> AddGenre(Genre genre)
        {
            db.Genres.Add(genre);
            int n = await db.SaveChangesAsync();
            if (n > 0)
            {
                cache.Remove(key);
                return true;
            }

            return false;
        }

        public async Task<Genre> GetGenre(int id)
        {
            Genre genre = null;
            genre = await db.Genres.FirstOrDefaultAsync(g => g.GenreId == id);

            return genre;
        }

        public async Task<Genre> EditGenre(Genre tempGenre)
        {
            Genre genre = null;
            genre = await db.Genres.FirstOrDefaultAsync(g => g.GenreId == tempGenre.GenreId);

            if (genre != null)
            {
                genre.GenreName = tempGenre.GenreName;
                genre.GenreDescription = tempGenre.GenreDescription;

                db.Genres.Update(genre);
                await db.SaveChangesAsync();

                cache.Remove(key);
            }

            return genre;
        }

        public async Task DeleteGenre(int id)
        {
            Genre genre = null;
            genre = await db.Genres.FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre != null)
            {
                db.Genres.Remove(genre);
                await db.SaveChangesAsync();

                cache.Remove(key);
            }
        }
    }
}
