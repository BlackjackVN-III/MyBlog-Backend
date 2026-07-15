using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Application.Queries
{
    public class QueryObject
    {
        public string? Search { get; set; } = null;       
        public string? TagSlug { get; set; } = null;  
        public string? SortBy { get; set; } = null;

        public bool isDecsending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
