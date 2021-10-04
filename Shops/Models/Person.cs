using System.Collections.Generic;
using Shops.Services;

namespace Shops.Models
{
    public class Person
    {
        private readonly string _name;
        private readonly List<Transaction> _transactions;

        public Person(string name, double balance)
        {
            _name = name;
            _transactions = new List<Transaction>();
            _transactions.Add(MoneyService.CreateDeposit(balance));
        }

        public string GetName()
        {
            return _name;
        }

        public double GetBalance()
        {
            return MoneyService.CalculateBalance(_transactions);
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }
    }
}