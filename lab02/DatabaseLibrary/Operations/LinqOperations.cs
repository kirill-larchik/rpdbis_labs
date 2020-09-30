using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseLibrary.Data;
using Microsoft.EntityFrameworkCore;
using DatabaseLibrary.Models;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DatabaseLibrary.Operations
{
    public static class LinqOperations
    {
        /// <summary>
        /// Task01: Выборка всех данных из таблицы, стоящей в схеме базы данных на стороне отношения «один».
        /// </summary>
        public static string SelectGenres(TvChannelContext db)
        {
            StringBuilder builder = new StringBuilder();

            var query = from g in db.Genres
                        orderby g.GenreId
                        select new
                        {
                            Название_жанра = g.GenreName,
                            Описание = g.GenreDescription
                        };

            foreach (var entity in query.Take(5))
                builder.Append(entity.ToString() + ";\n");

            builder.Append("\n");

            return builder.ToString();
        }

        /// <summary>
        /// ("Task02: выборка данных из таблицы, стоящей в схеме базы данных нас стороне отношения «один», 
        /// отфильтрованные по определенному условию, налагающему ограничения на одно или несколько полей.
        /// </summary>
        public static string SelectGenresByFilter(TvChannelContext db)
        {
            StringBuilder builder = new StringBuilder();

            var query = from g in db.Genres
                        where g.GenreName.First() == 't' // && g.GenreDescription.First() == 'b' 
                        orderby g.GenreId
                        select new
                        {
                            Название_жанра = g.GenreName,
                            Описание = g.GenreDescription
                        };

            foreach (var entity in query.Take(5))
                builder.Append(entity.ToString() + ";\n");

            builder.Append("\n");

            return builder.ToString();
        }

        /// <summary>
        /// Task03: выборка данных, сгруппированных по любому из полей данных с выводом какого - либо итогового результата
        /// (min, max, avg, сount или др.) по выбранному полю из таблицы, стоящей в схеме базы данных нас стороне отношения «многие».
        /// </summary>
        public static string SelectTimetablesByGroup(TvChannelContext db)
        {
            StringBuilder builder = new StringBuilder();

            var query = from s in db.Shows
                        join t in db.Timetables
                        on s.ShowId equals t.ShowId
                        group t by s.Name into g
                        select new
                        {
                            Телепередача = g.Key,
                            Кол_ство_трансляций = g.Count() 
                        };

            foreach (var entity in query.Take(5))
                builder.Append(entity.ToString() + ";\n");

            builder.Append("\n");

            return builder.ToString();
        }

        /// <summary>
        /// Task04: выборка данных из двух полей двух таблиц, связанных между собой отношением «один-ко-многим».
        /// </summary>
        public static string SelectShowsAndGenres(TvChannelContext db)
        {
            StringBuilder builder = new StringBuilder();

            var query = from g in db.Genres
                        join s in db.Shows
                        on g.GenreId equals s.GenreId
                        select new
                        {
                            Фильм = s.Name,
                            Жанр = g.GenreName
                        };

            foreach (var entity in query.Take(5))
                builder.Append(entity.ToString() + ";\n");

            builder.Append("\n");

            return builder.ToString();
        }

        /// <summary>
        /// Task05: выборка данных из двух таблиц, связанных между собой отношением «один-ко-многим» и отфильтрованным по некоторому
        /// условию, налагающему ограничения на значения одного или нескольких полей.
        /// </summary>
        public static string SelectShowsAndGenresByFilter(TvChannelContext db)
        {
            StringBuilder builder = new StringBuilder();
            
            DateTime date = new DateTime(2010, 9, 23);

            var query = from g in db.Genres
                        join s in db.Shows
                        on g.GenreId equals s.GenreId
                        where s.ReleaseDate > date
                        select new
                        {
                            Фильм = s.Name,
                            Жанр = g.GenreName
                        };

            foreach (var entity in query.Take(5))
                builder.Append(entity.ToString() + ";\n");

            builder.Append("\n");

            return builder.ToString();
        }

        /// <summary>
        /// Task06: вставка данных в таблицы, стоящей на стороне отношения «Один».
        /// </summary>
        public static string InsertGenre(string genreName, string genreDescription, TvChannelContext db)
        {
            Genre genre = new Genre
            {
                GenreName = genreName,
                GenreDescription = genreDescription
            };

            db.Genres.Add(genre);
            db.SaveChanges();

            return db.Genres.LastOrDefault().ToString();
        }

        /// <summary>
        /// Task07: вставка данных в таблицы, стоящей на стороне отношения «Многие».
        /// </summary>
        public static string InsertShows(string name, DateTime releaseDate, TimeSpan duration, int mark, 
            int markMonth, int markYear, int genreId, string description, TvChannelContext db)
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

            db.Shows.Add(show);
            db.SaveChanges();

            return SelectShows(db);
        }

        private static string SelectShows(TvChannelContext db)
        {
            var query = from s in db.Shows
                        join g in db.Genres
                        on s.GenreId equals g.GenreId
                        orderby s.ShowId
                        select new
                        {
                            Название_передачи = s.Name,
                            Дата_выхода = s.ReleaseDate,
                            Продолжительность = s.Duration,
                            Рейтинг = s.Mark,
                            Месяц_рейтинга = s.MarkMonth,
                            Год_рейтингка = s.MarkYear,
                            Жанр = g.GenreName,
                            Описание = s.Description
                        };

            return query.LastOrDefault().ToString();
        }

        /// <summary>
        /// Task08: удаление данных из таблицы, стоящей на стороне отношения «Один».
        /// </summary>
        public static string DeleteGenre(int genreId, TvChannelContext db)
        {
            EntityEntry<Genre> entity = db.Genres.Remove(db.Genres.Find(genreId));
            db.SaveChanges();

            return "\nУдаленная запись: " + entity.Entity.ToString() + "\n\n";
        }

        /// <summary>
        /// Task09: удаление данных из таблицы, стоящей на стороне отношения «Многие».
        /// </summary>
        public static string DeleteShow(int showId, TvChannelContext db)
        {
            EntityEntry<Show> entity = db.Shows.Remove(db.Shows.Find(showId));
            db.SaveChanges();

            return "\nУдаленная запись: " + entity.Entity.ToString() + "\n\n";
        }

        /// <summary>
        /// Task10: обновление удовлетворяющих определенному условию записей в любой из таблиц базы данных.
        /// </summary>
        public static string UpdateGenre(int genreId, string genreName, string genreDescription, TvChannelContext db)
        {
            Genre genre = db.Genres.Find(genreId);

            genre.GenreName = genreName;
            genre.GenreDescription = genreDescription;

            db.Genres.Update(genre);
            db.SaveChanges();

            return db.Genres.Find(genreId).ToString();
        }
    }
}
