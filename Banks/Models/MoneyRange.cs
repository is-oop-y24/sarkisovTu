using Banks.Tools;

namespace Banks.Models
{
    public class MoneyRange
    {
        public MoneyRange(double lowerLimit, double upperLimit)
        {
            if (lowerLimit >= upperLimit) throw new BanksException("Invalid money range");
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
        }

        public double LowerLimit { get; private set; }
        public double UpperLimit { get; private set; }

        public string GetStringRange()
        {
            return LowerLimit + " - " + UpperLimit;
        }
    }
}