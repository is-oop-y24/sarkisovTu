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

        public string OperationType
        {
            get { return _operationType; }
        }

        public double Amount
        {
            get { return _amount; }
        }
    }
}