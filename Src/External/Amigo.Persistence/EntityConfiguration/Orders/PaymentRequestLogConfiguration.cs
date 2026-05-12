using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Orders
{
   
        public class PaymentRequestLogConfiguration
            : BaseEntityConfigurations<PaymentRequestLog, Guid>
        {
            public override void Configure(EntityTypeBuilder<PaymentRequestLog> builder)
            {
                base.Configure(builder);

               
                builder.Property(x => x.RequestId)
                       .HasMaxLength(100)
                       .IsRequired();

               
                builder.Property(x => x.OrderId)
                       .IsRequired();

              
                builder.Property(x => x.ProviderPaymentId)
                       .HasMaxLength(200)
                       .IsRequired();

               
                builder.Property(x => x.ResponseJson)
                       .HasColumnType("text")
                       .IsRequired();

                builder.Property(x => x.CreatedAt)
                       .IsRequired();

              
                builder.HasIndex(x => x.RequestId)
                       .IsUnique();

                builder.HasIndex(x => x.OrderId);

                builder.HasIndex(x => x.ProviderPaymentId);
            }
        }
    
}
