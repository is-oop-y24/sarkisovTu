using System.Collections.Generic;
using Isu.Models;

namespace IsuExtra.Models
{
    public class ElectiveStudent
    {
        private readonly Student _studentRef;
        private readonly List<ElectiveGroup> _electiveGroups;

        public ElectiveStudent(Student studentRef)
        {
            _studentRef = studentRef;
            _electiveGroups = new List<ElectiveGroup>();
        }

        public Student StudentRef
        {
            get { return _studentRef; }
        }

        public List<ElectiveGroup> JoinedGroups
        {
            get { return _electiveGroups; }
        }

        public void AddElective(ElectiveGroup group)
        {
            _electiveGroups.Add(group);
        }

        public void RemoveElective(ElectiveGroup group)
        {
            _electiveGroups.Remove(group);
        }
    }
}