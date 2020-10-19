using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using WebApplication.Data;
using WebApplication.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApplication.Models;
using WebApplication.Infrastructure;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("SqlServer");

            services.AddDbContext<TvChannelContext>(options => options.UseSqlServer(connectionString));

            services.AddMemoryCache();

            services.AddScoped<ICachedGenresService ,CachedGenresService>();
            services.AddScoped<ICachedShowsService, CachedShowsService>();
            services.AddScoped<ICachedTimetablesService, CachedTimetablesService>();

            services.AddDistributedMemoryCache();
            services.AddSession();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TvChannelContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            app.Map("/info", Info);
            app.Map("/genres", Genres);
            app.Map("/shows", Shows);
            app.Map("/timetables", Timetables);

            app.Map("/searchform", SearchForm);

            app.Run(async (context) =>
            {
                ICachedGenresService cachedGenresService = context.RequestServices.GetService<ICachedGenresService>();
                cachedGenresService.GetGenres("genres20");

                ICachedShowsService cachedShowsService = context.RequestServices.GetService<ICachedShowsService>();
                cachedShowsService.GetShows("shows20");

                ICachedTimetablesService cachedTimetablesService = context.RequestServices.GetService<ICachedTimetablesService>();
                cachedTimetablesService.GetTimetables("timetables20");

                User user = context.Session.Get<User>("user") ?? new User();

                string htmlString = "<html>" +
                "<head>" +
                "<title>Форма пользователя</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>" +
                "<form action='/'>" +
                "<div>Введите логин:</div>";
                htmlString += $"<div><input type='text' name='loginStr' value=" + user.Login + "></div>";
                htmlString += "<div>Введите пароль:</div>";
                htmlString += $"<div><input type='text' name='passwordStr' value=" + user.Password + "></div>" +
                "<div><input type='submit' value='Enter/Update'></div>" +
                "</form>" +
                "<div><a href='/genres'>Table 'Genres'</a></div>" +
                "<div><a href='/shows'>Table 'Show'</a></div>" +
                "<div><a href='/timetables'>Table 'Timetables'</a></div>" +
                "<div><a href='/searchform'>Search Form</a></div>" +
                "</div>" +
                "</body>" +
                "</html>";

                string Login = context.Request.Query["loginStr"];
                string Password = context.Request.Query["passwordStr"];

                if(Login != null && Password != null)
                {
                    user.Login = Login;
                    user.Password = Password;
                    context.Session.Set<User>("user", user);
                }

                await context.Response.WriteAsync(htmlString);
            });
        }

        private static void Info(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                string httpString = "<html>" +
                "<head>" +
                "<title>Информация о клиенте</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body align='middle'>" +
                "<div> Сервер: " + context.Request.Host + "</div>" +
                "<div> Путь: " + context.Request.PathBase + "</div>" +
                "<div> Протокол: " + context.Request.Protocol + "</div>" +
                "<div><a href='/'>Главная</a></div>" +
                "</body>" +
                "</html>";

                await context.Response.WriteAsync(httpString);
            });
        }

        private static void Genres(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                ICachedGenresService cachedGenresService = context.RequestServices.GetService<ICachedGenresService>();
                IEnumerable<Genre> genres = cachedGenresService.GetGenres("genres20");

                string httpString = "<html>" +
                "<head>" +
                "<title>Таблица Genres</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>Таблица 'Genres'</div>" +
                "<div align='center'>" +
                "<table border=1>" +
                "<tr>" +
                "<td>Жанр</td>" +
                "<td>Описание жанра</td>" +
                "</tr>";

               foreach (Genre genre in genres)
                {
                    httpString += "<tr>";
                    httpString += $"<td>{genre.GenreName}</td>";
                    httpString += $"<td>{genre.GenreDescription}</td>";
                    httpString += "</tr>";
                }
                httpString += "</table>";
               
                httpString += "<div align='center'><a href='/'>Главная</a></div>";
                httpString += "</body>" +
                "</html>";

                await context.Response.WriteAsync(httpString);
            });
        }

        private static void Shows(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                ICachedShowsService cachedShowsService = context.RequestServices.GetService<ICachedShowsService>();
                IEnumerable<Show> shows = cachedShowsService.GetShows("shows20");

                string httpString = "<html>" +
                "<head>" +
                "<title>Таблица Shows</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>Таблица 'Shows'</div>" +
                "<div align='center'>" +
                "<table border=1>" +
                "<tr>" +
                "<td>Название</td>" +
                "<td>Описание жанра</td>" +
                "<td>Дата выхода</td>" +
                "<td>Продолжительность</td>" +
                "<td>Рейтинг</td>" +
                "<td>Рейтинг на месяц</td>" +
                "<td>Рейтинг на год</td>" +
                "<td>Жанр</td>" + 
                "<td>Описание</td>" +
                "</tr>";

                foreach (Show show in shows)
                {
                    httpString += "<tr>";
                    httpString += $"<td>{show.Name}</td>";
                    httpString += $"<td>{show.Description}</td>";
                    httpString += $"<td>{show.ReleaseDate.ToString("d")}</td>";
                    httpString += $"<td>{show.Duration}</td>";
                    httpString += $"<td>{show.Mark}</td>";
                    httpString += $"<td>{show.MarkMonth}</td>";
                    httpString += $"<td>{show.MarkYear}</td>";
                    httpString += $"<td>{show.Genre?.GenreName}</td>";
                    httpString += $"<td>{show.Description}</td>";
                    httpString += "</tr>";
                }
                httpString += "</table>";

                httpString += "<div align='center'><a href='/'>Главная</a></div>";
                httpString += "</body>" +
                "</html>";

                await context.Response.WriteAsync(httpString);
            });
        }

        private static void Timetables(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                ICachedTimetablesService cachedTimetablesService = context.RequestServices.GetService<ICachedTimetablesService>();
                IEnumerable<Timetable> timetables = cachedTimetablesService.GetTimetables("timetables20");

                string httpString = "<html>" +
                "<head>" +
                "<title>Таблица Timetables</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>Таблица 'Timetables'</div>" +
                "<div align='center'>" +
                "<table border=1>" +
                "<tr>" +
                "<td>День недели</td>" +
                "<td>Месяц</td>" +
                "<td>Год</td>" +
                "<td>Передача</td>" +
                "<td>Начало</td>" +
                "<td>Конец</td>" +
                "</tr>";

                foreach (Timetable timetable in timetables)
                {
                    httpString += "<tr>";
                    httpString += $"<td>{timetable.DayOfWeek}</td>";
                    httpString += $"<td>{timetable.Month}</td>";
                    httpString += $"<td>{timetable.Year}</td>";
                    httpString += $"<td>{timetable.Show?.Name}</td>";
                    httpString += $"<td>{timetable.StartTime}</td>";
                    httpString += $"<td>{timetable.EndTime}</td>";
                    httpString += "</tr>";
                }
                httpString += "</table>";

                httpString += "<div align='center'><a href='/'>Главная</a></div>";
                httpString += "</body>" +
                "</html>";

                await context.Response.WriteAsync(httpString);
            });
        }

        private static void SearchForm(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                ICachedGenresService cachedGenresService = context.RequestServices.GetService<ICachedGenresService>();
                IEnumerable<Genre> genres = cachedGenresService.GetGenres("genres20");

                ICachedShowsService cachedShowsService = context.RequestServices.GetService<ICachedShowsService>();
                IEnumerable<Show> shows = cachedShowsService.GetShows("shows20");

                ICachedTimetablesService cachedTimetablesService = context.RequestServices.GetService<ICachedTimetablesService>();
                IEnumerable<Timetable> timetables = cachedTimetablesService.GetTimetables("timetables20");

                string httpString = "<html>" +
                "<head>" +
                "<title>Форма поиска</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "select {font-size: 20; width=20%; }" +
                "input {font-size: 22; width=20%; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='middle' text-align='left'>" +
                "<form action='/searchform'>" +
                "<div width=20%>Выберете таблицу</div>" +
                "<select name='tableName'>" +
                "<option>Choose table</option>" +
                "<option>Genres</option>" +
                "<option>Shows</option>" +
                "<option>Timetables</option>" +
                "</select>" +
                "<input type = 'submit' value = 'Select'>";

                string selectedText = context.Request.Cookies["table"] ?? context.Request.Query["tableName"];

                if (context.Request.Cookies["table"] == "Choose table")
                    context.Response.Cookies.Delete("table");

                if (selectedText != null)
                {
                    if (selectedText != "Choose table" && selectedText != context.Request.Cookies["tableName"])
                    {
                        string querySttring = context.Request.Query["tableName"];
                        if (querySttring != null && querySttring != "Choose table")
                        {
                            context.Response.Cookies.Append("table", querySttring);
                            selectedText = querySttring;
                        }
                    }
                        

                    switch (selectedText)
                    {
                        case "Genres":
                            httpString += "<ul>";

                            foreach (Genre genre in genres)
                            {
                                httpString += $"<li>{genre.GenreName}</li>";
                            }
                            httpString += "</ul>";

                            break;
                        case "Shows":
                            httpString += "<ul>";

                            foreach (Show show in shows)
                            {
                                httpString += $"<li>{show.Name}</li>";
                            }
                            httpString += "</ul>";

                            break;
                        case "Timetables":
                            httpString += "<ul>";

                            foreach (Timetable timetable in timetables)
                            {
                                httpString += $"<li>{timetable.TimetableId}, {timetable.Show?.Name}.</li>";
                            }
                            httpString += "</ul>";
                            break;
                    }
                  
                    httpString += "<div>" +
                    "<input type='text' name='entity'>" +
                    "<input type='submit' value='Input'>" +
                    "</div>";

                    string entityInput;
                    if ((entityInput = context.Request.Query["entity"]) != null && entityInput != "")
                    {
                        switch (selectedText)
                        {
                            case "Genres":
                                Genre genre = genres.FirstOrDefault(g => g.GenreName == entityInput);
                                if (genre != null)
                                {
                                    httpString += "<div>" +
                                    "<p>" +
                                    $"Название жанра: {genre.GenreName}, Описание: {genre.GenreDescription}." +
                                    "</p>" +
                                    "</div>";
                                }
                                break;
                            case "Shows":
                                Show show = shows.FirstOrDefault(s => s.Name == entityInput);
                                if (show != null)
                                {
                                    httpString += "<div>" +
                                    "<p>" +
                                    $"Название: {show.Name}, Жанр {show.Genre?.GenreName}, Дата выхода: {show.ReleaseDate.ToString("d")}, " +
                                    $"Продолжительноть: {show.Duration}, Рейтинг: {show.Mark}." +
                                    "</p>" +
                                    "</div>";
                                }
                                break;
                            case "Timetables":
                                int id;
                                if (int.TryParse(entityInput, out id))
                                {
                                    Timetable newTimetable = timetables.FirstOrDefault(t => t.TimetableId == id);
                                    httpString += "<div>" +
                                    "<p>" +
                                    $"День недели: {newTimetable.DayOfWeek}, Месяц: {newTimetable.Month}, Год: {newTimetable.Year}, " +
                                    $"Передача {newTimetable.Show?.Name}, Начало {newTimetable.StartTime}, Конец {newTimetable.EndTime}.";
                                    httpString += "</p>" +
                                    "</div>";
                                }
                                break;
                        }
                    }
                }
                httpString += "</form>" +
                "<div><a href='/searchform'>Очистить</a></div>" +
                "<div><a href='/'>Главная</a></div>" +
                "</div>" +
                "</body>" +
                "</html>";

                await context.Response.WriteAsync(httpString);
            });
        }
    }
}
