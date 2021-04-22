using System;
using System.Linq;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.PictureUrl).IsRequired();
            builder.HasOne(b => b.ProductBrand).WithMany()
                .HasForeignKey(p => p.ProductBrandId);
            // builder.HasOne(t => t.ProductType).WithMany()
            //     .HasForeignKey(p => p.ProductTypeId);

            builder.HasOne(v => v.Vendor).WithMany()
                .HasForeignKey(p => p.VendorId);

            builder.HasMany(m => m.Medias).WithOne();
            builder.HasMany(m => m.Reviews).WithOne();

            builder.Property(p => p.DiscountedPrice).HasColumnType("decimal(18,2)");

            builder.Property(p => p.ReturnPolicies)
                    .HasConversion(r => string.Join(',', r), r => r.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            builder.HasOne(p => p.ProductCategory).WithMany()
                    .HasForeignKey(p => p.ProductCategoryId);
        }
    }
}