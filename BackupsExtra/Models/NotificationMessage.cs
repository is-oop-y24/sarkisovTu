using System;

namespace BackupsExtra.Models
{
    public class NotificationMessage
    {
        public NotificationMessage(BackupsJobService senderService, DateTime date, string message)
        {
            SenderService = senderService;
            Date = date;
            Message = message;
        }

        public BackupsJobService SenderService { get; private set; }
        public DateTime Date { get; private set; }
        public string Message { get; private set; }
    }
}