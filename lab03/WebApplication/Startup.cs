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

            services.AddTransient<CachedGenresService>();
            services.AddTransient<CachedShowsService>();
            services.AddTransient<CachedTimetablesService>();

            services.AddMemoryCache();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TvChannelContext tc)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/info", Info);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Page not found");
            });
        }

        private static void Info(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                string httpString = "<html><head>" +
               "<title>Информация о клиенте</title>" +
               "<style>" +
               "div { font-size: 24; }" +
               "</style></head>" +
               "<meta charset='utf-8'/>" +
               "<body><div> Сервер: " + context.Request.Host + "</div>" +
               "<div> Путь: " + context.Request.PathBase + "</div>" +
               "<div> Протокол: " + context.Request.Protocol + "</div>" +
               "<div><a href='/'>Главная</a></div>" +
               "</body></html>";

                await context.Response.WriteAsync(httpString);
            });
           
        }
    }
}
