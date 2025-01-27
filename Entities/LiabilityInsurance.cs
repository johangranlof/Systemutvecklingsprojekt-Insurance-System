namespace Entities
{
    public class LiabilityInsurance : BusinessInsurance
    {
        public decimal Deductible { get; set; }
        public int Extent { get; set; }
    }
}
