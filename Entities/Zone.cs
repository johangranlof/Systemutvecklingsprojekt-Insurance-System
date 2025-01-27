using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Zone
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(3, 1)")]
        public decimal Factor { get; set; }

        public ICollection<City>? Cities { get; set; }
        public ICollection<VehicleInsurance>? Vehicles { get; set; }
    }
}
