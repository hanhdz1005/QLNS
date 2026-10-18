using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNSCore.Entities;

namespace QLNS.Infrastructure.Data.Config
{
    public class BonusPenaltyConfiguration : IEntityTypeConfiguration<BonusPenalty>
    {
        public void Configure(EntityTypeBuilder<BonusPenalty> builder)
        {
            builder.ToTable("BONUS_PENALTIES");

            builder.HasKey(bp => bp.Id);
            builder.Property(bp => bp.EmployeeId).IsRequired();
            builder.Property(bp => bp.EmployeeName).HasMaxLength(100);
            builder.Property(bp => bp.Type).IsRequired();
            builder.Property(bp => bp.Amount).HasColumnType("decimal(18,2)");
            builder.Property(bp => bp.Reason).HasMaxLength(500);
            builder.Property(bp => bp.Date).IsRequired();
            builder.Property(bp => bp.Month).IsRequired();
            builder.Property(bp => bp.Year).IsRequired();
            builder.HasOne(bp => bp.Employee)
                .WithMany(e => e.BonusPenalties)
                .HasForeignKey(bp => bp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
