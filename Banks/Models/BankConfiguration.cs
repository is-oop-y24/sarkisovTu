using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Tools;

namespace Banks.Models
{
    public class BankConfiguration
    {
        public BankConfiguration(
            double notVerifiedWithdrawLimitDaily,
            double notVerifiedSendLimitDaily,
            double debitAccountPercentProfit,
            List<int> availableDepositAccountLockedDays,
            Dictionary<MoneyRange, double> depositAccountProfitTable,
            double creditAccountFeeAmountDaily,
            double creditAccountLowerLimit)
        {
            NotVerifiedWithdrawLimitDaily = notVerifiedWithdrawLimitDaily;
            NotVerifiedSendLimitDaily = notVerifiedSendLimitDaily;
            DebitAccountPercentProfit = debitAccountPercentProfit;
            AvailableDepositAccountLockedDays = availableDepositAccountLockedDays;
            if (!IsProfitTableValid(depositAccountProfitTable)) throw new BanksException("Provided deposit profit table is invalid");
            DepositAccountProfitTable = depositAccountProfitTable;
            CreditAccountFeeAmountDaily = creditAccountFeeAmountDaily;
            CreditAccountLowerLimit = creditAccountLowerLimit;
        }

        public double NotVerifiedWithdrawLimitDaily { get; private set; }
        public double NotVerifiedSendLimitDaily { get; private set; }

        public double DebitAccountPercentProfit { get; private set; }
        public List<int> AvailableDepositAccountLockedDays { get; private set; }
        public Dictionary<MoneyRange, double> DepositAccountProfitTable { get; private set; }
        public double CreditAccountFeeAmountDaily { get; private set; }
        public double CreditAccountLowerLimit { get; private set; }

        public double GetDepositProfitByInitialBalance(double balance)
        {
            double depositPercent = 0;
            foreach (KeyValuePair<MoneyRange, double> profitItem in DepositAccountProfitTable)
            {
                if ((balance >= profitItem.Key.LowerLimit) && (balance < profitItem.Key.UpperLimit)) depositPercent = profitItem.Value;
            }

            return depositPercent;
        }

        private bool IsProfitTableValid(Dictionary<MoneyRange, double> table)
        {
            bool isTableValid = false;
            if (table.Count == 1) return true;
            for (int i = 0; i < table.Count - 1; i++)
            {
                if (i == 0 && table.ElementAt(i).Key.LowerLimit != 0) break;
                if (table.ElementAt(i).Key.UpperLimit != table.ElementAt(i + 1).Key.LowerLimit) break;
                isTableValid = true;
            }

            return isTableValid;
        }
    }
}