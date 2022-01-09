using System.Collections.Generic;
using System.Linq;
using Banks.ConsoleInterface;

namespace Banks.Models
{
    public class ConsoleAppObserver : IBankObserver
    {
        public ConsoleAppObserver(Cli consoleInterface, Bank bankRef, BankClient bankClient)
        {
            ConsoleInterface = consoleInterface;
            BankRef = bankRef;
            BankClientRef = bankClient;
            Notifications = new List<Notification>();
        }

        public Cli ConsoleInterface { get; private set; }
        public Bank BankRef { get; private set; }
        public BankClient BankClientRef { get; private set; }
        public List<Notification> Notifications { get; private set; }

        public void Update(Notification message)
        {
            Notifications.Add(message);
            ConsoleInterface.DisplayNotification(BankRef, BankClientRef);
        }

        public List<Notification> GetAllUpdates()
        {
            return Enumerable.Reverse(Notifications).ToList();
        }

        public BankClient GetBankClientRef()
        {
            return BankClientRef;
        }
    }
}