using System;

namespace BackupsExtra.Models
{
    public class ConsoleNotificationClient : INotificationClient
    {
        public void Update(NotificationMessage message)
        {
            string notificationMessage = $"[{message.Date.ToString("MM/dd/yyyy HH:mm:ss")}] - {message.Message}";
            Console.WriteLine(notificationMessage);
        }
    }
}