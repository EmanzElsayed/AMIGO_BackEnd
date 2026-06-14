using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{
    public class CancellationConfigurations :BaseEntityConfigurations<Cancellation,Guid>

    {
        public override void Configure(EntityTypeBuilder<Cancellation> builder)
        {
            base.Configure(builder);

            

            // Foreign Key
            builder.Property(x => x.TourId)
                   .HasColumnType("uuid")
                   .IsRequired();

            
            builder.Property(x => x.CancelationPolicyType)
                   .IsRequired()
                   .HasConversion<int>();


            builder.Property(x => x.CancellationBefore)
                 .HasColumnType("interval")
                 .IsRequired();
                   

          
            builder.Property(x => x.RefundPercentage)
                   .HasPrecision(5, 2)
                   .IsRequired();

            //relations

            builder.HasOne(x => x.Tour)
                   .WithMany(t => t.Cancellations)
                   .HasForeignKey(x => x.TourId)
                   .OnDelete(DeleteBehavior.Cascade);





            builder.HasIndex(x => x.TourId);
                   
        }
    }
}
