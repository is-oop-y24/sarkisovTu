using System.Collections.Generic;
using Backups.Models;

namespace BackupsExtra.Models
{
    public interface IOptimizationAlgorithm
    {
        List<RestorePoint> Run(List<RestorePoint> restorePoints);
    }
}