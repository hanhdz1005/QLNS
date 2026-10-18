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
    public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("SALARIES");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.BasicSalary).IsRequired().HasColumnType("decimal(18,3)");
            builder.Property(s => s.ActualWorkDays).HasColumnType("decimal(5,2)").HasDefaultValue(0);
            builder.Property(s => s.PaidLeaveDays).HasColumnType("decimal(5,2)").HasDefaultValue(0);
            builder.Property(s => s.Allowance).IsRequired().HasColumnType("decimal(18,3)");
            builder.Property(s => s.Bonus).IsRequired().HasColumnType("decimal(18,3)");
            builder.Property(s => s.Penalty).IsRequired().HasColumnType("decimal(18,3)");
            builder.Property(s => s.OvertimePay).HasColumnType("decimal(18,3)").HasDefaultValue(0);
            builder.Property(s => s.GrossSalary).HasColumnType("decimal(18,3)").HasDefaultValue(0);
            builder.Property(s => s.Insurance).HasColumnType("decimal(18,3)").HasDefaultValue(0);
            builder.Property(s => s.Tax).HasColumnType("decimal(18,3)").HasDefaultValue(0);
            builder.Property(s => s.TotalSalary).IsRequired().HasColumnType("decimal(18,3)");
            builder.Property(s => s.Month).IsRequired();
            builder.Property(s => s.Year).IsRequired();

            builder.HasOne(s => s.Employee)
                   .WithMany(e => e.Salaries)
                   .HasForeignKey(s => s.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.EmployeeId, s.Month, s.Year }).IsUnique();
        }
    }
}
