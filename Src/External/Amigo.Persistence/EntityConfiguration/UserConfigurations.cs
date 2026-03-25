using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration
{
    public class UserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.HasKey(x => x.Id);

           

            //Identity Info 
            builder.Property(u => u.UserName)
               .HasMaxLength(256);

            builder.Property(u => u.Email)
                          .HasMaxLength(256)
                          ;
            //ApplicationUser Info


            builder.Property(u => u.FullName)
              .HasMaxLength(200);

            builder.Property(u => u.IsActive)
                   .HasDefaultValue(true);

            builder.Property(u => u.Image)
                   .HasMaxLength(500);

            builder.Property(u => u.Gender)
                   .HasConversion<int>(); 

            builder.Property(u => u.BirthDate)
                   .HasColumnType("date");

            builder.Property(u => u.Nationality)
                    
                           .HasMaxLength(200);

            builder.Property(u => u.Language)
                   .HasConversion<int>();

            // Owned type Address

            builder.OwnsOne(u => u.Address, a =>
            {
                a.Property(p => p.BuildingNumber).HasMaxLength(200);
                a.Property(p => p.City).HasMaxLength(100);
                a.Property(p => p.Country).HasMaxLength(100);
            });

            //Index 

            builder.HasIndex(u => u.NormalizedEmail).IsUnique();
            builder.HasIndex(u => u.PhoneNumber).IsUnique(false);

            //Base Info 


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
