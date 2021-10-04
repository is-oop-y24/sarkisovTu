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

        public Product GetProductRef()
        {
            return _productRef;
        }

        public int GetProductAmount()
        {
            return _amount;
        }

        public double GetProductPrice()
        {
            return _price;
        }
    }
}