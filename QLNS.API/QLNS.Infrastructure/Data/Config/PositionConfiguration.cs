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
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("POSITIONS");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PositionName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.BaseSalary)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
