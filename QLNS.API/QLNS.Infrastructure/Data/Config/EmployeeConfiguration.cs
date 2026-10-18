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
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("EMPLOYEES");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.FullName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Email)
                   .HasMaxLength(150);

            builder.Property(a => a.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(a => a.Status)
                   .HasConversion<int>();

            builder.Property(a => a.CitizenId)
                   .HasMaxLength(20);

            builder.Property(a => a.CitizenIdIssuePlace)
                   .HasMaxLength(200);

            builder.Property(a => a.TaxCode)
                   .HasMaxLength(30);

            builder.Property(a => a.SocialInsuranceNumber)
                   .HasMaxLength(30);

            builder.Property(a => a.BankAccountNumber)
                   .HasMaxLength(50);

            builder.Property(a => a.BankName)
                   .HasMaxLength(100);

            builder.Property(a => a.BankBranch)
                   .HasMaxLength(100);

            builder.Property(a => a.EmergencyContactName)
                   .HasMaxLength(100);

            builder.Property(a => a.EmergencyContactPhone)
                   .HasMaxLength(20);

            builder.Property(a => a.EmergencyContactRelationship)
                   .HasMaxLength(50);

            builder.HasOne(a => a.Department)
                   .WithMany(a => a.Employees)
                   .HasForeignKey(a => a.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Position)
                   .WithMany(a => a.Employees)
                   .HasForeignKey(a => a.PositionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Manager)
                   .WithMany(a => a.Subordinates)
                   .HasForeignKey(a => a.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}