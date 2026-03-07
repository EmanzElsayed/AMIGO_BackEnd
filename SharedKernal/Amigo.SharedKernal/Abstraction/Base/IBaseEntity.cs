namespace Amigo.SharedKernal.Abstraction.Base;

public interface IBaseEntity
{
    public int? CreatedBy { get; }
    public DateTime CreatedDate { get; }
    public int? ModifiedBy { get; }
    public DateTime? ModifiedDate { get; }
    public bool IsDeleted { get; }
    public void SetCreatedBy(int? createdBy);
    public void SetCreatedDate(DateTime createdDate);
    public void SetModifiedBy(int? modifiedBy);
    public void SetModifiedDate(DateTime? modifiedDate);
    public void SetIsDeleted(bool isDeleted);
}
