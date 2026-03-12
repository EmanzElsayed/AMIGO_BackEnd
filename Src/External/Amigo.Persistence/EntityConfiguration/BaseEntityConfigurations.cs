
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration
{
    public class BaseEntityConfigurations<TEntity,TKey>:IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TKey>
       
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.CreatedBy);
            
            builder.Property(e => e.CreatedDate)
                   .HasDefaultValueSql("TIMEZONE('UTC', NOW())")
                   .IsRequired();

            builder.Property(e => e.ModifiedBy);

            builder.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("TIMEZONE('UTC', NOW())");
                

            builder.Property(e => e.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}
