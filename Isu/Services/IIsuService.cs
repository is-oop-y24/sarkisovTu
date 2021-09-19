using System.Collections.Generic;
using System.Text.RegularExpressions;
using Isu.Models;
using Isu.Tools;
using Group = Isu.Models.Group;

namespace Isu.Services
{
    public interface IIsuService
    {
        Group AddGroup(string name);
        Student AddStudent(Group group, string name);

        Student GetStudent(int id);
        Student FindStudent(string name);
        List<Student> FindStudents(string groupName);
        List<Student> FindStudents(CourseNumber courseNumber);
        Group FindGroup(string groupName);
        List<Group> FindGroups(CourseNumber courseNumber);

        void ChangeStudentGroup(Student student, Group newGroup);
    }

    public class IsuService : IIsuService
    {
        private const int IdPadding = 100000;
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
            var newGroup = new Group(name);
            _groupsList.Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            if (!group.HasFreePlaces()) throw new IsuException("Group hasn't empty places");
            int studentId = GenerateStudentId();
            var newStudent = new Student(group, name, studentId);
            group.TakeStudentPlace();
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
                if (student.GetStudentGroup().GetName()[2] == courseNumber.ToString()[0])
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
                if (group.GetName().ToString()[2] == courseNumber.ToString()[0])
                {
                    queryGroups.Add(group);
                }
            });
            return queryGroups;
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (!newGroup.HasFreePlaces()) throw new IsuException("Group hasn't empty places");
            student.ChangeGroup(newGroup);
        }

        private int GenerateStudentId()
        {
            int studentId = IdPadding + _studentsList.Count;
            return studentId;
        }
    }
}