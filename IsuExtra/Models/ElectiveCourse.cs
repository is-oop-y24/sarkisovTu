using System.Collections.Generic;

namespace IsuExtra.Models
{
    public class ElectiveCourse
    {
        public ElectiveCourse(string letter, string name, string shortName)
        {
            Letter = letter;
            Name = name;
            ShortName = shortName;
            Shifts = new List<ElectiveShift>();
        }

        public string Letter { get; private set; }

        public string Name { get; private set; }

        public List<ElectiveShift> Shifts { get; private set; }

        public string ShortName { get; private set; }

        public void AddElectiveShift(ElectiveShift shift)
        {
            Shifts.Add(shift);
        }
    }
}