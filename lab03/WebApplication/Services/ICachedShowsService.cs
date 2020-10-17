using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface ICachedShowsService
    {
        public IEnumerable<Show> GetShows(int rowCount = 20);
        public void AddShows(string cacheKey, int rowCount = 20);
        public IEnumerable<Show> GetShows(string cacheKey, int rowCount = 20);
    }
}
