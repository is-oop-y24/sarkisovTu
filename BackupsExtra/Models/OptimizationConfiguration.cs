using System;
using System.Collections.Generic;
using BackupsExtra.Tools;
using BackupsExtra.Types;

namespace BackupsExtra.Models
{
    public class OptimizationConfiguration
    {
        public OptimizationConfiguration(List<IOptimizationAlgorithm> algorithms, bool mergeEnabled)
        {
            Algorithms = algorithms;
            MergeEnabled = mergeEnabled;
        }

        public List<IOptimizationAlgorithm> Algorithms { get; private set; }
        public bool MergeEnabled { get; private set; }
    }
}