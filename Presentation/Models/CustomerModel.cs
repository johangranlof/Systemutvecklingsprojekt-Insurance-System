using Entities;
using System.Collections.ObjectModel;

namespace Presentation.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string OrgSSN { get; set; }
        public bool IsPrivateCustomer { get; set; }
        public bool IsProspect { get; set; }

        public ObservableCollection<CustomerProspectInformation> CustomerProspectInformations { get; set; } = new ObservableCollection<CustomerProspectInformation>();
    }
}
