using Business.Controllers;
using Entities;
using Presentation.Models;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Presentation.Helpers
{
    public class InvoiceGenerator
    {
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly BusinessInsuranceController _businessInsuranceController;

        public InvoiceGenerator(PrivateInsuranceController privateInsuranceController, BusinessInsuranceController businessInsuranceController)
        {
            _privateInsuranceController = privateInsuranceController;
            _businessInsuranceController = businessInsuranceController;
        }

        public void GenerateInvoiceFile()
        {
            // Hämta nuvarande datum för att kontrollera försäkringar som ska faktureras denna månad
            DateTime currentDate = DateTime.Now;

            // Hämta alla privata försäkringar med betalningsdatum som matchar nuvarande månad och år
            var privateInsurances = _privateInsuranceController.GetAllPrivateInsurances()
                .Where(pi => pi.NextPaymentDate.HasValue
                             && pi.NextPaymentDate.Value.Year == currentDate.Year
                             && pi.NextPaymentDate.Value.Month == currentDate.Month)
                .ToList();

            // Hämta alla företagsförsäkringar med betalningsdatum som matchar nuvarande månad och år
            var businessInsurances = _businessInsuranceController.GetAllBusinessInsurances()
                .Where(bi => bi.NextPaymentDate.HasValue
                             && bi.NextPaymentDate.Value.Year == currentDate.Year
                             && bi.NextPaymentDate.Value.Month == currentDate.Month)
                .ToList();

            // Gruppera privata försäkringar per kund och sammanställ fakturadata för varje kund
            var privateInvoiceData = privateInsurances.GroupBy(pi => pi.PrivateCustomer.SSN)
                .Select(group => new PrivateInvoiceModel
                {
                    Förnamn = group.First().PrivateCustomer.FirstName,
                    Efternamn = group.First().PrivateCustomer.LastName,
                    Personnummer = group.First().PrivateCustomer.SSN,
                    Adress = group.First().PrivateCustomer.StreetAddress,
                    Postnummer = group.First().PrivateCustomer.ZipCode,
                    Ort = group.First().PrivateCustomer.City,
                    Försäkringar = group.Select(pi => pi.InsuranceType).ToList(),
                    TotalPremie = group.Sum(pi => pi.Premium +
                        pi.PrivateInsuranceOptionalExtras.Sum(extra => extra.PremiumSupplement))
                }).ToList();

            // Gruppera företagsförsäkringar per kund och sammanställ fakturadata för varje företag
            var businessInvoiceData = businessInsurances.GroupBy(bi => bi.BusinessCustomer.OrganisationNumber)
                .Select(group => new BusinessInvoiceModel
                {
                    Företagsnamn = group.First().BusinessCustomer.CompanyName,
                    Organisationsnummer = group.First().BusinessCustomer.OrganisationNumber,
                    Kontaktperson = group.First().ContactFirstName,
                    Adress = group.First().BusinessCustomer.StreetAddress,
                    Postnummer = group.First().BusinessCustomer.ZipCode,
                    Ort = group.First().BusinessCustomer.City,
                    Försäkringar = group.Select(bi =>
                    {
                        if (bi is LiabilityInsurance) return "Ansvarsförsäkring";
                        else if (bi is RealEstateInsurance) return "Fastighetsförsäkring";
                        else if (bi is VehicleInsurance) return "Fordonsförsäkring";
                        else return "Okänd försäkringstyp";
                    }).ToList(),
                    TotalPremie = group.Sum(bi => bi.Premium)
                }).ToList();

            // Kombinera data för privata och företagsfakturor
            var invoiceData = new
            {
                PrivatFakturor = privateInvoiceData,
                FöretagsFakturor = businessInvoiceData
            };

            // Definiera sökvägen för fakturafilen på skrivbordet
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "invoiceFile.json");

            // Ange JSON-format med indentering för läsbarhet
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            // Konvertera fakturadatan till en JSON-sträng
            string jsonString = JsonSerializer.Serialize(invoiceData, jsonOptions);

            // Skriv JSON-strängen till en fil
            File.WriteAllText(filePath, jsonString);

            // Visa ett meddelande till användaren om att filen har skapats
            MessageBox.Show($"Fakturafilen har skapats och sparats som JSON på skrivbordet: {filePath}");
        }
    }
}
