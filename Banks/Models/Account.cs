using System;
using System.Collections.Generic;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Models
{
    public class Account
    {
        public Account(string id, AccountType accountType, BankClient client, Bank bank)
        {
            Id = id;
            AccountType = accountType;
            BankClientRef = client;
            BankRef = bank;
            CreationDate = DateTime.Now;
        }

        public string Id { get; private set; }
        public AccountType AccountType { get; private set; }
        public Bank BankRef { get; private set; }
        public BankClient BankClientRef { get; private set; }
        public DateTime CreationDate { get; private set; }

        public TransactionManager TransactionManager { get { return BankRef.CentralBankRef.TransactionManager; } }

        public double CalculateCurrentBalance()
        {
            DateTime dateNow = DateTime.Now;
            return CalculateBalanceByTime(dateNow);
        }

        public double CalculateBalanceByTime(DateTime time)
        {
            return TransactionManager.CalculateBalanceByTime(this, time);
        }

        public Transaction DepositMoney(double amount)
        {
            Transaction newTransaction = TransactionManager.CreateDepositTransaction(this, amount);
            return newTransaction;
        }

        public Transaction WithdrawMoney(double value, double creditValue = 0)
        {
            if (CalculateCurrentBalance() - creditValue < value) throw new BanksException("Not enough money on balance");
            if (!BankClientRef.HasVerification() &&
                TransactionManager.CalculateExpensesOverDay(this, DateTime.Now) + value >
                BankRef.BankConfigurationProperties.NotVerifiedWithdrawLimitDaily)
                throw new BanksException("Your not verified daily limit of send operations was exceeded");
            Transaction newTransaction = TransactionManager.CreateWithdrawTransaction(this, value);
            return newTransaction;
        }

        public Transaction SendMoney(Account to, double value, double creditValue = 0)
        {
            if (CalculateCurrentBalance() - creditValue < value) throw new BanksException("Not enough money on balance");
            if (!BankClientRef.HasVerification() &&
                TransactionManager.CalculateExpensesOverDay(this, DateTime.Now) + value >
                BankRef.BankConfigurationProperties.NotVerifiedSendLimitDaily)
                throw new BanksException("Your not verified daily limit of send operations was exceeded");
            Transaction newTransaction = TransactionManager.CreateSendTransaction(this, to, value);
            return newTransaction;
        }
    }
}