using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SalesConfiguration : IEntityTypeConfiguration<SaleEntity>
{
    public void Configure(EntityTypeBuilder<SaleEntity> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnType("uuid");

        builder.Property(s => s.Date)
            .IsRequired();

        builder.Property(s => s.Customer)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Branch)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.TotalSaleAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.IsCanceled)
            .IsRequired();

        builder.HasMany(s => s.Products)
            .WithOne()
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}