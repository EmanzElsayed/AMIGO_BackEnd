using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.QueryParams
{
    public class GetAllOrdersQuery
    {
        public string? OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? OrderExpiresAt { get; set; }
        
        public string? BookingStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TourTitle { get; set; }

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
