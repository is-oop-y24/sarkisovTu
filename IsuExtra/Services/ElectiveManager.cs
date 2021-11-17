using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Isu.Models;
using Isu.Services;
using IsuExtra.Models;
using IsuExtra.Repository;
using IsuExtra.Tools;
using Group = Isu.Models.Group;

namespace IsuExtra.Services
{
    public class ElectiveManager
    {
        private readonly List<ElectiveCourse> _coursesList;
        private readonly IsuService _isuService;
        private readonly ScheduleManager _scheduleManager;
        private readonly ElectiveStudentsRepository _studentsRepository;

        public ElectiveManager(IsuService isuService, ScheduleManager scheduleManager, ElectiveStudentsRepository studentsRepository)
        {
            _coursesList = new List<ElectiveCourse>();
            _isuService = isuService;
            _scheduleManager = scheduleManager;
            _studentsRepository = studentsRepository;
        }

        public ElectiveCourse CreateElectiveCourse(string letter, string name, string shortName)
        {
            var newCourse = new ElectiveCourse(letter, name, shortName);
            _coursesList.Add(newCourse);
            return newCourse;
        }

        public ElectiveShift CreateShiftGroup(ElectiveCourse course)
        {
            string name = course.ShortName + " " + (course.Shifts.Count + 1);
            var newShift = new ElectiveShift(name, course);
            course.AddElectiveShift(newShift);
            return newShift;
        }

        public ElectiveGroup CreatePracticeGroup(ElectiveShift shift, int maxStudentNumber)
        {
            string name = shift.Name + "." + (shift.Groups.Count + 1);
            var newPracticeGroup = new ElectiveGroup(name, maxStudentNumber, shift);
            shift.AddElectiveGroup(newPracticeGroup);
            return newPracticeGroup;
        }

        public ElectiveGroup GetPracticeGroupByName(string groupName)
        {
            const string electiveGroupNamePattern = @"^[A-Z]+\s\d\.\d$";
            if (!Regex.IsMatch(groupName, electiveGroupNamePattern)) throw new IsuExtraException("Invalid group name");
            string[] groupNameParts = groupName.Split(" ");

            ElectiveCourse queryCourse = _coursesList.Find(course => course.ShortName == groupNameParts[0]);

            if (queryCourse == null) return null;
            if (int.Parse(groupNameParts[1].Split(".")[0]) > queryCourse.Shifts.Count) return null;
            ElectiveShift queryShift = queryCourse.Shifts[int.Parse(groupNameParts[1].Split(".")[0]) - 1];
            if (int.Parse(groupNameParts[1].Split(".")[1]) > queryShift.Groups.Count) return null;
            ElectiveGroup queryGroup = queryShift.Groups[int.Parse(groupNameParts[1].Split(".")[1]) - 1];
            return queryGroup;
        }

        public List<ElectiveShift> GetCoursesShifts(ElectiveCourse course)
        {
            return course.Shifts;
        }

        public List<Student> GetStudentsByGroup(ElectiveGroup group)
        {
            return _studentsRepository.FindByGroup(group).Select(student => student.StudentRef).ToList();
        }

        public List<Student> GetUnsubscribedStudents(Group group)
        {
            List<Student> queryAllStudents = _isuService.FindStudents(group.Name);
            List<Student> querySubscribedStudents = _studentsRepository.GetAll().Select(student => student.StudentRef).ToList();
            return queryAllStudents.Except(querySubscribedStudents).ToList();
        }

        public void SubscribeCourse(Student student, ElectiveGroup group)
        {
            if (student.Group.Name[0] == group.ShiftRef.CourseRef.Letter[0])
                throw new IsuExtraException("Elective belongs to same faculty");
            if (!_scheduleManager.IsElectiveAvailable(student.Group, group))
                throw new IsuExtraException("There are intersections in schedule");
            if (group.MaxStudentNumber <= _studentsRepository.FindByGroup(group).Count)
                throw new IsuExtraException("Selected course hasn't free places");
            ElectiveStudent queryStudent = _studentsRepository.FindById(student.Id);
            if (queryStudent != null && queryStudent.JoinedGroups.Count >= 2)
                throw new IsuExtraException("Student has already joined 2 courses");
            if (queryStudent == null)
            {
                var newElectiveStudent = new ElectiveStudent(student);
                newElectiveStudent.AddElective(group);
                _studentsRepository.Add(newElectiveStudent);
            }
            else
            {
                queryStudent.AddElective(group);
            }
        }

        public void UnsubscribeCourse(Student student, ElectiveGroup group)
        {
            ElectiveStudent queryStudent = _studentsRepository.FindById(student.Id);
            queryStudent.RemoveElective(group);
            if (queryStudent.JoinedGroups.Count == 0) _studentsRepository.Delete(queryStudent);
        }
    }
}