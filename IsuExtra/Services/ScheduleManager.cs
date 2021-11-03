using System.Collections.Generic;
using System.Linq;
using Isu.Models;
using IsuExtra.Models;

namespace IsuExtra.Services
{
    public class ScheduleManager
    {
        private readonly List<Class> _classesList;

        public ScheduleManager()
        {
            _classesList = new List<Class>();
        }

        public Class CreateClass(ClassType type, string name, ScheduleTime time, string groupId)
        {
            Class newClass = new Class(type, name, time, groupId);
            _classesList.Add(newClass);
            return newClass;
        }

        public List<Class> FindClasses(string groupName)
        {
            return _classesList.Where(curClass => curClass.Group == groupName).ToList();
        }

        public bool IsElectiveAvailable(Group studentGroup, ElectiveGroup electiveGroup)
        {
            List<Class> queryClassesByStudentGroup = FindClasses(studentGroup.Name);
            List<Class> queryClassesByElectiveGroup = new List<Class>();
            queryClassesByElectiveGroup.AddRange(FindClasses(electiveGroup.Name));
            queryClassesByElectiveGroup.AddRange(FindClasses(electiveGroup.ShiftRef.Name));
            for (int i = 0; i < queryClassesByStudentGroup.Count; i++)
            {
                for (int j = 0; j < queryClassesByElectiveGroup.Count; j++)
                {
                    if (HasIntersection(queryClassesByStudentGroup[i], queryClassesByElectiveGroup[j])) return false;
                }
            }

            return true;
        }

        private bool HasIntersection(Class first, Class second)
        {
            if (first.Time.Day != second.Time.Day) return false;

            int thisStartTime = int.Parse(first.Time.StartTime.Replace(":", string.Empty));
            int thisEndTime = int.Parse(first.Time.EndTime.Replace(":", string.Empty));

            int otherStartTime = int.Parse(second.Time.StartTime.Replace(":", string.Empty));
            int otherEndTime = int.Parse(second.Time.EndTime.Replace(":", string.Empty));

            if (thisEndTime >= otherStartTime && thisEndTime <= otherEndTime)
            {
                return true;
            }

            if (thisStartTime >= otherStartTime && thisStartTime <= otherEndTime)
            {
                return true;
            }

            return false;
        }
    }
}