using System;
using System.Text.RegularExpressions;
using Isu.Tools;

#pragma warning disable 659

namespace Isu.Models
{
    public class Group : IEquatable<Group>
    {
        private readonly string _name;
        private readonly int _maxStudentNumber;

        public Group(string name, int maxStudentNumber)
        {
            if (!ValidateName(name)) throw new IsuException($"{name} is invalid group name");
            _name = name;
            _maxStudentNumber = maxStudentNumber;
        }

        public string Name
        {
            get { return _name; }
        }

        public int MaxStudentNumber
        {
            get { return _maxStudentNumber; }
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

        private bool ValidateName(string groupName)
        {
            const string regexGroupPattern = @"^[a-zA-Z]{1}[3]{1}\d{3}$";
            return Regex.IsMatch(groupName, regexGroupPattern);
        }
    }
}