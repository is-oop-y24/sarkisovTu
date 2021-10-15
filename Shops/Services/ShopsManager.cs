using System.Collections.Generic;
using Shops.Models;
using Shops.Tools;

namespace Shops.Services
{
    public class ShopsManager
    {
        private const int IdPaddingShop = 0;
        private const int IdPaddingProduct = 0;
        private readonly List<Shop> _shopsList;
        private readonly List<Product> _productBase;

        public ShopsManager()
        {
            _shopsList = new List<Shop>();
            _productBase = new List<Product>();
        }

        public Shop CreateShop(string name)
        {
            int shopId = GenerateId(IdPaddingShop, _shopsList.Count);
            var newShop = new Shop(shopId, name);
            _shopsList.Add(newShop);
            return newShop;
        }

        public Shop FindBestBid(List<ProductConfiguration> productWishList)
        {
            double[] checkOffers = new double[_shopsList.Count];
            for (int i = 0; i < _shopsList.Count; i++)
            {
                checkOffers[i] = 0;
            }

            foreach (Shop shop in _shopsList)
            {
                foreach (ProductConfiguration requestProduct in productWishList)
                {
                    ProductConfiguration shopProduct = shop.GetProductStock().Find(product => product.ProductRef.Equals(requestProduct.ProductRef));
                    if (shopProduct != null && shopProduct.ProductAmount >= requestProduct.ProductAmount)
                    {
                        checkOffers[_shopsList.IndexOf(shop)] += shopProduct.ProductPrice * requestProduct.ProductAmount;
                    }
                    else
                    {
                        checkOffers[_shopsList.IndexOf(shop)] = -1;
                        break;
                    }
                }
            }

            int bestBidShopIndex = 0;
            int rejectedShopsCount = 0;
            for (int i = 0; i < checkOffers.Length; i++)
            {
                if ((int)checkOffers[i] == -1)
                {
                    rejectedShopsCount++;
                }

                if (checkOffers[i] < checkOffers[bestBidShopIndex] && (int)checkOffers[i] != -1)
                {
                    bestBidShopIndex = i;
                }
            }

            if (rejectedShopsCount == _shopsList.Count) throw new ShopsException("No suitable offers were found");

            return _shopsList[bestBidShopIndex];
        }

        public Product CreateProduct(string name)
        {
            int productId = GenerateId(IdPaddingProduct, _productBase.Count);
            var newProduct = new Product(productId, name);
            _productBase.Add(newProduct);
            return newProduct;
        }

        private int GenerateId(int padding, int numberInList)
        {
            return padding + numberInList;
        }
    }
}