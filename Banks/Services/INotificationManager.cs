using System.Collections.Generic;
using Banks.Models;

namespace Banks.Services
{
    public interface INotificationManager
    {
        void CreateNotificationClient(BankClient client);

        void RemoveNotificationClient(BankClient client);

        bool IsSubscribedClient(BankClient client);

        void SendNotificationToClient(BankClient client, Notification notification);

        List<Notification> GetAllNotifications(BankClient client);

        List<Notification> GetUncheckedNotifications(BankClient client);
    }
}