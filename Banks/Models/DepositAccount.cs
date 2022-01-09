using System;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Models
{
    public class DepositAccount : Account
    {
        public DepositAccount(string id, AccountType accountType, BankClient client, Bank bank, double initialSum, DateTime expirationDate)
            : base(id, accountType, client, bank)
        {
            InitialSum = initialSum;
            ExpirationDate = expirationDate;
        }

        public DateTime ExpirationDate { get; private set; }
        public double InitialSum { get; private set; }

        public Transaction WithdrawMoney(double amount)
        {
            TransactionManager transactionManager = BankRef.CentralBankRef.TransactionManager;
            if (DateTime.Compare(DateTime.Now, ExpirationDate) <= 0)
                throw new BanksException("Deposit account now locked for withdraw operations");
            return base.WithdrawMoney(amount);
        }

        public Transaction SendMoney(Account to, double value)
        {
            TransactionManager transactionManager = BankRef.CentralBankRef.TransactionManager;
            if (DateTime.Compare(DateTime.Now, ExpirationDate) <= 0)
                throw new BanksException("Deposit account now locked for withdraw operations");
            return base.SendMoney(to, value);
        }
    }
}