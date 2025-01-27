using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeNumber { get; set; }
        public string SSN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public ICollection<EmployeeRole>? EmployeeRoles { get; set; }
        public ICollection<User>? Users { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public decimal Comission { get; set; }
    }
}
