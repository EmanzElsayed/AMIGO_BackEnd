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



            builder.Property(x => x.TotalAmount)
               .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaymentMethod)
                   .HasConversion<int>();

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.Currency)
                   .HasConversion<int>();

            builder.Property(x => x.Note)
                   .HasMaxLength(1000);

            builder.Property(x => x.Provider)
                   .HasConversion<int>();

           

            builder.Property(x => x.ProviderTransactionId)
                   .HasMaxLength(250);

            builder.Property(x => x.RawResponseJson)
                   .HasMaxLength(5000);

            builder.HasIndex(x => x.OrderId);

            builder.HasOne(x => x.Order)
                   .WithMany(o => o.Payments)
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
