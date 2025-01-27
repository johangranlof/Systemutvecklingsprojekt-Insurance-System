using Business.Controllers;
using Entities;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Presentation.Models;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Presentation.ViewModels
{
    public class AnalyzeSalesVM : BaseVM
    {
        private readonly MainVM _mainVM;
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly BusinessInsuranceController _businessInsuranceController;
        private readonly EmployeeController _employeeController;

        public ObservableCollection<ISeries> MonthlyTrendSeries { get; set; }
        public ObservableCollection<ISeries> SalesSeries { get; set; }
        public Axis[] XAxis { get; set; }
        public Axis[] YAxis { get; set; }
        public ObservableCollection<int> AvailableYears { get; set; }
        public ObservableCollection<Employee> Sellers { get; set; }
        public List<PrivateInsuranceSalesModel> PrivateSales { get; set; }
        public List<BusinessInsuranceSalesModel> BusinessSales { get; set; }

        private Employee selectedSeller;
        private int _selectedYear;
        private string _selectedFilter;
        private int _selectedAgentNumber;
        private double _averagePerMonth;
        private int _totalSold;

        public Employee SelectedSeller
        {
            get => selectedSeller;
            set
            {
                selectedSeller = value;
                SelectedAgentNumber = selectedSeller?.EmployeeRoles.FirstOrDefault(er => er.RoleId == 1)?.AgentNumber ?? 0;
                OnPropertyChanged();
                GetSoldInsurancesByMonth();
                LoadData();
            }
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                GetSoldInsurancesByMonth();
                LoadData();
                LoadAllSales();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                GetSoldInsurancesByMonth();
                LoadData();
            }
        }

        public int SelectedAgentNumber
        {
            get => _selectedAgentNumber;
            set
            {
                _selectedAgentNumber = value;
                OnPropertyChanged();
                GetSoldInsurancesByMonth();
            }
        }

        public double AveragePerMonth
        {
            get => _averagePerMonth;
            set
            {
                _averagePerMonth = value;
                OnPropertyChanged();
            }
        }

        public int TotalSold
        {
            get => _totalSold;
            set
            {
                _totalSold = value;
                OnPropertyChanged();
            }
        }

        public AnalyzeSalesVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            _businessInsuranceController = new BusinessInsuranceController();
            _privateInsuranceController = new PrivateInsuranceController();
            _employeeController = new EmployeeController();
            PrivateSales = new List<PrivateInsuranceSalesModel>();
            BusinessSales = new List<BusinessInsuranceSalesModel>();
            MonthlyTrendSeries = new ObservableCollection<ISeries>();
            SalesSeries = new ObservableCollection<ISeries>();
            Sellers = new ObservableCollection<Employee>();

            XAxis = new Axis[]
            {
                new Axis
                {
                    ShowSeparatorLines = false,
                    //Name = "Månad",
                    //LabelsRotation = 15,
                    //NameTextSize = 12,
                    TextSize = 12,

                    Labels = Enumerable.Range(1, 12)
                                       .Select(i => DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i))
                                       .ToArray(),
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray),
                }
            };

            YAxis = new Axis[]
            {
                new Axis
                {
                    ShowSeparatorLines = false,
                    TextSize = 12,
                    Labeler = value => value.ToString(),
                    MinLimit = 0,
                    MaxLimit = 50,
                }
            };

            AvailableYears = new ObservableCollection<int>(
                Enumerable.Range(DateTime.Now.Year - 9, 10).Reverse());

            SelectedYear = DateTime.Now.Year;
            SelectedFilter = "Alla";

            LoadSalesPeople();
            LoadAllSales();
            SelectedSeller = Sellers.FirstOrDefault();
        }

        private void LoadData()
        {
            TotalSold = GetTotalSold();
            AveragePerMonth = GetAveragePerMonth();
        }

        private void LoadAllSales()
        {
            var allPrivateInsurances = _privateInsuranceController.GetActivePrivateInsurances().Where(pi => pi.StartDate.Year == SelectedYear);
            var privateSalesList = allPrivateInsurances
                .GroupBy(insurance => $"{insurance.User?.Employee?.FirstName} {insurance.User?.Employee?.LastName}")
                .Select(group => new PrivateInsuranceSalesModel
                {
                    Seller = group.Key,
                    Children = group.Count(i => i.InsuranceType == "Sjukförsäkring barn"),
                    Adults = group.Count(i => i.InsuranceType == "Sjukförsäkring vuxen"),
                    Life = group.Count(i => i.InsuranceType == "Livförsäkring vuxen"),
                    Total = group.Count()
                })
                .ToList();

            PrivateSales = privateSalesList;

            var allBusinessInsurances = _businessInsuranceController.GetActiveBusinessInsurances().Where(bi => bi.StartDate.Year == SelectedYear);

            var businessSalesList = allBusinessInsurances
                .GroupBy(insurance => $"{insurance.User?.Employee?.FirstName} {insurance.User?.Employee?.LastName}")
                .Select(group => new BusinessInsuranceSalesModel
                {
                    Seller = group.Key,
                    Vehicles = group.Count(i => i.GetType() == typeof(VehicleInsurance)),
                    RealEstate = group.Count(i => i.GetType() == typeof(RealEstateInsurance)),
                    Liability = group.Count(i => i.GetType() == typeof(LiabilityInsurance)),
                    Total = group.Count()
                })
                .ToList();

            BusinessSales = businessSalesList;

            OnPropertyChanged(nameof(PrivateSales));
            OnPropertyChanged(nameof(BusinessSales));
        }

        private void LoadSalesPeople()
        {
            var allSellers = _employeeController.GetAllEmployeesWithSalesRole();
            Sellers.Clear();
            Sellers.Add(new Employee
            {
                EmployeeNumber = 0,
                FirstName = "Alla",
                LastName = "",
                EmployeeRoles = new List<EmployeeRole> { new EmployeeRole { RoleId = 1, AgentNumber = 0 } }
            });

            if (allSellers?.Any() != true)
            {
                Console.WriteLine("Inga säljare hittades.");
                return;
            }

            foreach (var person in allSellers.Where(person => person != null))
                Sellers.Add(person);
        }

        private void GetSoldInsurancesByMonth()
        {
            var selectedYearStart = new DateTime(SelectedYear, 1, 1);
            var allMonths = Enumerable.Range(0, 12)
                                      .Select(i => selectedYearStart.AddMonths(i).ToString("yyyy-MM"))
                                      .ToList();

            var privateInsurances = _privateInsuranceController.GetActivePrivateInsurances()
                .Where(ins => (ins.AgentNumber == SelectedAgentNumber && ins.InsuranceStatusId == 1) || SelectedAgentNumber == 0)
                .Select(ins => new InsuranceModel
                {
                    Id = ins.Id,
                    InsuranceNumber = ins.InsuranceNumber,
                    CustomerName = ins.PrivateCustomer.FirstName,
                    InsuranceType = ins.InsuranceType,
                    IsPrivateCustomer = true,
                    AgentNumber = ins.AgentNumber,
                    Status = ins.InsuranceStatusId.ToString(),
                    StartDate = ins.StartDate
                });

            var businessInsurances = _businessInsuranceController.GetActiveBusinessInsurances()
                .Where(ins => (ins.AgentNumber == SelectedAgentNumber && ins.InsuranceStatusId == 1) || SelectedAgentNumber == 0)
                .Select(ins => new InsuranceModel
                {
                    Id = ins.Id,
                    InsuranceNumber = ins.InsuranceNumber,
                    CustomerName = ins.BusinessCustomer.CompanyName,
                    InsuranceType = GetBusinessInsuranceType(ins),
                    IsPrivateCustomer = false,
                    AgentNumber = ins.AgentNumber,
                    Status = ins.InsuranceStatusId.ToString(),
                    StartDate = ins.StartDate
                });

            var relevantInsurances = SelectedFilter switch
            {
                "Alla" => privateInsurances.Concat(businessInsurances),
                "Privat" => privateInsurances,
                "Företag" => businessInsurances,
                _ => privateInsurances
            };

            var soldInsurancesByMonth = relevantInsurances
                .GroupBy(ins => ins.StartDate.ToString("yyyy-MM"))
                .ToDictionary(group => group.Key, group => group.Count());

            var salesData = allMonths.Select(month => new
            {
                Month = month,
                Count = soldInsurancesByMonth.ContainsKey(month) ? soldInsurancesByMonth[month] : 0
            });

            var maxSalesCount = salesData.Max(x => x.Count);
            YAxis[0].MaxLimit = maxSalesCount + (maxSalesCount * 0.1);

            SalesSeries.Clear();
            SalesSeries.Add(new LineSeries<int>
            {
                Values = salesData.Select(x => x.Count).ToList(),
                Stroke = new SolidColorPaint(SKColors.DarkBlue, 2),
            });

            MonthlyTrendSeries.Clear();
            MonthlyTrendSeries.Add(new ColumnSeries<int>
            {
                Values = salesData.Select(x => x.Count).ToList(),
                Fill = new SolidColorPaint(SKColors.LightBlue)
            });

            var trendValues = salesData.Select(x => (double)x.Count).ToList();

            MonthlyTrendSeries.Add(new LineSeries<double>
            {
                Values = trendValues,
                Stroke = new SolidColorPaint(SKColors.DarkBlue, 2),
                GeometrySize = 5,
                GeometryStroke = new SolidColorPaint(SKColors.LightGray, 2),
                LineSmoothness = 0,
            });

            XAxis[0].Labels = salesData
                .Select(x => DateTime.ParseExact(x.Month, "yyyy-MM", null).ToString("MMM", CultureInfo.CurrentCulture))
                .ToArray();
        }

        public int GetTotalSold()
        {
            if (SelectedAgentNumber == 0)
            {
                return _businessInsuranceController.GetActiveBusinessInsurances() // Om du har en liknande metod för aktiva affärsförsäkringar
                    .Count(bi => bi.StartDate.Year == SelectedYear &&
                                 (SelectedFilter == "Alla" || SelectedFilter == "Företag")) +
                    _privateInsuranceController.GetActivePrivateInsurances()
                        .Count(pi => pi.StartDate.Year == SelectedYear &&
                                     (SelectedFilter == "Alla" || SelectedFilter == "Privat"));
            }
            else
            {
                var businessSold = _businessInsuranceController.GetActiveBusinessInsurances()
                    .Count(bi => bi.StartDate.Year == SelectedYear &&
                                 bi.AgentNumber == SelectedAgentNumber && // Filtrera på agentnummer
                                 (SelectedFilter == "Alla" || SelectedFilter == "Företag"));

                var privateSold = _privateInsuranceController.GetActivePrivateInsurances()
                    .Count(pi => pi.StartDate.Year == SelectedYear &&
                                 pi.AgentNumber == SelectedAgentNumber && // Filtrera på agentnummer
                                 (SelectedFilter == "Alla" || SelectedFilter == "Privat"));

                return businessSold + privateSold;
            }
        }

        public double GetAveragePerMonth()
        {
            var totalSold = GetTotalSold();
            return totalSold == 0 ? 0.0 : Math.Round(totalSold / 12.0, 1);
        }

        private string GetBusinessInsuranceType(BusinessInsurance bi) =>
        bi switch
        {
            LiabilityInsurance => "Ansvarsförsäkring",
            RealEstateInsurance => "Fastighetsförsäkring",
            VehicleInsurance => "Fordonsförsäkring",
            _ => "N/A"
        };
    }
}
