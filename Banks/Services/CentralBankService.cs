using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Models;
using Banks.Tools;

namespace Banks.Services
{
    public class CentralBankService
    {
        public CentralBankService()
        {
            Banks = new Dictionary<Bank, BankConfiguration>();
            TransactionManager = new TransactionManager();
            AccountManager = new AccountManager();
            RemainHistory = new List<DateTime>();
            CommissionHistory = new List<DateTime>();
            DepositBonusHistory = new List<DateTime>();
        }

        public Dictionary<Bank, BankConfiguration> Banks { get; private set; }
        public TransactionManager TransactionManager { get; private set; }
        public AccountManager AccountManager { get; private set; }
        public List<DateTime> RemainHistory { get; private set; }
        public List<DateTime> CommissionHistory { get; private set; }

        public List<DateTime> DepositBonusHistory { get; private set; }

        public Bank CreateBank(string name, BankConfiguration configuration, INotificationManager notificationManager)
        {
            if (Banks.Keys.ToList().Find(bank => bank.Name == name) != null) throw new BanksException("Bank with this name is already exist");
            Bank newBank = new Bank(this, name, notificationManager);
            Banks.Add(newBank, configuration);
            return newBank;
        }

        public BankConfiguration GetConfiguration(Bank bank)
        {
            return Banks[bank];
        }

        public void ToggleRemainBonus()
        {
            ToggleRemainBonus(DateTime.Now);
        }

        public void ToggleCommissionFee()
        {
            ToggleCommissionFee(DateTime.Now);
        }

        public void ToggleDepositBonus()
        {
            ToggleDepositBonus(DateTime.Now);
        }

        public void ToggleRemainBonus(DateTime dateNow)
        {
            foreach (Bank bank in Banks.Keys)
            {
                bank.PayRemainBonus(dateNow);
            }

            RemainHistory.Add(dateNow);
        }

        public void ToggleCommissionFee(DateTime dateNow)
        {
            foreach (var bank in Banks.Keys)
            {
                bank.WithdrawCommissionFee(dateNow);
            }

            CommissionHistory.Add(dateNow);
        }

        public void ToggleDepositBonus(DateTime dateNow)
        {
            foreach (var bank in Banks.Keys)
            {
                bank.PayDepositBonus(dateNow);
            }

            DepositBonusHistory.Add(dateNow);
        }

        public void ChangeBankConfiguration(Bank bank, BankConfiguration newConfiguration)
        {
            Bank bankToChange = bank;
            Banks.Remove(bank);
            Banks.Add(bank, newConfiguration);
        }
    }
}