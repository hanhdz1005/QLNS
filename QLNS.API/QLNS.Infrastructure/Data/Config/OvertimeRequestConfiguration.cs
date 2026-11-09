using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNSCore.Entities;

namespace QLNS.Infrastructure.Data.Config
{
    public class OvertimeRequestConfiguration : IEntityTypeConfiguration<OvertimeRequest>
    {
        public void Configure(EntityTypeBuilder<OvertimeRequest> builder)
        {
            builder.ToTable("OVERTIME_REQUESTS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EmployeeName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.RegisteredHours)
                   .HasColumnType("decimal(5,2)")
                   .IsRequired();

            builder.Property(x => x.Reason)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.ApprovedBy)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.HasOne(x => x.Employee)
                   .WithMany(x => x.OvertimeRequests)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
