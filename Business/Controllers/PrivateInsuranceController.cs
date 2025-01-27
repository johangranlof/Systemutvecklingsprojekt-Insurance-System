using Data;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Controllers
{
    public class PrivateInsuranceController
    {
        private readonly UnitOfWork _unitOfWork;

        public PrivateInsuranceController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }

        public List<PrivateInsurance> GetAllPrivateInsurances()
        {
            return _unitOfWork.PrivateInsuranceRepository
                .GetAll()
                .Include(pi => pi.Insured)
                .Include(pi => pi.PrivateInsuranceOptionalExtras)
                .Include(pi => pi.PaymentPlan)
                .Include(pi => pi.PrivateCustomer)
                .Include(pi => pi.User)
                .Include(pi => pi.User.Employee)
                .Include(bi => bi.User.Employee.EmployeeRoles)
                .Include(pi => pi.InsuranceStatus)
                .ToList();
        }

        public List<PrivateInsurance> GetActivePrivateInsurances()
        {
            return _unitOfWork.PrivateInsuranceRepository
                .GetAll()
                .Where(pi => pi.InsuranceStatusId == 1)
                .Include(pi => pi.Insured)
                .Include(pi => pi.PrivateInsuranceOptionalExtras)
                .Include(pi => pi.PaymentPlan)
                .Include(pi => pi.PrivateCustomer)
                .Include(pi => pi.User)
                .Include(pi => pi.User.Employee)
                .Include(pi => pi.InsuranceStatus)
                .ToList();
        }

        public PrivateInsurance GetPrivateInsuranceById(int id)
        {
            return _unitOfWork.PrivateInsuranceRepository.GetById(id);
        }

        public Insured GetPrivateInsuredBySSN(string ssn)
        {
            var insured = _unitOfWork.InsuredRepository.GetAll()
                .Where(bc => bc.SSN == ssn)
                .Include(bc => bc.PrivateInsurances)
                .FirstOrDefault();

            return insured;
        }

        public bool IsInsuranceNumberExists(string insuranceNumber)
        {
            return _unitOfWork.VehicleRepository.GetAll().Any(v => v.InsuranceNumber == insuranceNumber);
        }
        public void UpdatePrivateInsurance(PrivateInsurance privateInsurance)
        {
            _unitOfWork.PrivateInsuranceRepository.Update(privateInsurance);
            _unitOfWork.Save();
        }

        public void RemovePrivateInsurance(PrivateInsurance privateInsurance)
        {
            _unitOfWork.PrivateInsuranceRepository.Delete(privateInsurance);
            _unitOfWork.Save();
        }

        public void AddPrivateInsurance(PrivateInsurance privateInsurance)
        {
            _unitOfWork.PrivateInsuranceRepository.Add(privateInsurance);
            _unitOfWork.Save();
        }

        public List<PrivateInsuranceOptionalExtras> GetPrivateOptionalInsurances(int insuranceId)
        {
            return _unitOfWork.OptionalInsuranceExtras
                .FindByCondition(oe => oe.PrivateInsuranceId == insuranceId)
                .ToList();
        }
    }
}
