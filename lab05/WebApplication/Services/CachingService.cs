using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.ViewModels;
using WebApplication.ViewModels.Entities;

namespace WebApplication.Services
{
    enum Models
    {
        Genre,
        Show,
        Timetable
    }

    public class CachingService<V, T> where V: IEntitiesViewModel<T>
    {
        private readonly TvChannelContext db;
        private readonly IMemoryCache cache;
        
        public CachingService(TvChannelContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void AddEntity(V model)
        {
            string key = $"{typeof(T).Name}-{model.PageViewModel.CurrentPage}";
            cache.Set(key, model);
        }

        public V GetEntity(int page)
        {
            string key = $"{typeof(T).Name}-{page}";
            return (V)cache.Get(key);
        }

        public void Clear(int page) 
        {
            string key = $"{typeof(T).Name}-{page}";
            cache.Remove(key);
        }

        public bool HasEntity(int page)
        {
            string key = $"{typeof(T).Name}-{page}";
            if (cache.Get(key) == null)
                return false;
            else
                return true;
        }
    }
}
