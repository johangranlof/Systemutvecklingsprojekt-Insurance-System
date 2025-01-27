using Data;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Business.Controllers
{
    public class PrivateCustomerController
    {
        private readonly UnitOfWork _unitOfWork;
        public PrivateCustomerController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }

        public PrivateCustomer AddPrivateCustomer(PrivateCustomer privateCustomer)
        {
            _unitOfWork.PrivateCustomerRepository.Add(privateCustomer);
            _unitOfWork.Save();
            return privateCustomer;

        }

        public PrivateCustomer UpdatePrivateCustomer(PrivateCustomer privateCustomer)
        {
            _unitOfWork.PrivateCustomerRepository.Update(privateCustomer);
            _unitOfWork.Save();
            return privateCustomer;

        }

        public void DeletePrivateCustomer(PrivateCustomer privateCustomer)
        {

            _unitOfWork.PrivateCustomerRepository.Delete(privateCustomer);
            _unitOfWork.Save();

        }

        public List<PrivateCustomer> GetPrivateCustomers()

        {
            var privateCustomers = _unitOfWork.PrivateCustomerRepository.GetAll()

               .Include(pc => pc.PrivateInsurances)
               .Select(pc => new PrivateCustomer
               {
                   FirstName = pc.FirstName,
                   LastName = pc.LastName,
                   Id = pc.Id,
                   SSN = pc.SSN,
                   PrivateInsurances = pc.PrivateInsurances
               }).ToList();

            return privateCustomers;
        }

        public List<PaymentPlan> GetPaymentPlan()
        {
            var paymentplan = _unitOfWork.PaymentPlanRepository.GetAll()
               .Select(pc => new PaymentPlan
               {
                   Type = pc.Type,
                   Id = pc.Id,
               }).ToList();

            return paymentplan;
        }

        public void AddProspectInformation(CustomerProspectInformation customerProspectInformation)
        {
            _unitOfWork.CustomerProspectInformationRepository.Add(customerProspectInformation);
            _unitOfWork.Save();
        }

        public PrivateCustomer GetPrivateCustomerByID(int id)
        {
            var privatecustomerinfo = _unitOfWork.PrivateCustomerRepository.GetAll()
                  .Where(pc => pc.Id == id)
                  .FirstOrDefault();

            return privatecustomerinfo;
        }

        public bool IsSsnExists(string ssn)
        {
            return _unitOfWork.PrivateCustomerRepository
                .FindByCondition(pc => pc.SSN == ssn)
                .Any();
        }

        public ObservableCollection<CustomerProspectInformation> GetCustomerProspectInformation(int privateCustomerId)
        {
            var prospectInformationList = _unitOfWork.CustomerProspectInformationRepository
                .FindByCondition(cpi => cpi.CustomerId == privateCustomerId)
                .ToList();

            return new ObservableCollection<CustomerProspectInformation>(prospectInformationList);
        }

        public bool HasActiveInsurance(PrivateCustomer privateCustomer)
        {
            return privateCustomer.PrivateInsurances != null
                   && privateCustomer.PrivateInsurances.Any(insurance => insurance.InsuranceStatusId == 1);
        }
    }
}



