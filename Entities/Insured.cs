namespace Entities
{
    public class Insured
    {
        public int Id { get; set; }
        public string SSN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ICollection<PrivateInsurance>? PrivateInsurances { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
