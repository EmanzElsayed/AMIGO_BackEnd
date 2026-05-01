using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amigo.Persistence.EntityConfiguration.Outbox
{
    public class OutboxMessageConfiguration
        : BaseEntityConfigurations<OutboxMessage, Guid>
    {
        public override void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Type)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Payload)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(x => x.RetryCount)
                   .IsRequired()
                   .HasDefaultValue(0);

           

            builder.Property(x => x.LastError)
                   .HasMaxLength(1000);

            builder.HasIndex(x => new { x.Status, x.CreatedDate });

            builder.HasIndex(x => x.NextRetryAt);

            builder.HasIndex(x => x.Type);
        }
    }
}