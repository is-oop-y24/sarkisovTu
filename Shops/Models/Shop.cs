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
            ProductConfiguration queryConfiguration = _productStock.Find(config => config.ProductRef.Equals(product));
            if (queryConfiguration != null)
            {
                newShopProduct = new ProductConfiguration(product, queryConfiguration.ProductAmount + amount, price);
                _productStock.Remove(queryConfiguration);
            }

            _productStock.Add(newShopProduct);
        }

        public void ChangeProductPrice(Product product, double newPrice)
        {
            ProductConfiguration queryConfiguration = _productStock.Find(config => config.ProductRef.Equals(product));
            if (queryConfiguration != null)
            {
                var newShopProduct = new ProductConfiguration(
                    queryConfiguration.ProductRef,
                    queryConfiguration.ProductAmount,
                    newPrice);
                _productStock.Remove(queryConfiguration);
                _productStock.Add(newShopProduct);
            }
        }

        public void RegisterPurchase(Person person, List<ProductConfiguration> productWishList)
        {
            double checkOffer = 0;
            foreach (ProductConfiguration requestProduct in productWishList)
            {
                ProductConfiguration shopProduct = _productStock.Find(product =>
                    product.ProductRef.Equals(requestProduct.ProductRef));
                if (shopProduct != null && shopProduct.ProductAmount >= requestProduct.ProductAmount)
                {
                    checkOffer += shopProduct.ProductPrice * requestProduct.ProductAmount;
                }
                else
                {
                    checkOffer = -1;
                    break;
                }
            }

            if ((int)checkOffer == -1)
                throw new ShopsException("Wishlist contains products which are out of stock");
            if (checkOffer >= person.Balance)
                throw new ShopsException($"Customer's balance:{person.Balance} is less then total check:{checkOffer}");

            MoneyService.ShopTransaction(person, this, checkOffer);

            foreach (ProductConfiguration requestProduct in productWishList)
            {
                ProductConfiguration shopProduct = _productStock.Find(product =>
                    product.ProductRef.Equals(requestProduct.ProductRef));
                if (shopProduct != null)
                {
                    var newShopProduct = new ProductConfiguration(
                        shopProduct.ProductRef,
                        shopProduct.ProductAmount - requestProduct.ProductAmount,
                        shopProduct.ProductPrice);
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
            ProductConfiguration queryConfiguration = _productStock.Find(config => config.ProductRef.Equals(productToFind));
            return queryConfiguration;
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