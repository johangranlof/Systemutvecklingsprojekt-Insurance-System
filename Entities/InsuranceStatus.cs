namespace Entities
{
    public class InsuranceStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;

        public ICollection<BusinessInsurance>? BusinessInsurances { get; set; }
        public ICollection<PrivateInsurance>? PrivateInsurances { get; set; }
    }
}
