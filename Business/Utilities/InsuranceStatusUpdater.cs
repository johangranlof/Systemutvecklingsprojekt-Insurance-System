// InsuranceStatusUpdater-klassen ansvarar för att kontrollera och uppdatera status för försäkringar som har gått ut.
// Om en försäkring har passerat sitt slutdatum markeras den som "utgått" (InsuranceStatusId = 2).

using Business.Controllers;
using Entities;

namespace Business.Utilities
{
    public class InsuranceStatusUpdater
    {
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly BusinessInsuranceController _businessInsuranceController;

        public InsuranceStatusUpdater()
        {
            _privateInsuranceController = new PrivateInsuranceController();
            _businessInsuranceController = new BusinessInsuranceController();
        }

        public void UpdateExpiredInsurances()
        {
            UpdatePrivateInsurances();
            UpdateBusinessInsurances();
        }

        private void UpdatePrivateInsurances()
        {
            var privateInsurances = _privateInsuranceController.GetAllPrivateInsurances();

            foreach (var insurance in privateInsurances)
            {
                if (HasInsuranceExpired(insurance))
                {
                    insurance.InsuranceStatusId = 2;
                    _privateInsuranceController.UpdatePrivateInsurance(insurance);
                }
            }
        }

        private void UpdateBusinessInsurances()
        {
            var businessInsurances = _businessInsuranceController.GetAllBusinessInsurances();

            foreach (var insurance in businessInsurances)
            {
                if (HasInsuranceExpired(insurance))
                {
                    insurance.InsuranceStatusId = 2;
                    _businessInsuranceController.UpdateBusinessInsurance(insurance);
                }
            }
        }

        private bool HasInsuranceExpired(PrivateInsurance insurance)
        {
            return insurance.EndDate < DateTime.Now;
        }

        private bool HasInsuranceExpired(BusinessInsurance insurance)
        {
            return insurance.EndDate < DateTime.Now;
        }
    }
}
