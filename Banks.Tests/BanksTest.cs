using System;
using System.Collections.Generic;
using Banks.Models;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using NUnit.Framework;

namespace Banks.Tests
{
    public class BanksTest
    {
        private CentralBankService _centralBank;
        private Bank _chase;
        private Bank _morganStanley;

        [SetUp]
        public void Setup()
        {
            _centralBank = new CentralBankService();
            
            Dictionary<MoneyRange, double> depositTable = new Dictionary<MoneyRange, double>();
            depositTable.Add(new MoneyRange(0, 10000), 3.65);
            depositTable.Add(new MoneyRange(10000, 100000), 7.30);
            var bankConfiguration1 = new BankConfiguration(
                10000,
                10000,
                3.65,
                new List<int>() { 30, 60 },
                depositTable,
                100,
                -100000);

            var bankConfiguration2 = new BankConfiguration(
                50000,
                50000,
                3.65,
                new List<int>() { 30, 60, 120 },
                depositTable,
                500,
                -300000);
            _chase = _centralBank.CreateBank("Chase", bankConfiguration1, new ConsoleAppNotificationManager());
            _morganStanley = _centralBank.CreateBank("Morgan Stanley", bankConfiguration2, new ConsoleAppNotificationManager());
        }

        [Test]
        public void OverWithdrawMoneyFromDebitAccount_ThrowException()
        {
            BankClient unverifiedClient1 = _chase.CreateBaseClient("Nikita", "Sarkisov");
            DebitAccount debitAccount1 = _chase.CreateDebitAccount(unverifiedClient1);
            debitAccount1.DepositMoney(5000);
            Assert.Catch<BanksException>(() =>
            {
                debitAccount1.WithdrawMoney(10000);
            });
        }

        [Test]
        public void OverWithdrawMoneyFromCreditAccount_ThrowException()
        {
            BankClient verifiedClient1 = _chase.CreateVerifiedClient(
                "Nikita", 
                "Sarkisov", 
                new List<KycProperty>() { new KycProperty(KycType.DocumentId, "1221 021 322")});
            CreditAccount creditAccount1 = _chase.CreateCreditAccount(verifiedClient1);
            //Due to bankConfiguration1 lower limit for credit account is -100000
            Assert.Catch<BanksException>(() =>
            {
                creditAccount1.WithdrawMoney(200000);
            });
        }

        [Test]
        public void WithdrawMoneyFromAccountOfUnverifiedClient_ThrowException()
        {
            BankClient unverifiedClient1 = _chase.CreateBaseClient("Nikita", "Sarkisov");
            DebitAccount debitAccount1 = _chase.CreateDebitAccount(unverifiedClient1);
            debitAccount1.DepositMoney(30000);
            Assert.Catch<BanksException>(() =>
            {
                debitAccount1.WithdrawMoney(20000);
            });
                
            BankClient unverifiedClient2 = _morganStanley.CreateBaseClient("Nikita", "Sarkisov");
            DebitAccount debitAccount2 = _morganStanley.CreateDebitAccount(unverifiedClient2);
            debitAccount2.DepositMoney(80000);
            Assert.Catch<BanksException>(() =>
            {
                debitAccount2.WithdrawMoney(40000);
                debitAccount2.WithdrawMoney(40000);
            });
        }

        [Test]
        public void RemoveTransactionLimitsByVerification_LimitsWereRemoved()
        {
            BankClient unverifiedClient1 = _chase.CreateBaseClient("Nikita", "Sarkisov");
            DebitAccount debitAccount1 = _chase.CreateDebitAccount(unverifiedClient1);
            debitAccount1.DepositMoney(30000);
            DateTime datePoint1 = DateTime.Now;
            Assert.Catch<BanksException>(() =>
            {
                debitAccount1.WithdrawMoney(20000);
            });
            List<KycProperty> verificationMethod = new List<KycProperty>() { new KycProperty(KycType.Address, "Russia, Saint Petersburg, street 1") };
            _chase.VerifyClient(unverifiedClient1, verificationMethod);
            debitAccount1.WithdrawMoney(20000);
            DateTime datePoint2 = DateTime.Now;
            Assert.AreEqual(debitAccount1.CalculateBalanceByTime(datePoint1) - 20000, debitAccount1.CalculateBalanceByTime(datePoint2));
        }

        [Test]
        public void CreateDepositAccountWithNonExistLockedPeriod_ThrowException()
        {
            BankClient unverifiedClient1 = _morganStanley.CreateBaseClient("Nikita", "Sarkisov");
            Assert.Catch<BanksException>(() =>
            {
                DepositAccount depositAccount = _morganStanley.CreateDepositAccount(unverifiedClient1, 5000, 121);
            });
        }

        [Test]
        public void WithdrawAndSendMoneyFromLockedDepositAccount_ThrowException()
        {
            BankClient verifiedClient1 = _morganStanley.CreateVerifiedClient(
                "Nikita", 
                "Sarkisov", 
                new List<KycProperty>() { new KycProperty(KycType.DocumentId, "1221 021 322")});
            DebitAccount debitAccount = _morganStanley.CreateDebitAccount(verifiedClient1);
            DepositAccount depositAccount = _morganStanley.CreateDepositAccount(verifiedClient1, 20000, 30);
            Assert.Catch<BanksException>(() =>
            {
                depositAccount.WithdrawMoney(10);
                depositAccount.SendMoney(debitAccount, 10);
            });
        }

        [Test]
        public void PayDebitRemainBonusForAllDebitAccounts_BonusWasDeposited()
        {
            BankClient unverifiedClient1 = _morganStanley.CreateBaseClient("Nikita", "Sarkisov");
            BankClient unverifiedClient2 = _chase.CreateBaseClient("Elon", "Musk");
            DebitAccount debitAccount1 = _morganStanley.CreateDebitAccount(unverifiedClient1);
            DebitAccount debitAccount2 = _chase.CreateDebitAccount(unverifiedClient2);
            debitAccount1.DepositMoney(10000);
            debitAccount1.DepositMoney(10000);
            DateTime datePoint = DateTime.Now;
            //Simulate case when bonus will be added in 1 day
            _centralBank.ToggleRemainBonus(DateTime.Now.AddDays(1));
            Assert.AreEqual(
                debitAccount1.CalculateCurrentBalance() - debitAccount1.CalculateBalanceByTime(datePoint), 
                debitAccount1.CalculateBalanceByTime(datePoint) * _morganStanley.BankConfigurationProperties.DebitAccountPercentProfit / 365 / 100);
            Assert.AreEqual(
                debitAccount2.CalculateCurrentBalance() - debitAccount2.CalculateBalanceByTime(datePoint), 
                debitAccount2.CalculateBalanceByTime(datePoint) * _chase.BankConfigurationProperties.DebitAccountPercentProfit / 365 / 100);
        }

        [Test]
        public void CancelSuspiciousTransaction_MoneyWasTransferredToSender()
        {
            BankClient unverifiedClient1 = _morganStanley.CreateBaseClient("Nikita", "Sarkisov");
            BankClient unverifiedClient2 = _chase.CreateBaseClient("Elon", "Musk");
            DebitAccount debitAccount1 = _morganStanley.CreateDebitAccount(unverifiedClient1);
            DebitAccount debitAccount2 = _chase.CreateDebitAccount(unverifiedClient2);
            debitAccount1.DepositMoney(5000);
            Transaction suspiciousTransaction = debitAccount1.SendMoney(debitAccount2, 5000);
            _morganStanley.CancelTransaction(suspiciousTransaction.Id);
            Assert.AreEqual(debitAccount1.CalculateCurrentBalance(), 5000);
            Assert.AreEqual(debitAccount2.CalculateCurrentBalance(), 0);
        }
    }
}