using System.Collections.Generic;
using System.Linq;
using Banks.Models;
using Banks.Tools;

namespace Banks.Services
{
    public class ConsoleAppNotificationManager : INotificationManager
    {
        private Dictionary<BankClient, NotificationStorage> _notificationStorages;

        public ConsoleAppNotificationManager()
        {
            _notificationStorages = new Dictionary<BankClient, NotificationStorage>();
        }

        public void CreateNotificationClient(BankClient client)
        {
            _notificationStorages.Add(client, new NotificationStorage());
        }

        public void RemoveNotificationClient(BankClient client)
        {
            _notificationStorages.Remove(client);
        }

        public bool IsSubscribedClient(BankClient client)
        {
            if (_notificationStorages.ContainsKey(client)) return true;
            return false;
        }

        public void SendNotificationToClient(BankClient client, Notification notification)
        {
            if (!_notificationStorages.ContainsKey(client)) throw new BanksException("Provided client isn't subscribed for notifications");
            NotificationStorage queryStorage = _notificationStorages[client];
            queryStorage.AddNewNotification(notification);
        }

        public List<Notification> GetAllNotifications(BankClient client)
        {
            if (!_notificationStorages.ContainsKey(client)) throw new BanksException("Provided client isn't subscribed for notifications");
            List<Notification> allNotifications = new List<Notification>();
            NotificationStorage queryStorage = _notificationStorages[client];
            allNotifications = allNotifications.Concat(queryStorage.CheckedNotification).ToList().Concat(queryStorage.UncheckedNotifications).Reverse().ToList();
            return allNotifications;
        }

        public List<Notification> GetUncheckedNotifications(BankClient client)
        {
            if (!_notificationStorages.ContainsKey(client)) throw new BanksException("Provided client isn't subscribed for notifications");
            List<Notification> uncheckedNotifications = new List<Notification>();
            NotificationStorage queryStorage = _notificationStorages[client];
            uncheckedNotifications = uncheckedNotifications.Concat(queryStorage.UncheckedNotifications).Reverse().ToList();
            queryStorage.MarkNotificationsAsChecked();
            return uncheckedNotifications;
        }
    }
}