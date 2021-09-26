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
            const string regexGroupPattern = @"^[mM]{1}[3]{1}\d{3}$";
            if (!Regex.IsMatch(name, regexGroupPattern)) throw new IsuException($"{name} is invalid group name");
            name = char.ToUpper(name[0]) + name.Substring(1);
            _groupsList.ForEach(group =>
            {
                if (group.GetName() == name) throw new IsuException($"{name} group is already exist");
            });
            var newGroup = new Group(name, MaxStudentNumberInGroup);
            _groupsList.Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            if (!HasGroupFreePlaces(group)) throw new IsuException("Group hasn't empty places");
            int studentId = GenerateStudentId();
            var newStudent = new Student(group, name, studentId);
            _studentsList.Add(newStudent);
            return newStudent;
        }

        public Student GetStudent(int id)
        {
            Student queryStudent = null;
            const string regexIdPattern = @"^\d{6}$";
            if (!Regex.IsMatch(id.ToString(), regexIdPattern)) throw new IsuException($"{id} is invalid student id");
            _studentsList.ForEach(student =>
            {
                if (student.GetStudentId() == id)
                {
                    queryStudent = student;
                }
            });
            return queryStudent;
        }

        public Student FindStudent(string name)
        {
            Student queryStudent = null;
            _studentsList.ForEach(student =>
            {
                if (student.GetStudentName() == name)
                {
                    queryStudent = student;
                }
            });
            return queryStudent;
        }

        public List<Student> FindStudents(string groupName)
        {
            const string regexGroupPattern = @"^[mM]{1}[3]{1}\d{3}$";
            if (!Regex.IsMatch(groupName, regexGroupPattern)) throw new IsuException($"{groupName} is invalid group name");

            var queryStudent = new List<Student>();
            _studentsList.ForEach(student =>
            {
                if (student.GetStudentGroup().GetName() == groupName)
                {
                    queryStudent.Add(student);
                }
            });
            return queryStudent;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var queryStudents = new List<Student>();
            _studentsList.ForEach(student =>
            {
                if (student.GetStudentGroup().GetName()[2] - '0' == (int)courseNumber)
                {
                    queryStudents.Add(student);
                }
            });
            return queryStudents;
        }

        public Group FindGroup(string groupName)
        {
            Group queryGroup = null;
            _groupsList.ForEach(group =>
            {
                if (group.GetName() == groupName)
                {
                    queryGroup = group;
                }
            });
            return queryGroup;
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            var queryGroups = new List<Group>();
            _groupsList.ForEach(group =>
            {
                if (group.GetName().ToString()[2] - '0' == (int)courseNumber)
                {
                    queryGroups.Add(group);
                }
            });
            return queryGroups;
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (student.GetStudentGroup().Equals(newGroup)) return;
            if (!HasGroupFreePlaces(newGroup)) throw new IsuException("Group hasn't empty places");
            int studentId = student.GetStudentId();
            string studentName = student.GetStudentName();
            Student studentToRemove = _studentsList.Single(element => element.GetStudentId() == student.GetStudentId());
            _studentsList.Remove(studentToRemove);
            _studentsList.Add(new Student(newGroup, studentName, studentId));
        }

        private int GenerateStudentId()
        {
            int studentId = IdPadding + _studentsList.Count;
            return studentId;
        }

        private bool HasGroupFreePlaces(Group queryGroup)
        {
            int studentNumberInNewGroup = 0;
            _studentsList.ForEach(student =>
            {
                if (student.GetStudentGroup().Equals(queryGroup))
                {
                    studentNumberInNewGroup++;
                }
            });
            return studentNumberInNewGroup < queryGroup.GetMaxStudentNumber();
        }
    }
}