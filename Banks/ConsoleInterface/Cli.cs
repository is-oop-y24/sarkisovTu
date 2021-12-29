using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Models;
using Banks.Services;
using Banks.Types;
using static System.Console;

namespace Banks.ConsoleInterface
{
    public class Cli
    {
        private CentralBankService _centralBank;

        public Cli(CentralBankService centralBank)
        {
            _centralBank = centralBank;
        }

        public void Start()
        {
            RunMainMenu();
        }

        public void DisplayNotification(Bank bank, BankClient client)
        {
            RunNotificationPanelMenu(bank, client);
        }

        private void RunMainMenu()
        {
            Clear();
            List<string> options = new List<string>() { "CentralBank", "Banks", "Exit" };
            ConsoleMenu mainMenu = new ConsoleMenu("Welcome to Banks system", options);
            int selectedIndex = mainMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    RunCentralBankMenu();
                    break;
                case 1:
                    RunBanksBrowserMenu();
                    break;
                case 2:
                    ExitSystem();
                    break;
            }
        }

        private void RunCentralBankMenu()
        {
            Clear();
            List<string> options = new List<string>() { "Create new bank", "Toggle debit bonus", "Toggle deposit bonus", "Toggle credit fee", "Return to main menu" };
            ConsoleMenu centralBankMenu = new ConsoleMenu("Central bank section", options);
            int selectedIndex = centralBankMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    RunNewBankMenu();
                    break;
                case 1:
                    _centralBank.ToggleRemainBonus();
                    RunMainMenu();
                    break;
                case 2:
                    _centralBank.ToggleDepositBonus();
                    RunMainMenu();
                    break;
                case 3:
                    _centralBank.ToggleCommissionFee();
                    RunMainMenu();
                    break;
                case 4:
                    RunMainMenu();
                    break;
            }
        }

        private void RunNewBankMenu()
        {
            Clear();
            WriteLine("Create new bank form");
            WriteLine("New bank name: ");
            string name = ReadLine();
            BankConfiguration bankConfiguration = RunBankConfigurationForm();
            _centralBank.CreateBank(name, bankConfiguration);
            RunMainMenu();
        }

        private void RunBanksBrowserMenu()
        {
            Clear();
            List<string> options = _centralBank.Banks.Keys.Select(bank => bank.Name).ToList();
            options.Add("Return to main menu");
            int lastIndexOfMenu = options.Count - 1;
            ConsoleMenu centalBankMenu = new ConsoleMenu("Banks section", options);
            int selectedIndex = centalBankMenu.Run();

            if (selectedIndex == lastIndexOfMenu)
            {
                RunMainMenu();
            }
            else
            {
                RunBankMenu(_centralBank.Banks.Keys.ToList()[selectedIndex]);
            }
        }

        private void RunBankMenu(Bank bank)
        {
            Clear();
            List<string> options = new List<string>() { "Browse clients", "Bank configuration properties", "Create bank account", "Create client", "Return to banks browser" };
            ConsoleMenu bankMenu = new ConsoleMenu($"{bank.Name} panel", options);
            int selectedIndex = bankMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    RunClientBrowserMenu(bank);
                    break;
                case 1:
                    RunBankConfigurationPropertiesMenu(bank);
                    break;
                case 2:
                    RunChooseNewAccountClientMenu(bank);
                    break;
                case 3:
                    RunChooseClientTypeMenu(bank);
                    break;
                case 4:
                    RunBanksBrowserMenu();
                    break;
            }
        }

        private void RunBankConfigurationPropertiesMenu(Bank bank)
        {
            Clear();
            List<string> options = new List<string>() { "Change configuration properties", "Return to bank menu" };
            string depositProfitTable = string.Join(
                "; ",
                bank.BankConfigurationProperties.DepositAccountProfitTable.Select(pair => string.Format("{0} - {1}%", pair.Key.GetStringRange(), pair.Value.ToString())).ToArray());
            string menuHeader = $"Bank configuration properties \n" +
                                $" Not verified withdraw daily limit: {bank.BankConfigurationProperties.NotVerifiedWithdrawLimitDaily}\n" +
                                $" Not verified send daily limit: {bank.BankConfigurationProperties.NotVerifiedSendLimitDaily}\n" +
                                $" Debit account annual percent profit: {bank.BankConfigurationProperties.DebitAccountPercentProfit}\n" +
                                $" Available deposit locked periods in days: {string.Join(", ", bank.BankConfigurationProperties.AvailableDepositAccountLockedDays.ToArray())}\n" +
                                $" Deposit profit table: {depositProfitTable}\n" +
                                $" Credit account daily fee: {bank.BankConfigurationProperties.CreditAccountFeeAmountDaily}\n" +
                                $" Credit account lower balance limit: {bank.BankConfigurationProperties.CreditAccountLowerLimit}\n";
            ConsoleMenu bankConfigurationMenu = new ConsoleMenu(menuHeader, options);
            int selectedIndex = bankConfigurationMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    RunChangeConfigurationPropertiesMenu(bank);
                    break;
                case 1:
                    RunBankMenu(bank);
                    break;
            }
        }

        private void RunChangeConfigurationPropertiesMenu(Bank bank)
        {
            Clear();
            WriteLine("Change configuration properties form");
            List<string> options = new List<string>() { "Confirm changes", "Return to configuration page" };
            ConsoleMenu changeConfigurationMenu = new ConsoleMenu("Confirm changes?", options);
            BankConfiguration newConfiguration = RunBankConfigurationForm();
            int selectedIndex = changeConfigurationMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    bank.ChangeBankConfiguration(newConfiguration);
                    RunBankConfigurationPropertiesMenu(bank);
                    break;
                case 1:
                    RunBankConfigurationPropertiesMenu(bank);
                    break;
            }
        }

        private void RunChooseNewAccountClientMenu(Bank bank)
        {
            Clear();
            List<string> options = bank.Clients.Select(client => client.Id + " " + client.Name).ToList();
            options.Add("Return to bank menu");
            int lastIndexOfMenu = options.Count - 1;
            ConsoleMenu newAccountClientMenu = new ConsoleMenu($"Choose client", options);
            int selectedIndex = newAccountClientMenu.Run();

            if (selectedIndex == lastIndexOfMenu)
            {
                RunBankMenu(bank);
            }
            else
            {
                RunChooseNewAccountTypeMenu(bank, bank.Clients[selectedIndex]);
            }
        }

        private void RunChooseNewAccountTypeMenu(Bank bank, BankClient client)
        {
            List<string> options = new List<string>() { "Debit account", "Deposit account", "CreditAccount", "Return to bank menu" };
            ConsoleMenu newAccountTypeMenu = new ConsoleMenu("Choose new account type", options);
            int selectedIndex = newAccountTypeMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    bank.CreateDebitAccount(client);
                    RunBankMenu(bank);
                    break;
                case 1:
                {
                    WriteLine("Initial deposit amount: ");
                    string initialSum = ReadLine();
                    WriteLine("Locked days period(For more information check bank configuration): ");
                    string lockedPeriod = ReadLine();
                    bank.CreateDepositAccount(client, double.Parse(initialSum), int.Parse(lockedPeriod));
                    RunBankMenu(bank);
                    break;
                }

                case 2:
                    bank.CreateCreditAccount(client);
                    RunBankMenu(bank);
                    break;
                case 3:
                    RunBankMenu(bank);
                    break;
            }
        }

        private void RunChooseClientTypeMenu(Bank bank)
        {
            Clear();
            List<string> options = new List<string>() { "Verified", "Unverified", "Return to bank menu" };
            ConsoleMenu chooseClientMenu = new ConsoleMenu("Choose client type", options);
            int selectedIndex = chooseClientMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    RunVerifiedClientCreationMenu(bank);
                    break;
                case 1:
                    RunUnverifiedClientCreationMenu(bank);
                    break;
                case 2:
                    RunBankMenu(bank);
                    break;
            }
        }

        private void RunClientBrowserMenu(Bank bank)
        {
            Clear();
            List<string> options = bank.Clients.Select(client => client.Id + " " + client.Name).ToList();
            options.Add("Return to bank menu");
            int lastIndexOfMenu = options.Count - 1;
            ConsoleMenu clientBrowserMenu = new ConsoleMenu("Client browser", options);
            int selectedIndex = clientBrowserMenu.Run();

            if (selectedIndex == lastIndexOfMenu)
            {
                RunBankMenu(bank);
            }
            else
            {
                RunClientPersonalMenu(bank, bank.Clients[selectedIndex]);
            }
        }

        private void RunClientPersonalMenu(Bank bank, BankClient client)
        {
            Clear();
            List<string> options = new List<string>() { "Accounts list", "Return to client browser" };
            bool subscribedClientMenu = false;
            if (bank.IsClientSubscribed(client))
            {
                subscribedClientMenu = true;
                options.Insert(0, "Unsubscribe from notifications");
                options.Insert(0, "Client notifications");
            }
            else
            {
                options.Insert(0, "Subscribe to personal notifications");
            }

            int lastIndexOfMenu = options.Count - 1;
            string menuHeader = $"Client information \n ID: {client.Id} \n Name: {client.Name} \n Surname: {client.Surname} \n Verification properties number: {client.KycDictionary.Count} \n";
            ConsoleMenu personalClientMenu = new ConsoleMenu(menuHeader, options);
            int selectedIndex = personalClientMenu.Run();

            if (subscribedClientMenu)
            {
                switch (selectedIndex)
                {
                    case 0:
                        RunNotificationPanelMenu(bank, client);
                        break;
                    case 1:
                        bank.UnsubscribeClientFromNotifications(bank.GetObserverByClient(client));
                        RunClientPersonalMenu(bank, client);
                        break;
                    case 2:
                        RunAccountsListMenu(bank, client);
                        break;
                }
            }
            else
            {
                switch (selectedIndex)
                {
                    case 0:
                        bank.SubscribeClientForNotifications(new ConsoleAppObserver(this, bank, client));
                        RunClientPersonalMenu(bank, client);
                        break;
                    case 1:
                        RunAccountsListMenu(bank, client);
                        break;
                }
            }

            if (selectedIndex == lastIndexOfMenu) RunClientBrowserMenu(bank);
        }

        private void RunAccountsListMenu(Bank bank, BankClient client)
        {
            Clear();
            List<string> allAccountsList = bank.AccountManager.GetDebitAccounts(bank).FindAll(account => account.BankClientRef == client).Select(account => account.Id + " " + account.AccountType + " Balance: " + account.CalculateCurrentBalance()).ToList();
            List<string> depositAccountsList = bank.AccountManager.GetDepositAccounts(bank).FindAll(account => account.BankClientRef == client).Select(account => account.Id + " " + account.AccountType + " Balance: " + account.CalculateCurrentBalance()).ToList();
            List<string> creditAccountsList = bank.AccountManager.GetCreditAccounts(bank).FindAll(account => account.BankClientRef == client).Select(account => account.Id + " " + account.AccountType + " Balance: " + account.CalculateCurrentBalance()).ToList();
            allAccountsList.AddRange(depositAccountsList);
            allAccountsList.AddRange(creditAccountsList);

            string menuHeader = "Client all accounts list\n";
            foreach (string account in allAccountsList)
            {
                menuHeader += $" {account}\n";
            }

            List<string> options = new List<string>() { "Return to client page" };
            ConsoleMenu accountListMenu = new ConsoleMenu(menuHeader, options);
            int selectedIndex = accountListMenu.Run();

            if (selectedIndex == 0) RunClientPersonalMenu(bank, client);
        }

        private void RunNotificationPanelMenu(Bank bank, BankClient client)
        {
            Clear();
            List<Notification> notifications = bank.GetAllNotifications(bank.GetObserverByClient(client));
            string menuHeader = "Realtime notifications of client \n";
            if (notifications.Count == 0)
            {
                menuHeader += "Client hasn't any notifications \n";
            }
            else
            {
                foreach (Notification notification in notifications)
                {
                    menuHeader += notification.Date.ToShortDateString() + " Message: " + notification.Message + "\n";
                }
            }

            List<string> options = new List<string>() { "Return to client page" };
            ConsoleMenu notificationMenu = new ConsoleMenu(menuHeader, options);
            int selectedIndex = notificationMenu.Run();

            if (selectedIndex == 0) RunClientPersonalMenu(bank, client);
        }

        private void RunVerifiedClientCreationMenu(Bank bank)
        {
            Clear();
            WriteLine("Create verified client form");
            WriteLine("Name: ");
            string name = ReadLine();
            WriteLine("Surname: ");
            string surname = ReadLine();
            List<string> options = new List<string>();
            List<KycProperty> kycProperties = new List<KycProperty>();
            for (int i = 0; i < Enum.GetNames(typeof(KycType)).Length; i++)
            {
                KycType type = (KycType)i;
                options.Add(type.ToString());
            }

            while (kycProperties.Count < options.Count + 1)
            {
                ConsoleMenu verificationPropertyMenu = new ConsoleMenu("Choose verification method", options);
                int selectedIndex = verificationPropertyMenu.Run();
                string selectedOption = options[selectedIndex];
                if (selectedIndex == options.Count - 1 && kycProperties.Count >= 1) break;
                int kycIndex = (int)Enum.Parse(typeof(KycType), selectedOption);
                KycType selectedType = (KycType)kycIndex;
                WriteLine($"{selectedOption}: ");
                string verificationProperty = ReadLine();
                kycProperties.Add(new KycProperty((KycType)kycIndex, verificationProperty));
                options.Remove(selectedType.ToString());
                if (kycProperties.Count == 1) options.Add("Create verified client");
            }

            bank.CreateVerifiedClient(name, surname, kycProperties);
            RunBankMenu(bank);
        }

        private void RunUnverifiedClientCreationMenu(Bank bank)
        {
            Clear();
            WriteLine("Create unverified client form");
            WriteLine("Name: ");
            string name = ReadLine();
            WriteLine("Surname: ");
            string surname = ReadLine();
            bank.CreateBaseClient(name, surname);
            RunBankMenu(bank);
        }

        private void ExitSystem()
        {
            WriteLine("Press any key to exit");
            ReadKey(true);
            Environment.Exit(0);
        }

        private BankConfiguration RunBankConfigurationForm()
        {
            WriteLine("Not verified withdraw daily limit:");
            double notVerifiedWithdrawLimitDaily = Convert.ToDouble(ReadLine());
            WriteLine("Not verified send daily limit:");
            double notVerifiedSendLimitDaily = Convert.ToDouble(ReadLine());
            WriteLine("Debit account remain percent profit:");
            double debitAccountPercentProfit = Convert.ToDouble(ReadLine());
            WriteLine("Available deposit locked periods in days:");
            string availableDepositAccountLockedDaysString = ReadLine();
            List<int> availableDepositAccountLockedDays = availableDepositAccountLockedDaysString.Split(',').Select(int.Parse).ToList();
            WriteLine("Deposit profit table:");
            Dictionary<MoneyRange, double> depositAccountProfitTable = new Dictionary<MoneyRange, double>();
            string[] profitTableString = ReadLine().Split(";");
            for (int i = 0; i < profitTableString.Length; i++)
            {
                string[] profitRangeString = profitTableString[i].Split(" - ");
                MoneyRange moneyRange = new MoneyRange(Convert.ToDouble(profitRangeString[0]), Convert.ToDouble(profitRangeString[1]));
                double percentProfit = Convert.ToDouble(profitRangeString[2].Remove(profitRangeString[2].Length - 1));
                depositAccountProfitTable.Add(moneyRange, percentProfit);
            }

            WriteLine("Credit account daily fee:");
            double creditAccountFeeAmountDaily = Convert.ToDouble(ReadLine());
            WriteLine("Credit account lower balance limit:");
            double creditAccountLowerLimit = Convert.ToDouble(ReadLine());

            BankConfiguration newConfiguration = new BankConfiguration(
                notVerifiedWithdrawLimitDaily,
                notVerifiedSendLimitDaily,
                debitAccountPercentProfit,
                availableDepositAccountLockedDays,
                depositAccountProfitTable,
                creditAccountFeeAmountDaily,
                creditAccountLowerLimit);
            return newConfiguration;
        }
    }
}