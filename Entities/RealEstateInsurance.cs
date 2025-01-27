namespace Entities
{
    public class RealEstateInsurance : BusinessInsurance
    {
        public string StreetAddress { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal PropertyValue { get; set; }
        public decimal RealEstatePremium { get; set; }
        public decimal InventoryValue { get; set; }
        public decimal InventoryPrice { get; set; }
    }
}
