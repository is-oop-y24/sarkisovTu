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
            DebitAccounts = new List<DebitAccount>();
            DepositAccounts = new List<DepositAccount>();
            CreditAccounts = new List<CreditAccount>();
        }

        public List<DebitAccount> DebitAccounts { get; private set; }
        public List<DepositAccount> DepositAccounts { get; private set; }
        public List<CreditAccount> CreditAccounts { get; private set; }

        public void AddAccount(DebitAccount account)
        {
            DebitAccounts.Add(account);
        }

        public void AddAccount(DepositAccount account)
        {
            DepositAccounts.Add(account);
        }

        public void AddAccount(CreditAccount account)
        {
            CreditAccounts.Add(account);
        }

        public List<DebitAccount> GetDebitAccounts(Bank bank)
        {
             return DebitAccounts.FindAll(account => account.BankRef == bank);
        }

        public List<DepositAccount> GetDepositAccounts(Bank bank)
        {
            return DepositAccounts.FindAll(account => account.BankRef == bank);
        }

        public List<CreditAccount> GetCreditAccounts(Bank bank)
        {
            return CreditAccounts.FindAll(account => account.BankRef == bank);
        }
    }
}