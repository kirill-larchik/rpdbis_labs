using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Data;
using WebApplication.Models;
using System.Data.Entity;

namespace WebApplication.Timetables
{
    public partial class Timetables : System.Web.UI.Page
    {
        private readonly TvChannelContext _context = new TvChannelContext();
        private const string startTimeLimit = "20:00:00";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                GetTimetables();
        }

        private void GetTimetables()
        {
            IEnumerable<Timetable> timetables = _context.Timetables.Include(t => t.Show).OrderBy(t => t.TimetableId).ToList();
            TimetablesGridView.DataSource = timetables;
            TimetablesGridView.DataBind();
        }

        protected void TimetablesGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            TimetablesGridView.PageIndex = e.NewPageIndex;
            GetTimetables();
        }

        protected void AddTimetableButton_Click(object sender, EventArgs e)
        {
            int dayOfWeek = int.TryParse(TimetableDayOfWeekTextBox.Text, out dayOfWeek) == true ? dayOfWeek : default;
            int month = int.TryParse(TimetableMonthTextBox.Text, out month) == true ? month : default;
            int year = int.TryParse(TimetableYearTextBox.Text, out year) == true ? year : default;
            int showId = int.Parse(ShowsDropDownList.SelectedValue);
            TimeSpan startTime = TimeSpan.TryParse(TimetableStartTimeTextBox.Text, out startTime) == true ? startTime : default;

            if (CheckValues(dayOfWeek, month, year, startTime))
            {
                Timetable timetable = new Timetable
                {
                    DayOfWeek = dayOfWeek,
                    Month = month,
                    Year = year,
                    ShowId = showId,
                    StartTime = startTime,
                    EndTime = startTime + _context.Shows.FirstOrDefault(s => s.ShowId == showId).Duration
                };

                _context.Timetables.Add(timetable);
                _context.SaveChanges();

                AddStatusLabel.Text = "Timetable was successfully added.";

                TimetablesGridView.PageIndex = TimetablesGridView.PageCount;
                GetTimetables();
            }
        }
        protected void TimetablesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            TimetablesGridView.EditIndex = e.NewEditIndex;
            GetTimetables();
        }

        protected void TimetablesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            TimetablesGridView.EditIndex = -1;
            GetTimetables();
        }
        protected void TimetablesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int dayOfWeek = int.TryParse((string)e.NewValues["DayOfWeek"], out dayOfWeek) == true ? dayOfWeek : default;
            int month = int.TryParse((string)e.NewValues["Month"], out month) == true ? month : default;
            int year = int.TryParse((string)e.NewValues["Year"], out year) == true ? year : default;
            int showId = int.Parse((string)e.NewValues["ShowId"]);
            TimeSpan startTime = TimeSpan.TryParse((string)e.NewValues["StartTime"], out startTime) == true ? startTime : default;

            if (CheckValues(dayOfWeek, month, year, startTime))
            {
                var row = TimetablesGridView.Rows[e.RowIndex];
                int id = int.Parse(row.Cells[1].Text);

                Timetable timetable = _context.Timetables.FirstOrDefault(t => t.TimetableId == id);

                timetable.DayOfWeek = dayOfWeek;
                timetable.Month = month;
                timetable.Year = year;
                timetable.ShowId = showId;
                timetable.StartTime = startTime;
                timetable.EndTime = startTime + _context.Shows.FirstOrDefault(s => s.ShowId == showId).Duration;

                _context.SaveChanges();

                AddStatusLabel.Text = "Timetable was successfully updated.";

                TimetablesGridView.EditIndex = -1;
                GetTimetables();
            }
        }

        protected void TimetablesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var row = TimetablesGridView.Rows[e.RowIndex];
            int id = int.Parse(row.Cells[1].Text);

            Timetable timetable = _context.Timetables.FirstOrDefault(t => t.TimetableId == id);
            _context.Timetables.Remove(timetable);
            _context.SaveChanges();

            AddStatusLabel.Text = "Timetable was successfully deleted.";
            GetTimetables();
        }

        public bool CheckValues(int dayOfWeek, int month, int year, TimeSpan startTime)
        {
            if (dayOfWeek == default)
            {
                AddStatusLabel.Text = "Incorrect 'Day Of Week' data.";
                return false;
            }

            if (month == default)
            {
                AddStatusLabel.Text = "Incorrect 'Month' data.";
                return false;
            }

            if (year == default)
            {
                AddStatusLabel.Text = "Incorrect 'Year'.";
                return false;
            }

            if (startTime == default || startTime > TimeSpan.Parse(startTimeLimit))
            {
                AddStatusLabel.Text = "Incorrect 'Start Time' data (less '20:00:00').";
                return false;
            }

            return true;
        }
    }
}