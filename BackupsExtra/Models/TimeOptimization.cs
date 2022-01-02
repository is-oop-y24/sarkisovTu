using System;
using System.Collections.Generic;
using Backups.Models;

namespace BackupsExtra.Models
{
    public class TimeOptimization : IOptimizationAlgorithm
    {
        private DateTime _date;

        public TimeOptimization(DateTime date)
        {
            _date = date;
        }

        public List<RestorePoint> Run(List<RestorePoint> restorePoints)
        {
            return restorePoints.FindAll(restorePoint => DateTime.Compare(restorePoint.DateOfCreation, _date) >= 0);
        }
    }
}