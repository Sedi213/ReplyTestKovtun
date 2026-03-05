using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.Data.Configurations;

public class ContractDynamicFieldConfiguration : IEntityTypeConfiguration<ContractDynamicField>
{
    public void Configure(EntityTypeBuilder<ContractDynamicField> builder)
    {
        builder.HasKey(f => f.Id);
        builder.HasOne(f => f.Contract)
               .WithMany(c => c.DynamicFields)
               .HasForeignKey(f => f.ContractId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(f => new { f.ContractId, f.Name }).IsUnique();
        builder.Property(f => f.Type).HasConversion<string>();
    }
}
