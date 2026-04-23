using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CustomerSpecification
{
    public class GetUserWithEmailConfirmedSpecification : UserBaseSpecification
    {
        public GetUserWithEmailConfirmedSpecification(string email) 
            : base(u => u.NormalizedEmail == email.Trim().ToUpper() && !u.IsDeleted)
        {
        }
    }
}
