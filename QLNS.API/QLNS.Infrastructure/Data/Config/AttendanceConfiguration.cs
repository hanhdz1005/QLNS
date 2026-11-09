using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNSCore.Entities;

namespace QLNS.Infrastructure.Data.Config
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("ATTENDANCES");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.EmployeeId).IsRequired();
            builder.Property(a => a.Date).IsRequired();
            builder.Property(a => a.CheckIn);
            builder.Property(a => a.CheckOut);
            builder.Property(a => a.RegularHours).HasColumnType("decimal(5,2)");
            builder.Property(a => a.OvertimeHours).HasColumnType("decimal(5,2)");
            builder.Property(a => a.HoursWorked).HasColumnType("decimal(5,2)");
            builder.Property(a => a.ScanCount).HasDefaultValue(1);
            builder.Property(a => a.Status).IsRequired();
            builder.HasOne(a => a.Employee)
               .WithMany(a => a.Attendances)
               .HasForeignKey(a => a.EmployeeId);
        }
    }
}
