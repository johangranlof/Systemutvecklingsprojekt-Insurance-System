using Data;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Controllers
{
    public class BusinessInsuranceController
    {
        private readonly UnitOfWork _unitOfWork;

        public BusinessInsuranceController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }

        public List<BusinessInsurance> GetAllBusinessInsurances()
        {
            return _unitOfWork.BusinessInsuranceRepository
                .GetAll()
                .Include(bi => bi.PaymentPlan)
                .Include(bi => bi.InsuranceStatus)
                .Include(bi => bi.User)
                .Include(bi => bi.User.Employee)
                .Include(bi => bi.User.Employee.EmployeeRoles)
                .Include(bi => bi.BusinessCustomer)
                .ToList();
        }

        public List<BusinessInsurance> GetActiveBusinessInsurances()
        {
            return _unitOfWork.BusinessInsuranceRepository
                .GetAll()
                .Where(bi => bi.InsuranceStatusId == 1)
                .Include(bi => bi.PaymentPlan)
                .Include(bi => bi.InsuranceStatus)
                .Include(bi => bi.User)
                .Include(bi => bi.User.Employee)
                .Include(bi => bi.BusinessCustomer)
                .ToList();
        }
        public List<City> GetAllCities()
        {
            return _unitOfWork.CityRepository.GetAll().ToList();
        }

        public BusinessInsurance GetBusinessInsuranceById(int id)
        {
            return _unitOfWork.BusinessInsuranceRepository.GetById(id);
        }

        public void UpdateBusinessInsurance(BusinessInsurance businessInsurance)
        {
            _unitOfWork.BusinessInsuranceRepository.Update(businessInsurance);
            _unitOfWork.Save();
        }

        public void RemoveBusinessInsurance(BusinessInsurance businessInsurance)
        {
            _unitOfWork.BusinessInsuranceRepository.Delete(businessInsurance);
            _unitOfWork.Save();
        }

        public bool IsInsuranceNumberExists(string insuranceNumber)
        {
            return _unitOfWork.VehicleRepository.GetAll().Any(v => v.InsuranceNumber == insuranceNumber);
        }

        public void AddVehicleInsurance(VehicleInsurance VehicleInsurance)
        {
            _unitOfWork.VehicleRepository.Add(VehicleInsurance);
            _unitOfWork.Save();
        }

        public List<VehicleInsurance> GetVehicleInsurancesByBusinessCustomerId(int businessCustomerId)
        {
            return _unitOfWork.VehicleRepository
                .FindByCondition(vi => vi.BusinessCustomerId == businessCustomerId)
                .AsQueryable()
                .Include(vi => vi.BusinessCustomer)
                .Include(vi => vi.PaymentPlan)
                .Include(vi => vi.Zone)
                .ToList();
        }

        public List<RealEstateInsurance> GetRealEstateInsurancesByBusinessCustomerId(int businessCustomerId)
        {
            return _unitOfWork.RealestateRepository
                .FindByCondition(vi => vi.BusinessCustomerId == businessCustomerId)
                .AsQueryable()
                .Include(vi => vi.BusinessCustomer)
                .Include(vi => vi.PaymentPlan)

                .ToList();
        }

        public List<LiabilityInsurance> GetLiabilityInsurancesByBusinessCustomerId(int businessCustomerId)
        {
            return _unitOfWork.LiabilityRepository
                .FindByCondition(vi => vi.BusinessCustomerId == businessCustomerId)
                .AsQueryable()
                .Include(vi => vi.BusinessCustomer)
                .Include(vi => vi.PaymentPlan)

                .ToList();
        }

        public void AddRealEstateInsurance(RealEstateInsurance realEstate)
        {
            _unitOfWork.RealestateRepository.Add(realEstate);
            _unitOfWork.Save();
        }

        public void AddBusinessInsurance(BusinessInsurance businessInsurance)
        {
            _unitOfWork.BusinessInsuranceRepository.Add(businessInsurance);
            _unitOfWork.Save();
        }
    }
}
