using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.CustomerSpecification
{
    public class CountAdminCustomersFilterSpecification : UserBaseSpecification
    {
        public CountAdminCustomersFilterSpecification(GetAllCustomersQuery query , List<string> adminIds)
            : base(CustomerCommonSpecification.BuildCriteria(query, adminIds))
        { 
        
        }
    }
}
