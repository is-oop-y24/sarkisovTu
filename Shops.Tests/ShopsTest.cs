using System.Collections.Generic;
using NUnit.Framework;
using Shops.Models;
using Shops.Services;
using Shops.Tools;

namespace Shops.Tests
{
    public class Tests
    {
        private ShopsManager _shopsManager;
        
        [SetUp]
        public void Setup()
        {
            _shopsManager = new ShopsManager();
        }
        
        [Test]
        public void AddProductToShop_ShopHasProduct()
        {
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            Product orange = _shopsManager.CreateProduct("Orange");
            Product milk = _shopsManager.CreateProduct("Milk");
            shop1.AddProduct(orange, 10, 1.5);
            shop1.AddProduct(milk, 20, 5);
            Assert.AreEqual(shop1.GetProductInfo(orange).ProductRef, orange);
            Assert.AreEqual(shop1.GetProductInfo(milk).ProductRef, milk);
        }

        [Test]
        public void ChangeProductPrice_PriceChanged()
        {
            double startPrice = 1.5;
            double changedPrice = 1;
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            Product orange = _shopsManager.CreateProduct("Orange");
            shop1.AddProduct(orange, 10, startPrice);
            Assert.AreEqual(shop1.GetProductInfo(orange).ProductPrice, startPrice);
            shop1.ChangeProductPrice(orange, changedPrice);
            Assert.AreEqual(shop1.GetProductInfo(orange).ProductPrice, changedPrice);
        }

        [Test]
        public void FindBidWithUnrepresentedProducts_ThrowException()
        {
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            Product orange = _shopsManager.CreateProduct("Orange");
            Product milk = _shopsManager.CreateProduct("Milk");
            shop1.AddProduct(orange, 10, 1.5);
            shop1.AddProduct(milk, 1, 5);
            
            var wishList = new List<ProductConfiguration>();
            var orangeConfigRequest = new ProductConfiguration(orange, 5);
            var milkConfigRequest = new ProductConfiguration(milk, 2);
            wishList.Add(orangeConfigRequest);
            wishList.Add(milkConfigRequest);

            Assert.Catch<ShopsException>(() =>
            {
                Shop queryShop = _shopsManager.FindBestBid(wishList);
            });
        }

        [Test]
        public void MakePurchaseWithUnrepresentedProducts_ThrowException()
        {
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            var customer1 = new Person("Name Surname", 2000);
            Product orange = _shopsManager.CreateProduct("Orange");
            Product milk = _shopsManager.CreateProduct("Milk");
            shop1.AddProduct(orange, 10, 1.5);
            shop1.AddProduct(milk, 1, 5);
            
            var wishList = new List<ProductConfiguration>();
            var orangeConfigRequest = new ProductConfiguration(orange, 5);
            var milkConfigRequest = new ProductConfiguration(milk, 2);
            wishList.Add(orangeConfigRequest);
            wishList.Add(milkConfigRequest);

            Assert.Catch<ShopsException>(() =>
            {
                shop1.RegisterPurchase(customer1, wishList);
            });
        }

        [Test]
        public void MakePurchaseWithInsufficientBalance_ThrowException()
        {
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            var customer1 = new Person("Name Surname", 10);
            Product orange = _shopsManager.CreateProduct("Orange");
            shop1.AddProduct(orange, 50, 1.5);

            var wishList = new List<ProductConfiguration>();
            var orangeConfigRequest = new ProductConfiguration(orange, 20);
            wishList.Add(orangeConfigRequest);

            Assert.Catch<ShopsException>(() =>
            {
                shop1.RegisterPurchase(customer1, wishList);
            });
        }

        [Test]
        public void MakePurchaseSuccessfully_MoneyTransferredAndProductAmountChanged()
        {
            double startUserBalance = 1000;
            double productPrice = 1.5;
            int productCount = 50;
            int productCountToBuy = 20;
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            var customer1 = new Person("Name Surname", startUserBalance);
            Product orange = _shopsManager.CreateProduct("Orange");
            shop1.AddProduct(orange, productCount, productPrice);

            var wishList = new List<ProductConfiguration>();
            var orangeConfigRequest = new ProductConfiguration(orange, productCountToBuy);
            wishList.Add(orangeConfigRequest);

            double shop1BalanceBeforePurchase = shop1.Balance;
            shop1.RegisterPurchase(customer1, wishList);

            Assert.AreEqual(startUserBalance - productPrice * orangeConfigRequest.ProductAmount, customer1.Balance);
            Assert.AreEqual(shop1BalanceBeforePurchase + productPrice * orangeConfigRequest.ProductAmount, shop1.Balance);
            Assert.AreEqual(productCount - productCountToBuy, shop1.GetProductInfo(orange).ProductAmount);
        }

        [Test]
        public void CompareProductWithNonProductObject_ObjectsAreDifferent()
        {
            Shop shop1 = _shopsManager.CreateShop("Whole Foods");
            var customer1 = new Person("Name Surname", 1000);
            Product orange = _shopsManager.CreateProduct("Orange");
            Assert.AreEqual(orange.Equals(customer1), false);
        }
        
    }
}