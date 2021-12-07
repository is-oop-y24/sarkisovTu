using System;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Models
{
    public class CreditAccount : Account
    {
        public CreditAccount(string id, AccountType accountType, BankClient client, Bank bank)
            : base(id, accountType, client, bank)
        {
        }

        public double AccountFee { get { return BankRef.BankConfigurationProperties.CreditAccountFeeAmountDaily; } }
        public double LowerLimit { get { return BankRef.BankConfigurationProperties.CreditAccountLowerLimit; } }

        public Transaction WithdrawMoney(double amount)
        {
            return this.WithdrawMoney(amount, LowerLimit);
        }

        public Transaction SendMoney(Account to, double value)
        {
            return this.SendMoney(to, value, LowerLimit);
        }
    }
}