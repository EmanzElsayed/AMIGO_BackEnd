using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Amigo.Persistence.EntityConfiguration.Orders
{

    public class PaymentConfiguration : BaseEntityConfigurations<Payment, Guid>
    {
        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            base.Configure(builder);

            

            builder.Property(p => p.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PaymentMethod)
                   .HasConversion<int>();

            builder.Property(p => p.Status)
                   .HasConversion<int>();

            builder.Property(p => p.Currency)
                   .HasConversion<int>();

            builder.Property(p => p.TransactionId)
                   .HasMaxLength(200);

            builder.HasIndex(p => p.OrderId);

            builder.HasOne(p => p.Order)
                   .WithMany()
                   .HasForeignKey(p => p.OrderId);
        }
    }
}
