using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Isu.Models;
using Isu.Tools;
using Group = Isu.Models.Group;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private const int IdPadding = 100000;
        private const int MaxStudentNumberInGroup = 35;
        private readonly List<Group> _groupsList;
        private readonly List<Student> _studentsList;
        public IsuService()
        {
            _groupsList = new List<Group>();
            _studentsList = new List<Student>();
        }

        public Group AddGroup(string name)
        {
            name = char.ToUpper(name[0]) + name.Substring(1);
            _groupsList.ForEach(group =>
            {
                if (group.Name == name) throw new IsuException($"{name} group is already exist");
            });
            var newGroup = new Group(name, MaxStudentNumberInGroup);
            _groupsList.Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            if (!HasGroupFreePlaces(group)) throw new IsuException("Group hasn't empty places");
            int studentId = GenerateStudentId();
            var newStudent = new Student(studentId, name, group);
            _studentsList.Add(newStudent);
            return newStudent;
        }

        public Student GetStudent(int id)
        {
            if (!IsValidId(id)) throw new IsuException($"{id} is invalid student id");
            Student queryStudent = _studentsList.Find(student => student.Id == id);
            return queryStudent;
        }

        public Student FindStudent(string name)
        {
            Student queryStudent = _studentsList.Find(student => student.Name == name);
            return queryStudent;
        }

        public List<Student> FindStudents(string groupName)
        {
            groupName = char.ToUpper(groupName[0]) + groupName.Substring(1);
            var queryStudent = _studentsList.Where(student => student.Group.Name == groupName).ToList();
            return queryStudent;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var queryStudent = _studentsList.Where(student => student.Group.Name[2] - '0' == (int)courseNumber).ToList();
            return queryStudent;
        }

        public Group FindGroup(string groupName)
        {
            groupName = char.ToUpper(groupName[0]) + groupName.Substring(1);
            Group queryGroup = _groupsList.Find(group => group.Name == groupName);
            return queryGroup;
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            var queryGroups = _groupsList.Where(group => group.Name.ToString()[2] - '0' == (int)courseNumber).ToList();
            return queryGroups;
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (student.Group.Equals(newGroup)) return;
            if (!HasGroupFreePlaces(newGroup)) throw new IsuException("Group hasn't empty places");
            int studentId = student.Id;
            string studentName = student.Name;
            Student studentToRemove = _studentsList.Single(element => element.Id == student.Id);
            _studentsList.Remove(studentToRemove);
            _studentsList.Add(new Student(studentId, studentName, newGroup));
        }

        private int GenerateStudentId()
        {
            int studentId = IdPadding + _studentsList.Count;
            return studentId;
        }

        private bool HasGroupFreePlaces(Group queryGroup)
        {
            int studentNumberInNewGroup = _studentsList.Count(student => student.Group.Equals(queryGroup));
            return studentNumberInNewGroup < queryGroup.MaxStudentNumber;
        }

        private bool IsValidId(int id)
        {
            const string regexIdPattern = @"^\d{6}$";
            return Regex.IsMatch(id.ToString(), regexIdPattern);
        }
    }
}