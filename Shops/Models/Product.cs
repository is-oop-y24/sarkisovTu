using System;
#pragma warning disable 659

namespace Shops.Models
{
    public class Product : IEquatable<Product>
    {
        private readonly string _name;
        private int _id;

        public Product(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public override bool Equals(object other)
        {
            return Equals(other);
        }

        public bool Equals(Product other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return false;
        }
    }
}