using System;
#pragma warning disable 659

namespace Isu.Models
{
    public class Group : IEquatable<Group>
    {
        private readonly string _name;
        private readonly int _maxStudentNumber;

        public Group(string name, int maxStudentNumber)
        {
            _name = name;
            _maxStudentNumber = maxStudentNumber;
        }

        public string GetName()
        {
            return _name;
        }

        public int GetMaxStudentNumber()
        {
            return _maxStudentNumber;
        }

        public override bool Equals(object other)
        {
            return Equals(other);
        }

        public bool Equals(Group other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return false;
        }
    }
}