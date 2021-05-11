using EcommerceWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Filter
{
    public class PaginationFilter<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public List<T> PagedData { get; set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }


        public PaginationFilter()
        {

        }

        //public PaginationFilter()
        //{
        //    this.PageNumber = 1;
        //    this.PageSize = 10;
        //}

        //public PaginationFilter(int pageNumber, int pageSize)
        //{
        //    this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
        //    this.PageSize = pageSize > 10 ? 10 : pageSize;
        //}
    }
}
