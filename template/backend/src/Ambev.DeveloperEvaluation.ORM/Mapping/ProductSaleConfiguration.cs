using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductSaleConfiguration : IEntityTypeConfiguration<ProductSale>
{
    public void Configure(EntityTypeBuilder<ProductSale> builder)
    {
        builder.ToTable("ProductSales");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnType("uuid");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Quantity)
            .IsRequired();

        builder.Property(p => p.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne<SaleEntity>()
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}