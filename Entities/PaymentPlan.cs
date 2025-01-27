namespace Entities
{
    public class PaymentPlan
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;

        public ICollection<PrivateInsurance>? PrivateInsurances { get; set; }
        public ICollection<BusinessInsurance>? BusinessInsurances { get; set; }
    }
}
