namespace Entities
{
    public class BusinessCustomer : Customer
    {
        public string OrganisationNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string AreaCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public ICollection<BusinessInsurance>? BusinessInsurances { get; set; }
    }
}