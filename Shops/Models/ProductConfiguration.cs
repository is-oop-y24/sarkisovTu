namespace Shops.Models
{
    public class ProductConfiguration
    {
        private readonly Product _productRef;
        private readonly int _amount;
        private readonly double _price;

        public ProductConfiguration(Product productRef, int amount, double price)
        {
            _productRef = productRef;
            _amount = amount;
            _price = price;
        }

        public ProductConfiguration(Product productRef, int amount)
        {
            _productRef = productRef;
            _amount = amount;
            _price = 0;
        }

        public Product ProductRef
        {
            get { return _productRef; }
        }

        public int ProductAmount
        {
            get { return _amount; }
        }

        public double ProductPrice
        {
            get { return _price; }
        }
    }
}