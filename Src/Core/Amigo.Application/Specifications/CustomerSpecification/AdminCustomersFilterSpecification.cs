using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CustomerSpecification
{
    public class AdminCustomersFilterSpecification : UserBaseSpecification
    {
        public AdminCustomersFilterSpecification(GetAllCustomersQuery query,List<string> adminIds) 
            : base(CustomerCommonSpecification.BuildCriteria(query ,adminIds))
        {
            AddInclude(u => u.Address);
            ApplyPagination(query.PageSize, query.PageNumber);
        }
    }
}
