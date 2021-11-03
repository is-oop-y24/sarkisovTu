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

        public override bool Equals(object other)
        {
            if (other.GetType().Name != "Product") return false;
            return Equals(other);
        }

        public bool Equals(Product other)
        {
            if (other == null) return false;
            if (this._id == other._id) return true;
            if (ReferenceEquals(this, other)) return true;
            return false;
        }
    }
}