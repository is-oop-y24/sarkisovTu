using System.Collections.Generic;
using System.Linq;
using Backups.Models;
using BackupsExtra.Tools;

namespace BackupsExtra.Models
{
    public class CapacityOptimization : IOptimizationAlgorithm
    {
        private int _capacity;

        public CapacityOptimization(int capacity)
        {
            if (capacity < 1) throw new BackupsExtraException("Provided capacity amount less than 1");
            _capacity = capacity;
        }

        public List<RestorePoint> Run(List<RestorePoint> restorePoints)
        {
            return Enumerable.Reverse(restorePoints).Take(_capacity).Reverse().ToList();
        }
    }
}