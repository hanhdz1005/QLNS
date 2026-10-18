using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNSCore.Entities;

namespace QLNS.Infrastructure.Data.Config
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("CONTRACTS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContractNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.EmployeeName)
                   .HasMaxLength(200);

            builder.Property(x => x.ContractType)
                   .HasConversion<int>();

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.ContractSalary)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.InsuranceSalary)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.AttachmentUrl)
                   .HasMaxLength(500);

            builder.Property(x => x.Notes)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Employee)
                   .WithMany(x => x.Contracts)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
