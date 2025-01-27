namespace Presentation.Models
{
    public class InsuranceModel
    {
        public int Id { get; set; }
        public string InsuranceNumber { get; set; }
        public string CustomerName { get; set; }
        public string InsuranceType { get; set; }
        public bool IsPrivateCustomer { get; set; }
        public int AgentNumber { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public string Insured { get; set; }
        public string OrgSSN { get; set; }
    }
}
