namespace Entities
{
    public class PrivateInsuranceOptionalExtras
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal PremiumSupplement { get; set; }
        public int BaseAmount { get; set; }

        public int PrivateInsuranceId { get; set; }
        public PrivateInsurance? PrivateInsurance { get; set; }
    }
}
