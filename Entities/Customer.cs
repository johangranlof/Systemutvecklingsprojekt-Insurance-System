using System.Collections.ObjectModel;

namespace Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool Anonymized { get; set; }
        public bool IsPrivateCustomer { get; set; }
        public string? Email { get; set; }
        public ObservableCollection<CustomerProspectInformation> CustomerProspectInformation { get; set; }
    }
}
