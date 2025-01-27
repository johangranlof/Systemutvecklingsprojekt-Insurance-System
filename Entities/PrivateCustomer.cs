namespace Entities
{
    public class PrivateCustomer : Customer
    {
        public string SSN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MobilePhoneNumber { get; set; }
        public string? WorkPhoneNumber { get; set; }
        public ICollection<PrivateInsurance>? PrivateInsurances { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
