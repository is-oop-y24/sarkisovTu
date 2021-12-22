using System;
using BackupsExtra.Tools;
using BackupsExtra.Types;

namespace BackupsExtra.Models
{
    public class OptimizationConfiguration
    {
        public OptimizationConfiguration(int capacity, bool mergeEnabled)
        {
            if (!IsCapacityValid(capacity)) throw new BackupsExtraException("Provided capacity amount less than 1");
            IsHybrid = false;
            Capacity = capacity;
            MergeEnabled = mergeEnabled;
        }

        public OptimizationConfiguration(DateTime date, bool mergeEnabled)
        {
            IsHybrid = false;
            Date = date;
            MergeEnabled = mergeEnabled;
        }

        public OptimizationConfiguration(int capacity, DateTime date, RestorePointOptimizationType optimizationType, bool mergeEnabled)
        {
            if (!IsCapacityValid(capacity)) throw new BackupsExtraException("Provided capacity amount less than 1");
            IsHybrid = true;
            Capacity = capacity;
            Date = date;
            OptimizationType = optimizationType;
            MergeEnabled = mergeEnabled;
        }

        public int Capacity { get; private set; }
        public DateTime Date { get; private set; }
        public RestorePointOptimizationType OptimizationType { get; private set; }
        public bool IsHybrid { get; private set; }

        public bool MergeEnabled { get; private set; }

        public bool IsCapacityValid(int capacity)
        {
            return capacity >= 1;
        }
    }
}