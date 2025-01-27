namespace Entities
{
    public class VehicleInsurance : BusinessInsurance
    {
        public string RegNumber { get; set; } = string.Empty;
        public string DriverFirstName { get; set; } = string.Empty;
        public string DriverLastName { get; set; } = string.Empty;
        public string DriverSSN { get; set; }
        public decimal Debuctible { get; set; }
        public int Extent { get; set; }

        public int ZoneId { get; set; }
        public Zone? Zone { get; set; }
    }
}
