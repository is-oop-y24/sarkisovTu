using System.Collections.Generic;
using Banks.Tools;
using Banks.Types;

namespace Banks.Models
{
    public class BankClient
    {
        public BankClient(string name, string surname, Dictionary<KycType, string> kycDictionary)
        {
            Name = name;
            Surname = surname;
            KycDictionary = kycDictionary;
        }

        private BankClient(Builder builder)
        {
            Id = builder.Id;
            Name = builder.Name;
            Surname = builder.Surname;
            KycDictionary = builder.KycDictionary;
        }

        public int Id { get; }
        public string Name { get; }
        public string Surname { get; }
        public Dictionary<KycType, string> KycDictionary { get; }

        public void AddNewVerificationProperty(KycType type, string value)
        {
            if (KycDictionary.ContainsKey(type)) throw new BanksException("Provided verification type was already specified");
            KycDictionary.Add(type, value);
        }

        public bool HasVerification()
        {
            if (KycDictionary.Count != 0) return true;
            return false;
        }

        public class Builder
        {
            public Builder(int id, string name, string surname)
            {
                Id = id;
                Name = name;
                Surname = surname;
                KycDictionary = new Dictionary<KycType, string>();
            }

            internal int Id { get; private set; }
            internal string Name { get; private set; }
            internal string Surname { get; private set; }
            internal Dictionary<KycType, string> KycDictionary { get; }

            public Builder SetNewVerificationProperty(KycType type, string value)
            {
                KycDictionary.Add(type, value);
                return this;
            }

            public BankClient Build()
            {
                return new BankClient(this);
            }
        }
    }
}