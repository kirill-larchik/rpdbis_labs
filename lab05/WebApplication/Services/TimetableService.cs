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
    public class TimetableService
    {
        private readonly TvChannelContext db;
        private readonly IMemoryCache cache;
        private const string key = "timetables";

        public TimetableService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public async Task<IEnumerable<Timetable>> GetTimetables()
        {
            IEnumerable<Timetable> timetables = null;
            if (!cache.TryGetValue(key, out timetables))
            {
                timetables = await db.Timetables.Include(t => t.Show).ToListAsync();
                if (timetables != null)
                {
                    cache.Set(key, timetables);
                }
            }

            return timetables;
        }

        public async Task<bool> AddTimetable(Timetable timetable)
        {
            db.Timetables.Add(timetable);
            int n = await db.SaveChangesAsync();
            if (n > 0)
            {
                cache.Remove(key);
                return true;
            }

            return false;
        }

        public async Task<Timetable> GetTimetable(int id)
        {
            Timetable timetable = null;
            timetable = await db.Timetables.Include(t => t.Show).FirstOrDefaultAsync(t => t.TimetableId == id);

            return timetable;
        }

        public async Task<Timetable> EditTimetable(Timetable tempTimetable)
        {
            Timetable timetable = null;
            timetable = await db.Timetables.FirstOrDefaultAsync(t => t.TimetableId == tempTimetable.TimetableId);

            if (timetable != null)
            {
                timetable.DayOfWeek = tempTimetable.DayOfWeek;
                timetable.Month = tempTimetable.Month;
                timetable.Year = tempTimetable.Year;
                timetable.ShowId = tempTimetable.ShowId;
                timetable.StartTime = tempTimetable.StartTime;
                timetable.EndTime = tempTimetable.EndTime;
                //TODO: Staff

                db.Timetables.Update(timetable);
                await db.SaveChangesAsync();

                cache.Remove(key);
            }

            return timetable;
        }

        public async Task DeleteTimetable(int id)
        {
            Timetable timetable = null;
            timetable = await db.Timetables.FirstOrDefaultAsync(t => t.TimetableId == id);

            if (timetable != null)
            {
                db.Timetables.Remove(timetable);
                await db.SaveChangesAsync();

                cache.Remove(key);
            }
        }
    }
}
