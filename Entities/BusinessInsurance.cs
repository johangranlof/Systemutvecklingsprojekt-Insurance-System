namespace Entities
{
    public class BusinessInsurance
    {
        public int Id { get; set; }
        public string InsuranceNumber { get; set; }
        public string ContactFirstName { get; set; } = string.Empty;
        public string ContactLastName { get; set; } = string.Empty;
        public string ContactPhoneNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Premium { get; set; }
        public int AgentNumber { get; set; }
        public string? Note { get; set; }
        public DateTime? NextPaymentDate { get; set; }

        public int BusinessCustomerId { get; set; }
        public BusinessCustomer? BusinessCustomer { get; set; }

        public int PaymentPlanId { get; set; }
        public PaymentPlan? PaymentPlan { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int InsuranceStatusId { get; set; }
        public InsuranceStatus? InsuranceStatus { get; set; }

        public decimal SellerCommission => Premium * 0.12m;
    }
}
