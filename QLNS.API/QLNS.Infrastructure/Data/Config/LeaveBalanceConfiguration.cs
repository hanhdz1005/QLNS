using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNSCore.Entities;

namespace QLNS.Infrastructure.Data.Config
{
    public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
    {
        public void Configure(EntityTypeBuilder<LeaveBalance> builder)
        {
            builder.ToTable("LEAVE_BALANCES");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EmployeeName)
                   .HasMaxLength(200);

            builder.Property(x => x.TotalLeaveDays)
                   .HasColumnType("decimal(4,1)");

            builder.Property(x => x.UsedLeaveDays)
                   .HasColumnType("decimal(4,1)");

            builder.Property(x => x.RemainingLeaveDays)
                   .HasColumnType("decimal(4,1)");

            builder.HasIndex(x => new { x.EmployeeId, x.Year })
                   .IsUnique();

            builder.HasOne(x => x.Employee)
                   .WithMany(x => x.LeaveBalances)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
