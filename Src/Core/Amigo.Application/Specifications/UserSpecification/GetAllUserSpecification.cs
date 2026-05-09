using Amigo.SharedKernal.Entities;
using System;
using System.Linq.Expressions;

namespace Amigo.Application.Specifications.UserSpecification
{
    public class GetAllUserSpecification : UserBaseSpecification
    {
        public GetAllUserSpecification() 
            : base(u => u != null)
        {
        }
    }
}
