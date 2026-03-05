using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Email).IsRequired();
        builder.HasIndex(c => new { c.Email, c.UserId });
        builder.HasMany(c => c.DynamicFields)
               .WithOne(f => f.Contract)
               .HasForeignKey(f => f.ContractId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
