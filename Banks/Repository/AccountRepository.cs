using System.Collections.Generic;
using System.Linq;
using Banks.Models;
using Banks.Types;

namespace Banks.Repository
{
    public class AccountRepository
    {
        public AccountRepository()
        {
            Accounts = new List<Account>();
        }

        public List<Account> Accounts { get; private set; }

        public void AddAccount(DebitAccount account)
        {
            Accounts.Add(account);
        }

        public void AddAccount(DepositAccount account)
        {
            Accounts.Add(account);
        }

        public void AddAccount(CreditAccount account)
        {
            Accounts.Add(account);
        }

        public List<Account> GetDebitAccounts(Bank bank)
        {
            return Accounts.FindAll(account => account.BankRef == bank && account.AccountType == AccountType.Debit);
        }

        public List<Account> GetDepositAccounts(Bank bank)
        {
            return Accounts.FindAll(account => account.BankRef == bank && account.AccountType == AccountType.Deposit);
        }

        public List<Account> GetCreditAccounts(Bank bank)
        {
            return Accounts.FindAll(account => account.BankRef == bank && account.AccountType == AccountType.Credit);
        }
    }
}