namespace Presentation.Models
{
    public class PrivateInvoiceModel
    {
        public string Förnamn { get; set; }
        public string Efternamn { get; set; }
        public string Personnummer { get; set; }
        public string Adress { get; set; }
        public string Postnummer { get; set; }
        public string Ort { get; set; }
        public List<string> Försäkringar { get; set; }
        public decimal TotalPremie { get; set; }
    }

    public class BusinessInvoiceModel
    {
        public string Företagsnamn { get; set; }
        public string Organisationsnummer { get; set; }
        public string Kontaktperson { get; set; }
        public string Adress { get; set; }
        public string Postnummer { get; set; }
        public string Ort { get; set; }
        public List<string> Försäkringar { get; set; }
        public decimal TotalPremie { get; set; }
    }
}
