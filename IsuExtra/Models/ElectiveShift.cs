using System.Collections.Generic;

namespace IsuExtra.Models
{
    public class ElectiveShift
    {
        public ElectiveShift(string name, ElectiveCourse courseRef)
        {
            Name = name;
            CourseRef = courseRef;
            Groups = new List<ElectiveGroup>();
        }

        public string Name { get; private set; }

        public ElectiveCourse CourseRef { get; private set; }

        public List<ElectiveGroup> Groups { get; private set; }

        public int MaxStudentNumber
        {
            get { return ComputeMaxStudentNumber(); }
        }

        public void AddElectiveGroup(ElectiveGroup group)
        {
            Groups.Add(group);
        }

        private int ComputeMaxStudentNumber()
        {
            int sum = 0;
            foreach (ElectiveGroup group in Groups)
            {
                sum += group.MaxStudentNumber;
            }

            return sum;
        }
    }
}