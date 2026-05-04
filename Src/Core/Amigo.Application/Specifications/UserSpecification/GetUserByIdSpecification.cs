using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.UserSpecification
{
    public class GetUserByIdSpecification : UserBaseSpecification
    {
        public GetUserByIdSpecification(string id) 
            : base(u => u.Id == id && !u.IsDeleted)
        {
            
        }
    }
}
