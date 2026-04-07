using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;

namespace Amigo.SharedKernal.QueryParams
{
    public class GetAllDestinationQuery
    {
        public string? Name { get; set; }
        public string? CountryCode { get; set; }
        public string? Language { get; set; } 



        #region Pagination

        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 10;


        public int PageNumber = 1;

        private int pageSize = DefaultPageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? DefaultPageSize : value); }
        }
        #endregion
    }
}
