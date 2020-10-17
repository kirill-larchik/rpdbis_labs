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

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TvChannelContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

                await context.Response.WriteAsync("Page not found");
            });
        }

        private static void Info(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                string httpString = "<html>" +
                "<head>" +
                "<title>���������� � �������</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body align='middle'>" +
                "<div> ������: " + context.Request.Host + "</div>" +
                "<div> ����: " + context.Request.PathBase + "</div>" +
                "<div> ��������: " + context.Request.Protocol + "</div>" +
                "<div><a href='/'>�������</a></div>" +
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
                "<title>������� Genres</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>������� 'Genres'</div>" +
                "<div align='center'>" +
                "<table border=1>" +
                "<tr>" +
                "<td>����</td>" +
                "<td>�������� �����</td>" +
                "</tr>";

               foreach (Genre genre in genres)
                {
                    httpString += "<tr>";
                    httpString += $"<td>{genre.GenreName}</td>";
                    httpString += $"<td>{genre.GenreDescription}</td>";
                    httpString += "</tr>";
                }
                httpString += "</table>";
               
                httpString += "<div align='center'><a href='/'>�������</a></div>";
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
                "<title>������� Shows</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>������� 'Shows'</div>" +
                "<div align='center'>" +
                "<table border=1>" +
                "<tr>" +
                "<td>��������</td>" +
                "<td>�������� �����</td>" +
                "<td>���� ������</td>" +
                "<td>�����������������</td>" +
                "<td>�������</td>" +
                "<td>������� �� �����</td>" +
                "<td>������� �� ���</td>" +
                "<td>����</td>" + 
                "<td>��������</td>" +
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

                httpString += "<div align='center'><a href='/'>�������</a></div>";
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
                "<title>������� Timetables</title>" +
                "<style>" +
                "div { font-size: 24; }" +
                "table { font-size: 20; }" +
                "</style>" +
                "</head>" +
                "<meta charset='utf-8'/>" +
                "<body>" +
                "<div align='center'>������� 'Timetables'</div>" +
                "<div align='center'>" +
                "<table border=1>" +
                "<tr>" +
                "<td>���� ������</td>" +
                "<td>�����</td>" +
                "<td>���</td>" +
                "<td>��������</td>" +
                "<td>������</td>" +
                "<td>�����</td>" +
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

                httpString += "<div align='center'><a href='/'>�������</a></div>";
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
                "<title>����� ������</title>" +
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
                "<div width=20%>�������� �������</div>" +
                "<select name='tableName'>" +
                "<option>Genres</option>" +
                "<option>Shows</option>" +
                "<option>Timetables</option>" +
                "</select>" +
                "<input type = 'submit' value = 'Select'>";

                string input;
                if ((input = context.Request.Query["tableName"]) != null)
                {
                    string text = context.Request.Query["tableName"];
                    switch (text)
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
                                httpString += $"<li>{timetable.TimetableId}, ({timetable.Show?.Name}.</li>";
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
                        switch (input)
                        {
                            case "Genres":
                                Genre genre = genres.FirstOrDefault(g => g.GenreName == entityInput);
                                if (genre != null)
                                {
                                    httpString += "<div>" +
                                    "<p>" +
                                    $"�������� �����: {genre.GenreName}, ��������: {genre.GenreDescription}." +
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
                                    $"��������: {show.Name}, ���� {show.Genre?.GenreName}, ���� ������: {show.ReleaseDate.ToString("d")}, " +
                                    $"����������������: {show.Duration}, �������: {show.Mark}." + 
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
                                    $"���� ������: {newTimetable.DayOfWeek}, �����: {newTimetable.Month}, ���: {newTimetable.Year}, " +
                                    $"�������� {newTimetable.Show?.Name}, ������ {newTimetable.StartTime}, ����� {newTimetable.EndTime}.";
                                    httpString += "</p>" +
                                    "</div>";
                                }
                                break;
                        }
                    }
                }
                
                httpString += "</form>" +
                "<div><a href='/searchform'>��������</a></div>" +
                "<div><a href='/'>�������</a></div>" +
                "</div>" +
                "</body>" +
                "</html>";

                await context.Response.WriteAsync(httpString);
            });
        }
    }
}
