using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;

        public ICollection<BusinessInsurance>? BusinessInsurances { get; set; }
        public ICollection<PrivateInsurance>? PrivateInsurances { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeNumber { get; set; }
        public Employee Employee { get; set; }

        public string PasswordMasked => new string('*', Password?.Length ?? 0);
    }
}
