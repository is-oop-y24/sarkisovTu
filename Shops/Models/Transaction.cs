using System;
using Shops.Tools;

namespace Shops.Models
{
    public class Transaction
    {
        public Transaction(TransactionType operationType, double amount)
        {
            OperationType = operationType;
            Amount = amount;
        }

        public TransactionType OperationType
        {
            get;
            private set;
        }

        public double Amount
        {
            get;
            private set;
        }
    }
}