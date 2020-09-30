using System;
using System.Collections.Generic;

namespace DatabaseLibrary.Models
{
    public partial class Timetable
    {
        public int TimetableId { get; set; }
        public int DayOfWeek { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ShowId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? StaffId { get; set; }

        public Show Show { get; set; }

        public override string ToString()
        {
            return $"Дата и время трансляции: {DayOfWeek}-{Month}-{Year} {StartTime} - {EndTime};";
        }
    }
}
