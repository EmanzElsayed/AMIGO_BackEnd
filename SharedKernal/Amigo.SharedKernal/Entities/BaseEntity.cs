namespace Amigo.SharedKernal.Entities;

public abstract class BaseEntity<T> : IBaseEntity
{
    [Key]
    [Column(Order = 1)]
    public T Id { get;  set; }

    [Column(Order = 2)]
    public int? CreatedBy { get; private set; }

    [Column(Order = 3)]
    public DateTime CreatedDate { get; private set; }

    [Column(Order = 4)]
    public int? ModifiedBy { get; private set; }

    [Column(Order = 5)]
    public DateTime? ModifiedDate { get; private set; }

    [Column(Order = 6)]
    public bool IsDeleted { get; private set; }

    protected BaseEntity()
    {

    }

    public void SetCreatedBy(int? createdBy) => CreatedBy = createdBy;
    public void SetCreatedDate(DateTime createdDate) => CreatedDate = createdDate;
    public void SetModifiedBy(int? modifiedBy) => ModifiedBy = modifiedBy;
    public void SetModifiedDate(DateTime? modifiedDate) => ModifiedDate = modifiedDate;
    public void SetIsDeleted(bool isDeleted) => IsDeleted = isDeleted;
}