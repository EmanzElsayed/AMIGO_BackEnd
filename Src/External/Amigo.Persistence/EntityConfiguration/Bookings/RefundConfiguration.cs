using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{
    public class RefundConfiguration : BaseEntityConfigurations<Refund, Guid>
    {
        public override void Configure(EntityTypeBuilder<Refund> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Amount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.ProviderRefundId)
                   .HasMaxLength(250);

            builder.HasIndex(x => x.PaymentId);

            builder.HasOne(x => x.Payment)
                   .WithMany()
                   .HasForeignKey(x => x.PaymentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
