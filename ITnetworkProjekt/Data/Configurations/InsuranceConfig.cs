using ITnetworkProjekt.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITnetworkProjekt.Data.Configurations
{
    public class InsuranceConfig : IEntityTypeConfiguration<Insurance>
    {
        public void Configure(EntityTypeBuilder<Insurance> builder)
        {
            builder.ToTable("Insurances");
            builder.HasKey(i => i.Id);
            builder.HasOne(i => i.InsuredPerson)
                   .WithMany() 
                   .HasForeignKey(i => i.InsuredPersonId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(i => i.PolicyType).IsRequired().HasMaxLength(50);
            builder.Property(i => i.StartDate).IsRequired();
            builder.Property(i => i.EndDate).IsRequired();
            builder.Property(i => i.PremiumAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(i => i.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");

            builder.HasIndex(i => i.InsuredPersonId);
            builder.HasIndex(i => i.PolicyType);
            builder.HasIndex(i => i.StartDate);
        }
    }
}
