using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Shared.Common.Pagination
{
    public class UserQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "LastName";
        public bool SortDescending { get; set; } = false;
        public string? SearchTerm { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }

   
}
