﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewModels
{
    public class PageViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; private set; }

        public void SetPages(int count, int pageSize = 10)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (CurrentPage <= 0)
                CurrentPage = 1;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;
        }

        public bool HasPreviousPage
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }
    }
}

