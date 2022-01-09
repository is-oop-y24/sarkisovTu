using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Models
{
    public class Bank
    {
        public Bank(CentralBankService centralBank, string name)
        {
            CentralBankRef = centralBank;
            Name = name;
            Clients = new List<BankClient>();
            NotificationClients = new List<IBankObserver>();
        }

        public CentralBankService CentralBankRef { get; private set; }

        public List<IBankObserver> NotificationClients { get; private set; }
        public string Name { get; private set; }

        public List<BankClient> Clients { get; private set; }

        public BankConfiguration BankConfigurationProperties { get { return CentralBankRef.GetConfiguration(this); } }

        public AccountManager AccountManager { get { return CentralBankRef.AccountManager; } }

        public BankClient CreateBaseClient(string name, string surname)
        {
            int newClientId = Clients.Count;
            BankClient newClient = new BankClient.Builder(newClientId, name, surname).Build();
            Clients.Add(newClient);
            return newClient;
        }

        public BankClient CreateVerifiedClient(string name, string surname, List<KycProperty> kycProps)
        {
            int newClientId = Clients.Count;
            BankClient.Builder newClient = new BankClient.Builder(newClientId, name, surname);
            foreach (KycProperty kycProperty in kycProps)
            {
                newClient = newClient.SetNewVerificationProperty(kycProperty.Type, kycProperty.Value);
            }

            BankClient finalClient = newClient.Build();
            Clients.Add(finalClient);
            return finalClient;
        }

        public void VerifyClient(BankClient bankClient, List<KycProperty> kycProps)
        {
            foreach (KycProperty kycProperty in kycProps)
            {
                bankClient.AddNewVerificationProperty(kycProperty.Type, kycProperty.Value);
            }
        }

        public DebitAccount CreateDebitAccount(BankClient client)
        {
            DebitAccount newAccount = AccountManager.CreateDebitAccount(client, this);
            return newAccount;
        }

        public DepositAccount CreateDepositAccount(BankClient client, double initialSum, int days)
        {
            DepositAccount newAccount = AccountManager.CreateDepositAccount(client, this, initialSum, days);
            return newAccount;
        }

        public CreditAccount CreateCreditAccount(BankClient client)
        {
            CreditAccount newAccount = AccountManager.CreateCreditAccount(client, this);
            return newAccount;
        }

        public void CancelTransaction(Guid id)
        {
            TransactionManager transactionManager = CentralBankRef.TransactionManager;
            transactionManager.CancelTransaction(id);
        }

        public void PayRemainBonus(DateTime time)
        {
            List<Account> debitAccounts = AccountManager.GetDebitAccounts(this);
            DateTime lastRemainPaymentDate = CentralBankRef.RemainHistory.LastOrDefault();
            double remainBonusPercent = CentralBankRef.GetConfiguration(this).DebitAccountPercentProfit;
            bool emptyRemainBonusHistory = lastRemainPaymentDate == default(DateTime);

            foreach (DebitAccount debitAccount in debitAccounts)
            {
                double remainBonus = 0;
                if (emptyRemainBonusHistory) lastRemainPaymentDate = debitAccount.CreationDate;
                while (DateTime.Compare(lastRemainPaymentDate, time) < 0)
                {
                    remainBonus += debitAccount.CalculateBalanceByTime(lastRemainPaymentDate) * remainBonusPercent / 365 / 100;
                    lastRemainPaymentDate = lastRemainPaymentDate.AddDays(1);
                }

                debitAccount.DepositMoney(remainBonus);
            }
        }

        public void WithdrawCommissionFee(DateTime time)
        {
            List<Account> creditAccounts = AccountManager.GetCreditAccounts(this);
            DateTime lastCommissionWithdrawDate = CentralBankRef.CommissionHistory.LastOrDefault();
            double commissionFeeValue = CentralBankRef.GetConfiguration(this).CreditAccountFeeAmountDaily;
            bool emptyCommissionHistory = lastCommissionWithdrawDate == default(DateTime);

            foreach (CreditAccount creditAccount in creditAccounts)
            {
                double commissionFee = 0;
                if (emptyCommissionHistory) lastCommissionWithdrawDate = creditAccount.CreationDate;
                while (DateTime.Compare(lastCommissionWithdrawDate, time) < 0)
                {
                    if (creditAccount.CalculateBalanceByTime(lastCommissionWithdrawDate) < 0) commissionFee += commissionFeeValue;
                    lastCommissionWithdrawDate = lastCommissionWithdrawDate.AddDays(1);
                }

                creditAccount.WithdrawMoney(commissionFee);
            }
        }

        public void PayDepositBonus(DateTime time)
        {
            List<Account> depositAccounts = AccountManager.GetDepositAccounts(this);
            DateTime lastDepositBonusDate = CentralBankRef.DepositBonusHistory.LastOrDefault();

            bool emptyDepositBonusHistory = lastDepositBonusDate == default(DateTime);

            foreach (DepositAccount depositAccount in depositAccounts)
            {
                double depositBonus = 0;
                if (emptyDepositBonusHistory) lastDepositBonusDate = depositAccount.CreationDate;
                while (DateTime.Compare(lastDepositBonusDate, time) < 0 && DateTime.Compare(lastDepositBonusDate, depositAccount.ExpirationDate) < 0)
                {
                    double depositBonusPercent = CentralBankRef.GetConfiguration(this).GetDepositProfitByInitialBalance(depositAccount.InitialSum);
                    depositBonus += depositAccount.CalculateBalanceByTime(time) * depositBonusPercent / 365 / 100;
                    lastDepositBonusDate = lastDepositBonusDate.AddDays(1);
                }

                depositAccount.DepositMoney(depositBonus);
            }
        }

        public void ChangeBankConfiguration(BankConfiguration newConfiguration)
        {
            BankConfiguration currentConfiguration = CentralBankRef.GetConfiguration(this);

            if (currentConfiguration.NotVerifiedWithdrawLimitDaily != newConfiguration.NotVerifiedWithdrawLimitDaily)
            {
                Notification notification = new Notification($"Bank {Name} changed not verified withdraw limit from {currentConfiguration.NotVerifiedWithdrawLimitDaily} to {newConfiguration.NotVerifiedWithdrawLimitDaily}");
                NotifyClients(notification);
            }

            if (currentConfiguration.NotVerifiedSendLimitDaily != newConfiguration.NotVerifiedSendLimitDaily)
            {
                Notification notification = new Notification($"Bank {Name} changed not verified send limit from {currentConfiguration.NotVerifiedSendLimitDaily} to {newConfiguration.NotVerifiedSendLimitDaily}");
                NotifyClients(notification);
            }

            if (currentConfiguration.DebitAccountPercentProfit != newConfiguration.DebitAccountPercentProfit)
            {
                Notification notification = new Notification($"Bank {Name} changed debit remain bonus from {currentConfiguration.DebitAccountPercentProfit}% to {newConfiguration.DebitAccountPercentProfit}%");
                NotifyClients(notification);
            }

            if (currentConfiguration.CreditAccountFeeAmountDaily != newConfiguration.CreditAccountFeeAmountDaily)
            {
                Notification notification = new Notification($"Bank {Name} changed credit daily fee from {currentConfiguration.CreditAccountFeeAmountDaily} to {newConfiguration.CreditAccountFeeAmountDaily}");
                NotifyClients(notification);
            }

            if (currentConfiguration.CreditAccountLowerLimit != newConfiguration.CreditAccountLowerLimit)
            {
                Notification notification = new Notification($"Bank {Name} changed credit daily fee from {currentConfiguration.CreditAccountLowerLimit} to {newConfiguration.CreditAccountLowerLimit}");
                NotifyClients(notification);
            }

            CentralBankRef.ChangeBankConfiguration(this, newConfiguration);
        }

        public void SubscribeClientForNotifications(IBankObserver newNotificationClient)
        {
            if (NotificationClients.Find(notificationClient => notificationClient.GetBankClientRef() == newNotificationClient.GetBankClientRef()) != null) throw new BanksException("Attempt to subscribe existing notification client");
            NotificationClients.Add(newNotificationClient);
            Notification welcomingNotification = new Notification("You was successfully subscribed for notificationsYou was successfully subscribed for notifications");
            newNotificationClient.Update(welcomingNotification);
        }

        public bool IsClientSubscribed(BankClient client)
        {
            return NotificationClients.Find(notificationClient => notificationClient.GetBankClientRef() == client) != null;
        }

        public void UnsubscribeClientFromNotifications(IBankObserver notificationClient)
        {
            NotificationClients.Remove(notificationClient);
        }

        public void NotifyClients(Notification notification)
        {
            foreach (IBankObserver notificationClient in NotificationClients)
            {
                notificationClient.Update(notification);
            }
        }

        public IBankObserver GetObserverByClient(BankClient client)
        {
            return NotificationClients.Find(notificationClient => notificationClient.GetBankClientRef() == client);
        }

        public List<Notification> GetAllNotifications(IBankObserver notificationClient)
        {
            return notificationClient.GetAllUpdates();
        }
    }
}