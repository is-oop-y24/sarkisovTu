using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Models;
using Banks.Repository;
using Banks.Tools;
using Banks.Types;

namespace Banks.Services
{
    public class AccountManager
    {
        private AccountRepository _accountRepository;
        public AccountManager()
        {
            _accountRepository = new AccountRepository();
        }

        public DebitAccount CreateDebitAccount(BankClient client, Bank bank)
        {
            int accountCount = _accountRepository.Accounts.Where(account => account.AccountType == AccountType.Debit).Count();
            string newAccountId = ((int)AccountType.Debit).ToString() + '-' + accountCount;
            DebitAccount newAccount = new DebitAccount(newAccountId, AccountType.Debit, client, bank);
            _accountRepository.AddAccount(newAccount);
            return newAccount;
        }

        public DepositAccount CreateDepositAccount(BankClient client, Bank bank, double initialSum, int days)
        {
            if (!bank.BankConfigurationProperties.AvailableDepositAccountLockedDays.Contains(days))
                throw new BanksException("Bank doesn't provide this locked period");
            DateTime expirationDate = DateTime.Now.AddDays(days);
            int accountCount = _accountRepository.Accounts.Where(account => account.AccountType == AccountType.Deposit).Count();
            string newAccountId = ((int)AccountType.Deposit).ToString() + '-' + accountCount;
            DepositAccount newAccount = new DepositAccount(newAccountId, AccountType.Deposit, client, bank, initialSum, expirationDate);
            _accountRepository.AddAccount(newAccount);
            newAccount.DepositMoney(initialSum);
            return newAccount;
        }

        public CreditAccount CreateCreditAccount(BankClient client, Bank bank)
        {
            int accountCount = _accountRepository.Accounts.Where(account => account.AccountType == AccountType.Credit).Count();
            string newAccountId = ((int)AccountType.Credit).ToString() + '-' + accountCount;
            CreditAccount newAccount = new CreditAccount(newAccountId, AccountType.Credit, client, bank);
            _accountRepository.AddAccount(newAccount);
            return newAccount;
        }

        public List<Account> GetDebitAccounts(Bank bank)
        {
            return _accountRepository.GetDebitAccounts(bank);
        }

        public List<Account> GetDepositAccounts(Bank bank)
        {
            return _accountRepository.GetDepositAccounts(bank);
        }

        public List<Account> GetCreditAccounts(Bank bank)
        {
            return _accountRepository.GetCreditAccounts(bank);
        }
    }
}