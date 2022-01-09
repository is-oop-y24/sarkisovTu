using System.Collections.Generic;

namespace Banks.Models
{
    public interface IBankObserver
    {
        void Update(Notification message);

        List<Notification> GetAllUpdates();

        BankClient GetBankClientRef();
    }
}