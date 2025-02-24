using ITnetworkProjekt.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITnetworkProjekt.Data.Configurations
{
    public class InsuredPersonConfig : IEntityTypeConfiguration<InsuredPerson>
    {
        public void Configure(EntityTypeBuilder<InsuredPerson> builder)
        {
            builder.ToTable("InsuredPersons");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.DateOfBirth).IsRequired();
            builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.Email).IsUnique();
            builder.Property(p => p.Address).IsRequired().HasMaxLength(200);
            builder.Property(p => p.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.SocialSecurityNumber).IsRequired().HasMaxLength(11);
            builder.HasIndex(p => p.SocialSecurityNumber).IsUnique();
            builder.Property(p => p.UserId).HasMaxLength(450);
        }
    }
}
