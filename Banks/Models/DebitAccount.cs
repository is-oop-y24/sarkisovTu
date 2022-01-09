using System;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Models
{
    public class DebitAccount : Account
    {
        public DebitAccount(string id, AccountType accountType, BankClient client, Bank bank)
            : base(id, accountType, client, bank)
        {
        }

        public double DebitAccountPercentProfit { get { return BankRef.BankConfigurationProperties.DebitAccountPercentProfit; } }
    }
}