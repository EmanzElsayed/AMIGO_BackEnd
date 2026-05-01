using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amigo.Persistence.EntityConfiguration.Payments
{
    public class WebhookEventLogConfiguration
        : BaseEntityConfigurations<WebhookEventLog, Guid>
    {
        public override void Configure(EntityTypeBuilder<WebhookEventLog> builder)
        {
            base.Configure(builder);

        
            builder.Property(x => x.Provider)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(x => x.ProviderEventId)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Payload)
                   .HasColumnType("text") 
                   .IsRequired();

            builder.Property(x => x.Processed)
                   .IsRequired();
           
            builder.HasIndex(x => x.ProviderEventId)
                   .IsUnique();

            builder.HasIndex(x => x.Processed);

            builder.HasIndex(x => x.Provider);
        }
    }
}