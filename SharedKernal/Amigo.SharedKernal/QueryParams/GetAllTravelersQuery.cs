using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.QueryParams
{
    public class GetAllTravelersQuery
    {
        [Required(ErrorMessage = "Type Of Travelers Required")]
        public string Type { get; set; } = null!;
        public Guid? CartItemId { get; set; }
        public string? FullName { get; set; }



    }
}
