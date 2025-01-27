namespace Entities
{
    public class CustomerProspectInformation
    {
        public int Id { get; set; }
        public DateTime ContactDate { get; set; }
        public string Outcome { get; set; } = string.Empty;
        public int AgentNumber { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

    }
}
