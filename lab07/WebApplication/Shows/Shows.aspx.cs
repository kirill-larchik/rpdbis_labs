using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Data;
using WebApplication.Models;
using System.Data.Entity;

namespace WebApplication.Shows
{
    public partial class Shows : System.Web.UI.Page
    {
        private readonly TvChannelContext _context = new TvChannelContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                GetShows();
        }

        private void GetShows()
        {
            IEnumerable<Show> shows = _context.Shows.Include(s => s.Genre).ToList();
            ShowsGridView.DataSource = shows;
            GenresSqlDataSource.Select(DataSourceSelectArguments.Empty);
            ShowsGridView.DataBind();
        }

        protected void ShowsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ShowsGridView.PageIndex = e.NewPageIndex;
            GetShows();
        }

        protected void AddShowButton_Click(object sender, EventArgs e)
        {
            string name = ShowNameTextBox.Text;
            DateTime releaseDate = ShowReleaseDateTextBox.SelectedDate;
            TimeSpan duration = TimeSpan.TryParse(ShowDurationTextBox.Text, out duration) == true ? duration : default;
            int mark = int.TryParse(ShowMarkTextBox.Text, out mark) == true ? mark : default;
            int markMonth = int.TryParse(ShowMarkMonthTextBox.Text, out markMonth) == true ? markMonth : default;
            int markYear = int.TryParse(ShowMarkYearTextBox.Text, out markYear) == true ? markYear : default;
            int genreId = int.Parse(GenreDropDownList.SelectedValue);
            string description = ShowDescriptionTextBox.Text;

            if (CheckValues(name, releaseDate, duration, mark, markMonth, markYear, description))
            {
                Show show = new Show
                {
                    Name = name,
                    ReleaseDate = releaseDate,
                    Duration = duration,
                    Mark = mark,
                    MarkMonth = markMonth,
                    MarkYear = markYear,
                    GenreId = genreId,
                    Description = description
                };

                _context.Shows.Add(show);
                _context.SaveChanges();

                AddStatusLabel.Text = "Show was successfully added.";

                ShowsGridView.PageIndex = ShowsGridView.PageCount;
                GetShows();
            }
        }

        protected void ShowsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            ShowsGridView.EditIndex = e.NewEditIndex;
            GetShows();
        }

        protected void ShowsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ShowsGridView.EditIndex = -1;
            GetShows();
        }

        protected void ShowsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string name = (string)e.NewValues["Name"];
            DateTime releaseDate = (DateTime)e.NewValues["ReleaseDate"];
            TimeSpan duration = TimeSpan.TryParse((string)e.NewValues["Duration"], out duration) == true ? duration : default;
            int mark = int.TryParse((string)e.NewValues["Mark"], out mark) == true ? mark : default;
            int markMonth = int.TryParse((string)e.NewValues["MarkMonth"], out markMonth) == true ? markMonth : default;
            int markYear = int.TryParse((string)e.NewValues["MarkYear"], out markYear) == true ? markYear : default;
            int genreId = int.Parse((string)e.NewValues["GenreId"]);
            string description = (string)e.NewValues["Description"];

            if (CheckValues(name, releaseDate, duration, mark, markMonth, markYear, description))
            {
                var row = ShowsGridView.Rows[e.RowIndex];
                int id = int.Parse(row.Cells[1].Text);

                Show show = _context.Shows.FirstOrDefault(s => s.ShowId == id);

                show.Name = name;
                show.ReleaseDate = releaseDate;
                show.Duration = duration;
                show.Mark = mark;
                show.MarkMonth = markMonth;
                show.MarkYear = markYear;
                show.GenreId = genreId;
                show.Description = description;

                _context.SaveChanges();

                AddStatusLabel.Text = "Show was successfully updated.";

                ShowsGridView.EditIndex = -1;
                GetShows();
            }
        }

        protected void ShowsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var row = ShowsGridView.Rows[e.RowIndex];
            int id = int.Parse(row.Cells[1].Text);

            Show show = _context.Shows.FirstOrDefault(s => s.ShowId == id);
            _context.Shows.Remove(show);
            _context.SaveChanges();

            AddStatusLabel.Text = "Show was successfully deleted.";
            GetShows();
        }

        public bool CheckValues(string name, DateTime releaseDate, TimeSpan duration, int mark, int markMonth, int markYear, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                AddStatusLabel.Text = "Incorrect 'Name' data.";
                return false;
            }

            if (releaseDate == default)
            {
                AddStatusLabel.Text = "Incorrect 'Release date' data.";
                return false;
            }

            if (duration == default || duration > TimeSpan.FromHours(3))
            {
                AddStatusLabel.Text = "Incorrect 'Duration' data (less 3 hours).";
                return false;
            }

            if (mark < 0 || mark > 10)
            {
                AddStatusLabel.Text = "Incorrect 'Mark' data (1 to 10).";
                return false;
            }

            if (markMonth < 0 || markMonth > 12)
            {
                AddStatusLabel.Text = "Incorrect 'Mark month' data (1 to 12)";
                return false;
            }

            if (markYear < releaseDate.Year)
            {
                AddStatusLabel.Text = "Incorrect 'Mark year' data (more then release date).";
                return false;
            }

            if (string.IsNullOrEmpty(description))
            {
                AddStatusLabel.Text = "Incorrect 'Description' data.";
                return false;
            }

            return true;
        }
    }
}