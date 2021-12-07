using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Models;
using Banks.Tools;
using Banks.Types;

namespace Banks.Services
{
    public class TransactionManager
    {
        private List<Transaction> _transactions;

        public TransactionManager()
        {
            _transactions = new List<Transaction>();
        }

        public Transaction CreateSendTransaction(Account from, Account to, double value)
        {
            if (!IsTransactionValid(value)) throw new BanksException("Incorrect transaction value");
            int newTransactionId = _transactions.Count;
            Transaction newTransaction = new Transaction.Builder(newTransactionId)
                .SetTransactionType(TransactionType.Send).SetFromAccount(from).SetToAccount(to).SetValue(value).Build();
            _transactions.Add(newTransaction);
            return newTransaction;
        }

        public Transaction CreateDepositTransaction(Account to, double value)
        {
            if (!IsTransactionValid(value)) throw new BanksException("Incorrect transaction value");
            int newTransactionId = _transactions.Count;
            Transaction newTransaction = new Transaction.Builder(newTransactionId)
                .SetTransactionType(TransactionType.Deposit).SetFromAccount(null).SetToAccount(to).SetValue(value)
                .Build();
            _transactions.Add(newTransaction);
            return newTransaction;
        }

        public Transaction CreateWithdrawTransaction(Account from, double value)
        {
            if (!IsTransactionValid(value)) throw new BanksException("Incorrect transaction value");
            int newTransactionId = _transactions.Count;
            Transaction newTransaction = new Transaction.Builder(newTransactionId)
                .SetTransactionType(TransactionType.Withdraw).SetFromAccount(from).SetToAccount(null).SetValue(value)
                .Build();
            _transactions.Add(newTransaction);
            return newTransaction;
        }

        public void CancelTransaction(int transactionId)
        {
            Transaction transactionToCancel = _transactions.Find(transaction => transaction.Id == transactionId);
            if (transactionToCancel == null) throw new BanksException("Transaction with provided Id wasn't found");
            int newTransactionId = _transactions.Count();
            CreateSendTransaction(transactionToCancel.To, transactionToCancel.From, transactionToCancel.Value);
        }

        public double CalculateBalanceByTime(Account account, DateTime time)
        {
            return _transactions.FindAll(transaction =>
                (DateTime.Compare(transaction.Date, time) <= 0) && ((transaction.To == account) || (transaction.From == account)))
                .Sum(
                transaction =>
                {
                    double finalValue = 0;
                    if (transaction.TransactionType == TransactionType.Deposit)
                    {
                        finalValue = transaction.Value;
                    }

                    if (transaction.TransactionType == TransactionType.Withdraw)
                    {
                        finalValue = -transaction.Value;
                    }

                    if (transaction.TransactionType == TransactionType.Send)
                    {
                        if (transaction.To == account) finalValue = transaction.Value;
                        if (transaction.From == account) finalValue = -transaction.Value;
                    }

                    return finalValue;
                });
        }

        public double CalculateExpensesOverDay(Account account, DateTime time)
        {
            return _transactions.FindAll(transaction => (transaction.From == account) &&
                                                        (transaction.Date.ToShortDateString() == time.ToShortDateString()))
                .Sum(transaction => transaction.Value);
        }

        private bool IsTransactionValid(double value)
        {
            if (value < 0) return false;
            return true;
        }
    }
}