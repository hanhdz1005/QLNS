using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Infrastructure.Data.Config
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("DEPARTMENTS");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.DepartmentName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(d => d.Description)
                   .HasMaxLength(500);
        }
    }
}
