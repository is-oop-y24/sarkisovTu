using System.Collections.Generic;
using Shops.Models;

namespace Shops.Services
{
    public static class MoneyService
    {
        public static void ShopTransaction(Person source, Shop destination, double amount)
        {
            source.AddTransaction(new Transaction("withdraw", amount));
            destination.AddTransaction(new Transaction("deposit", amount));
        }

        public static Transaction CreateDeposit(double amount)
        {
            return new Transaction("deposit", amount);
        }

        public static double CalculateBalance(List<Transaction> transactions)
        {
            double balance = 0;
            foreach (Transaction transaction in transactions)
            {
                if (transaction.OperationType == "deposit")
                {
                    balance += transaction.Amount;
                }

                if (transaction.OperationType == "withdraw")
                {
                    balance -= transaction.Amount;
                }
            }

            return balance;
        }
    }
}