using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface ICachedTimetablesService
    {
        public IEnumerable<Timetable> GetTimetables(int rowCount = 20);
        public void AddTimetables(string cacheKey, int rowCount = 20);
        public IEnumerable<Timetable> GetTimetables(string cacheKey, int rowCount = 20);
    }
}
