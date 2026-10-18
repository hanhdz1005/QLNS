namespace QLNSCore.Entities
{
    public class BonusPenalty : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Employee Employee { get; set; }
        public BonusPenaltyType Type { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateOnly Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public enum BonusPenaltyType
    {
        Bonus = 1,
        Penalty = 2
    }
}
