using System.Collections.Generic;

namespace Banks.Models
{
    public class NotificationStorage
    {
        public NotificationStorage()
        {
            CheckedNotification = new List<Notification>();
            UncheckedNotifications = new List<Notification>();
        }

        public List<Notification> CheckedNotification { get; private set; }
        public List<Notification> UncheckedNotifications { get; private set; }

        public void MarkNotificationsAsChecked()
        {
            CheckedNotification.AddRange(UncheckedNotifications);
            UncheckedNotifications.Clear();
        }

        public void AddNewNotification(Notification notification)
        {
            UncheckedNotifications.Add(notification);
        }
    }
}