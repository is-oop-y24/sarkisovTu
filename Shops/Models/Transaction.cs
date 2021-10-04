using Shops.Tools;

namespace Shops.Models
{
    public class Transaction
    {
        private readonly string _operationType;
        private readonly double _amount;

        public Transaction(string operationType, double amount)
        {
            if (operationType == "deposit" || operationType == "withdraw")
            {
                _operationType = operationType;
                _amount = amount;
            }
            else
            {
                throw new ShopsException("Invalid transaction type");
            }
        }

        public string GetOperationType()
        {
            return _operationType;
        }

        public double GetAmount()
        {
            return _amount;
        }
    }
}