using Data;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Business.Controllers
{
    public class BusinessCustomerController
    {
        private readonly UnitOfWork _unitOfWork;

        public BusinessCustomerController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }
        public BusinessCustomer AddBusinessCustomer(BusinessCustomer businessCustomer)
        {
            _unitOfWork.BusinessCustomerRepository.Add(businessCustomer);
            _unitOfWork.Save();
            return businessCustomer;
        }
        public bool HasActiveInsurance(BusinessCustomer businessCustomer)
        {
            return businessCustomer.BusinessInsurances != null
                   && businessCustomer.BusinessInsurances.Any(insurance => insurance.InsuranceStatusId == 1);
        }
        public BusinessCustomer UpdateBusinessCustomer(BusinessCustomer businessCustomer)
        {
            _unitOfWork.BusinessCustomerRepository.Update(businessCustomer);
            _unitOfWork.Save();
            return businessCustomer;

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
        public void DeleteBusinessCustomer(BusinessCustomer businessCustomer)
        {
            _unitOfWork.BusinessCustomerRepository.Delete(businessCustomer);
            _unitOfWork.Save();
        }
        public bool IsOrganisationNumberExists(string organisationNumber)
        {
            return _unitOfWork.BusinessCustomerRepository
                .FindByCondition(bc => bc.OrganisationNumber == organisationNumber)
                .Any();
        }
        public List<BusinessCustomer> GetBusinessCustomers()
        {
            var businessCustomers = _unitOfWork.BusinessCustomerRepository.GetAll()
               .Include(bc => bc.BusinessInsurances)
               .Select(bc => new BusinessCustomer
               {
                   Id = bc.Id,
                   CompanyName = bc.CompanyName,
                   OrganisationNumber = bc.OrganisationNumber,
                   BusinessInsurances = bc.BusinessInsurances
               }).ToList();

            return businessCustomers;
        }
        public BusinessCustomer GetBusinessCustomerByID(int id)
        {
            var businessCustomerWithPhonesAndEmails = _unitOfWork.BusinessCustomerRepository.GetAll()
               .Where(bc => bc.Id == id)
               .FirstOrDefault();

            return businessCustomerWithPhonesAndEmails;
        }
        public ObservableCollection<CustomerProspectInformation> GetCustomerProspectInformation(int businessCustomerId)
        {
            var prospectInformationList = _unitOfWork.CustomerProspectInformationRepository
                .FindByCondition(cpi => cpi.CustomerId == businessCustomerId)
                .ToList();

            return new ObservableCollection<CustomerProspectInformation>(prospectInformationList);
        }
        public void AddProspectInformation(CustomerProspectInformation customerProspectInformation)
        {
            _unitOfWork.CustomerProspectInformationRepository.Add(customerProspectInformation);
            _unitOfWork.Save();
        }

    }
}




