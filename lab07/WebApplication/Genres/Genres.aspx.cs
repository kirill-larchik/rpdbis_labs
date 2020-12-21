using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Genres
{
    public partial class Genres : System.Web.UI.Page
    {
        private readonly TvChannelContext _context = new TvChannelContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                GetGenres();
        }

        private void GetGenres()
        {
            IEnumerable<Genre> genres = _context.Genres.ToList();
            GenresGridView.DataSource = genres;
            GenresGridView.DataBind();
        }

        protected void GenresGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GenresGridView.PageIndex = e.NewPageIndex;
            GetGenres();
        }

        protected void AddGenreButton_Click(object sender, EventArgs e)
        {
            string name = GenreNameTextBox.Text;
            string description = GenreDescriptionTextBox.Text;

            if (CheckValues(name, description))
            {
                Genre genre = new Genre
                {
                    GenreName = name,
                    GenreDescription = description
                };

                _context.Genres.Add(genre);
                _context.SaveChanges();

                GenreNameTextBox.Text = string.Empty;
                GenreDescriptionTextBox.Text = string.Empty;

                AddStatusLabel.Text = "Genre was successfully added.";

                GenresGridView.PageIndex = GenresGridView.PageCount;
                GetGenres();
            }
        }

        protected void GenresGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GenresGridView.EditIndex = e.NewEditIndex;
            GetGenres();
        }

        protected void GenresGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GenresGridView.EditIndex = -1;
            GetGenres();
        }

        protected void GenresGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string name = (string)e.NewValues["GenreName"];
            string description = (string)e.NewValues["GenreDescription"];

            if (CheckValues(name, description))
            {
                var row = GenresGridView.Rows[e.RowIndex];
                int id = int.Parse(row.Cells[1].Text);

                Genre genre = _context.Genres.FirstOrDefault(g => g.GenreId == id);

                genre.GenreName = name;
                genre.GenreDescription = description;

                _context.SaveChanges();

                AddStatusLabel.Text = "Genre was successfully updated.";

                GenresGridView.EditIndex = -1;
                GetGenres();
            }
        }

        protected void GenresGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var row = GenresGridView.Rows[e.RowIndex];
            int id = int.Parse(row.Cells[1].Text);

            Genre genre = _context.Genres.FirstOrDefault(g => g.GenreId == id);
            _context.Genres.Remove(genre);
            _context.SaveChanges();

            AddStatusLabel.Text = "Genre was successfully deleted.";

            GetGenres();
        }

        public bool CheckValues(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                AddStatusLabel.Text = "Incorrect 'Name' data.";
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