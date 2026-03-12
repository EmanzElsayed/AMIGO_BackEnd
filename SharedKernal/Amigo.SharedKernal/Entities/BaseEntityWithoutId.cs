using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.SharedKernal.Entities
{
    public class BaseEntityWithoutId:IBaseEntity
    {


        
        public int? CreatedBy { get; private set; }

        
        public DateTime CreatedDate { get; private set; }

        
        public int? ModifiedBy { get; private set; }

       
        public DateTime? ModifiedDate { get; private set; }

       
        public bool IsDeleted { get; private set; }

        protected BaseEntityWithoutId()
        {

        }

        public void SetCreatedBy(int? createdBy) => CreatedBy = createdBy;
        public void SetCreatedDate(DateTime createdDate) => CreatedDate = createdDate;
        public void SetModifiedBy(int? modifiedBy) => ModifiedBy = modifiedBy;
        public void SetModifiedDate(DateTime? modifiedDate) => ModifiedDate = modifiedDate;
        public void SetIsDeleted(bool isDeleted) => IsDeleted = isDeleted;
    }
}
