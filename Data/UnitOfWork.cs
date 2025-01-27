using Data.Interfaces;
using Entities;

namespace Data.Repositories
{
    public class UnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepositoryBase<User> UserRepository { get; private set; }
        public IRepositoryBase<Employee> EmployeeRepository { get; private set; }
        public IRepositoryBase<BusinessCustomer> BusinessCustomerRepository { get; private set; }
        public IRepositoryBase<PrivateCustomer> PrivateCustomerRepository { get; private set; }
        public IRepositoryBase<Role> RoleRepository { get; private set; }
        public IRepositoryBase<EmployeeRole> EmployeeRoleRepository { get; private set; }
        public IRepositoryBase<BusinessInsurance> BusinessInsuranceRepository { get; private set; }
        public IRepositoryBase<PrivateInsurance> PrivateInsuranceRepository { get; private set; }
        public IRepositoryBase<PaymentPlan> PaymentPlanRepository { get; private set; }
        public IRepositoryBase<Insured> InsuredRepository { get; private set; }
        public IRepositoryBase<PrivateInsuranceOptionalExtras> OptionalInsuranceExtras { get; private set; }
        public IRepositoryBase<Zone> ZoneRepository { get; private set; }
        public IRepositoryBase<City> CityRepository { get; private set; }
        public IRepositoryBase<VehicleInsurance> VehicleRepository { get; private set; }
        public IRepositoryBase<RealEstateInsurance> RealestateRepository { get; private set; }
        public IRepositoryBase<CustomerProspectInformation> CustomerProspectInformationRepository { get; private set; }
        public IRepositoryBase<LiabilityInsurance> LiabilityRepository { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            UserRepository = new RepositoryBase<User>(_context);
            EmployeeRepository = new RepositoryBase<Employee>(_context);
            BusinessCustomerRepository = new RepositoryBase<BusinessCustomer>(_context);
            PrivateCustomerRepository = new RepositoryBase<PrivateCustomer>(_context);
            RoleRepository = new RepositoryBase<Role>(_context);
            EmployeeRoleRepository = new RepositoryBase<EmployeeRole>(_context);
            BusinessInsuranceRepository = new RepositoryBase<BusinessInsurance>(_context);
            PrivateInsuranceRepository = new RepositoryBase<PrivateInsurance>(_context);
            PaymentPlanRepository = new RepositoryBase<PaymentPlan>(_context);
            InsuredRepository = new RepositoryBase<Insured>(_context);
            OptionalInsuranceExtras = new RepositoryBase<PrivateInsuranceOptionalExtras>(_context);
            ZoneRepository = new RepositoryBase<Zone>(_context);
            CityRepository = new RepositoryBase<City>(_context);
            VehicleRepository = new RepositoryBase<VehicleInsurance>(_context);
            RealestateRepository = new RepositoryBase<RealEstateInsurance>(_context);
            CustomerProspectInformationRepository = new RepositoryBase<CustomerProspectInformation>(_context);
            LiabilityRepository = new RepositoryBase<LiabilityInsurance>(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

