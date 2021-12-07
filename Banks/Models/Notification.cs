using System;

namespace Banks.Models
{
    public class Notification
    {
        public Notification(string message)
        {
            Date = DateTime.Now;
            Message = message;
        }

        public DateTime Date { get; private set; }
        public string Message { get; private set; }
    }
}