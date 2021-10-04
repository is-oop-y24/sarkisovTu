using System.Collections.Generic;
using Shops.Services;
using Shops.Tools;

namespace Shops.Models
{
    public class Shop
    {
        private readonly List<ProductConfiguration> _productStock;
        private readonly List<Transaction> _transactions;
        private readonly string _name;
        private readonly int _id;

        public Shop(int id, string name)
        {
            _id = id;
            _name = name;
            _productStock = new List<ProductConfiguration>();
            _transactions = new List<Transaction>();
        }

        public void AddProduct(Product product, int amount, double price)
        {
            var newShopProduct = new ProductConfiguration(product, amount, price);

            foreach (ProductConfiguration productConfiguration in _productStock)
            {
                if (productConfiguration.GetProductRef().Equals(product))
                {
                    newShopProduct = new ProductConfiguration(product, productConfiguration.GetProductAmount() + amount, price);
                    _productStock.Remove(productConfiguration);
                    break;
                }
            }

            _productStock.Add(newShopProduct);
        }

        public void ChangeProductPrice(Product product, double newPrice)
        {
            foreach (ProductConfiguration productConfiguration in _productStock)
            {
                if (productConfiguration.GetProductRef().Equals(product))
                {
                    var newShopProduct = new ProductConfiguration(
                        productConfiguration.GetProductRef(),
                        productConfiguration.GetProductAmount(),
                        newPrice);
                    _productStock.Remove(productConfiguration);
                    _productStock.Add(newShopProduct);
                    break;
                }
            }
        }

        public void RegisterPurchase(Person person, List<ProductConfiguration> productWishList)
        {
            double checkOffer = 0;
            foreach (ProductConfiguration requestProduct in productWishList)
            {
                ProductConfiguration shopProduct = _productStock.Find(product =>
                    product.GetProductRef().Equals(requestProduct.GetProductRef()));
                if (shopProduct != null && shopProduct.GetProductAmount() >= requestProduct.GetProductAmount())
                {
                    checkOffer += shopProduct.GetProductPrice() * requestProduct.GetProductAmount();
                }
                else
                {
                    checkOffer = -1;
                    break;
                }
            }

            if ((int)checkOffer == -1)
                throw new ShopsException("Wishlist contains products which are out of stock");
            if (checkOffer >= person.GetBalance())
                throw new ShopsException($"Customer's balance:{person.GetBalance()} is less then total check:{checkOffer}");

            MoneyService.ShopTransaction(person, this, checkOffer);

            foreach (ProductConfiguration requestProduct in productWishList)
            {
                ProductConfiguration shopProduct = _productStock.Find(product =>
                    product.GetProductRef().Equals(requestProduct.GetProductRef()));
                if (shopProduct != null)
                {
                    var newShopProduct = new ProductConfiguration(
                        shopProduct.GetProductRef(),
                        shopProduct.GetProductAmount() - requestProduct.GetProductAmount(),
                        shopProduct.GetProductPrice());
                    _productStock.Remove(shopProduct);
                    _productStock.Add(newShopProduct);
                }
            }
        }

        public List<ProductConfiguration> GetProductStock()
        {
            return _productStock;
        }

        public ProductConfiguration GetProductInfo(Product productToFind)
        {
            foreach (ProductConfiguration product in _productStock)
            {
                if (product.GetProductRef().Equals(productToFind)) return product;
            }

            return null;
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }

        public string GetName()
        {
            return _name;
        }

        public double GetBalance()
        {
            return MoneyService.CalculateBalance(_transactions);
        }
    }
}