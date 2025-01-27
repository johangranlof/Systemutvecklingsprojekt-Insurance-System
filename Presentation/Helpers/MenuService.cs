// MenuService-klassen genererar en lista av menyobjekt baserat på användarens roll.
// Varje roll har specifika menyalternativ och kommandon som användaren får tillgång till i applikationen.

using MaterialDesignThemes.Wpf;
using Presentation.Helpers;
using System.Windows.Input;

public class MenuService
{
    // Metoden genererar en lista av menyobjekt beroende på användarens roll
    public List<MenuObject> GetMenuItemsForRole(string permission,
        ICommand HandleEmployeeCommand,
        ICommand HandleComissionCommand,
        ICommand HandleInsurancesCommand,
        ICommand HandleCustomerCommand,
        ICommand AnalyzeSalesStatisticsCommand)
    {
        var items = new List<MenuObject>();

        if (permission == "Admin")
        {
            items.Add(new MenuObject("Anställd", HandleEmployeeCommand, PackIconKind.AccountGroup));
        }
        else if (permission == "Ekonomiassistent")
        {
            // Ekonomiassistent får tillgång till menyalternativen "Provision", "Försäkring", och "Kund"
            items.Add(new MenuObject("Provision", HandleComissionCommand, PackIconKind.AccountCash));
            items.Add(new MenuObject("Försäkring", HandleInsurancesCommand, PackIconKind.ShieldOutline));
            items.Add(new MenuObject("Kund", HandleCustomerCommand, PackIconKind.AccountSupervisor));
        }
        else if (permission == "Försäljningsassistent")
        {
            // Försäljningsassistent har tillgång till "Försäkring" och "Kund"
            items.Add(new MenuObject("Försäkring", HandleInsurancesCommand, PackIconKind.ShieldOutline));
            items.Add(new MenuObject("Kund", HandleCustomerCommand, PackIconKind.AccountSupervisor));
        }
        else if (permission == "Säljare")
        {
            // Säljare har tillgång till "Försäkring" och "Kund"
            items.Add(new MenuObject("Försäkring", HandleInsurancesCommand, PackIconKind.ShieldOutline));
            items.Add(new MenuObject("Kund", HandleCustomerCommand, PackIconKind.AccountSupervisor));
        }
        else if (permission == "VD")
        {
            // VD har tillgång till "Försäljningsstatistik", "Försäkring", och "Kund"
            items.Add(new MenuObject("Försäljningsstatistik", AnalyzeSalesStatisticsCommand, PackIconKind.ChartLine));
            items.Add(new MenuObject("Försäkring", HandleInsurancesCommand, PackIconKind.ShieldOutline));
            items.Add(new MenuObject("Kund", HandleCustomerCommand, PackIconKind.AccountSupervisor));
        }
        else if (permission == "Försäljningschef")
        {
            // Försäljningschef har tillgång till "Försäljningsstatistik", "Försäkring", och "Kund"
            items.Add(new MenuObject("Försäljningsstatistik", AnalyzeSalesStatisticsCommand, PackIconKind.ChartLine));
            items.Add(new MenuObject("Försäkring", HandleInsurancesCommand, PackIconKind.ShieldOutline));
            items.Add(new MenuObject("Kund", HandleCustomerCommand, PackIconKind.AccountSupervisor));
        }

        return items;
    }
}
