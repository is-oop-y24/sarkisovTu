using System;
using Banks.Types;

namespace Banks.Models
{
    public class Transaction
    {
        public Transaction(int id, TransactionType transactionType, DateTime date, Account from, Account to, double value)
        {
            Id = id;
            TransactionType = transactionType;
            Date = date;
            From = from;
            To = to;
            Value = value;
        }

        private Transaction(Builder builder)
        {
            Id = builder.Id;
            TransactionType = builder.TransactionType;
            Date = builder.Date;
            From = builder.From;
            To = builder.To;
            Value = builder.Value;
        }

        public int Id { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public DateTime Date { get; private set; }
        public Account From { get; private set; }
        public Account To { get; private set; }
        public double Value { get; private set; }

        public class Builder
        {
            public Builder(int id)
            {
                Id = id;
                Date = DateTime.Now;
            }

            internal int Id { get; private set; }
            internal TransactionType TransactionType { get; private set; }
            internal DateTime Date { get; private set; }
            internal Account From { get; private set; }
            internal Account To { get; private set; }
            internal double Value { get; private set; }

            public Builder SetTransactionType(TransactionType transactionType)
            {
                TransactionType = transactionType;
                return this;
            }

            public Builder SetFromAccount(Account from)
            {
                From = from;
                return this;
            }

            public Builder SetToAccount(Account to)
            {
                To = to;
                return this;
            }

            public Builder SetValue(double value)
            {
                Value = value;
                return this;
            }

            public Transaction Build()
            {
                return new Transaction(this);
            }
        }
    }
}