using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public partial class Timetable
    {
        public int TimetableId { get; set; }
        [Required]
        [Range(1, 7)]
        public int DayOfWeek { get; set; }
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int ShowId { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
        //TODO: Remove NULL
        public TimeSpan? EndTime { get; set; }
        public int? StaffId { get; set; }

        public Show Show { get; set; }
    }
}
