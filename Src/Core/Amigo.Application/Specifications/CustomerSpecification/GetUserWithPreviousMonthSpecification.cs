using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.CustomerSpecification
{
    public class GetUserWithPreviousMonthSpecification : UserBaseSpecification
    {
        public GetUserWithPreviousMonthSpecification(DateTime currentMonthStart, DateTime previousMonthStart, List<string> adminIds)
            : base(u => !u.IsDeleted && u.CreatedDate >= previousMonthStart && u.CreatedDate < currentMonthStart
                && !adminIds.Contains(u.Id)
            )
        { }
    }
}
