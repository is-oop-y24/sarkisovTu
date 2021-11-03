using System.Collections.Generic;
using Shops.Models;

namespace Shops.Services
{
    public static class MoneyService
    {
        public static void ShopTransaction(Person source, Shop destination, double amount)
        {
            source.AddTransaction(new Transaction(TransactionType.Withdraw, amount));
            destination.AddTransaction(new Transaction(TransactionType.Deposit, amount));
        }

        public static Transaction CreateDeposit(double amount)
        {
            return new Transaction(TransactionType.Deposit, amount);
        }

        public static double CalculateBalance(List<Transaction> transactions)
        {
            double balance = 0;
            foreach (Transaction transaction in transactions)
            {
                if ((int)transaction.OperationType == 1)
                {
                    balance += transaction.Amount;
                }

                if ((int)transaction.OperationType == 2)
                {
                    balance -= transaction.Amount;
                }
            }

            return balance;
        }
    }
}