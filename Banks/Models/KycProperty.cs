using System.Collections.Generic;
using Banks.Types;

namespace Banks.Models
{
    public class KycProperty
    {
        public KycProperty(KycType type, string value)
        {
            Type = type;
            Value = value;
        }

        public KycType Type { get; private set; }
        public string Value { get; private set; }
    }
}