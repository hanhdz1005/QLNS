using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Data.Config
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LEAVE_REQUESTS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                   .HasMaxLength(500);

            builder.Property(x => x.AprovedBy)
                   .IsRequired(false);

            builder.Property(x => x.LeaveType)
                   .HasConversion<int>();

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.HasOne(x => x.Employee)
                   .WithMany(x => x.LeaveRequests)
                   .HasForeignKey(x => x.EmployeeId);
        }
    }
}
