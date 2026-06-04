using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.QueryParams
{
    public class GetAllCustomersQuery
    {

        public string? Query { get; set; }
        public string? Q { get; set; }
        public string? Search { get; set; }

        public string? Status { get; set; }

        public string? Country { get; set; }
        public string? CountryCode { get; set; }


        #region Pagination

            private const int DefaultPageSize = 5;
            private const int MaxPageSize = 10;


        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }
        private int pageSize = DefaultPageSize;

            public int PageSize
            {
                get { return pageSize; }
                set { pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? DefaultPageSize : value); }
            }
            #endregion
        
    }
}
