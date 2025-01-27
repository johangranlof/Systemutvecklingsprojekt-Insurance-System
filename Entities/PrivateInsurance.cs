namespace Entities
{
    public class PrivateInsurance
    {
        public int Id { get; set; }
        public string InsuranceNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AgentNumber { get; set; }
        public string? Note { get; set; }
        public int BaseAmount { get; set; }
        public decimal Premium { get; set; }
        public bool Anonymized { get; set; }
        public string InsuranceType { get; set; } = string.Empty;
        public DateTime? NextPaymentDate { get; set; }
        public int PrivateCustomerId { get; set; }
        public PrivateCustomer? PrivateCustomer { get; set; }

        public int InsuredId { get; set; }
        public Insured? Insured { get; set; }

        public int PaymentPlanId { get; set; }
        public PaymentPlan? PaymentPlan { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int InsuranceStatusId { get; set; }
        public InsuranceStatus? InsuranceStatus { get; set; }

        public ICollection<PrivateInsuranceOptionalExtras>? PrivateInsuranceOptionalExtras { get; set; }
    }
}
