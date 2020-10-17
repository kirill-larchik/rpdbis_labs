using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface ICachedGenresService
    {
        public IEnumerable<Genre> GetGenres(int rowCount = 20);
        public void AddGenres(string cacheKey, int rowCount = 20);
        public IEnumerable<Genre> GetGenres(string cacheKey, int rowCount = 20);
    }
}
