using Microsoft.EntityFrameworkCore;

namespace Entities
{
    [PrimaryKey(nameof(EmployeeNumber), nameof(RoleId))]
    public class EmployeeRole
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int EmployeeNumber { get; set; }
        public Employee? Employee { get; set; }
        public int EmploymentRate { get; set; }
        public int AgentNumber { get; set; }
    }
}
