using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.UserSpecification
{
    public class GetUserWithCurrentMonthSpecification : UserBaseSpecification
    {
        public GetUserWithCurrentMonthSpecification(DateTime currentMonthStart , DateTime nextMonthStart ,List<string> adminIds) 
            : base(u => !u.IsDeleted && u.CreatedDate >= currentMonthStart && u.CreatedDate < nextMonthStart
                && !adminIds.Contains(u.Id)
            )
        {
        }
    }
}
