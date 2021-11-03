using System.Collections.Generic;
using System.Linq;
using Isu.Models;
using IsuExtra.Models;

namespace IsuExtra.Repository
{
    public class ElectiveStudentsRepository
    {
        private readonly List<ElectiveStudent> _studentsList;

        public ElectiveStudentsRepository()
        {
            _studentsList = new List<ElectiveStudent>();
        }

        public void Add(ElectiveStudent student)
        {
            _studentsList.Add(student);
        }

        public List<ElectiveStudent> GetAll()
        {
            return _studentsList;
        }

        public void Delete(ElectiveStudent student)
        {
            _studentsList.Remove(student);
        }

        public List<ElectiveStudent> FindByGroup(ElectiveGroup group)
        {
            return _studentsList.Where(student => student.JoinedGroups.Contains(group)).ToList();
        }

        public ElectiveStudent FindById(int id)
        {
            return _studentsList.Find(student => student.StudentRef.Id == id);
        }
    }
}