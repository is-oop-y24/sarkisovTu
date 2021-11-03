namespace Shops.Models
{
    public class ProductConfiguration
    {
        public ProductConfiguration(Product productRef, int amount, double price)
        {
            ProductRef = productRef;
            ProductAmount = amount;
            ProductPrice = price;
        }

        public ProductConfiguration(Product productRef, int amount)
        {
            ProductRef = productRef;
            ProductAmount = amount;
            ProductPrice = 0;
        }

        public Product ProductRef
        {
            get;
            private set;
        }

        public int ProductAmount
        {
            get;
            private set;
        }

        public double ProductPrice
        {
            get;
            private set;
        }
    }
}