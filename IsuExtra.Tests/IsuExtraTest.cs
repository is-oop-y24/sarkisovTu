using Isu.Models;
using Isu.Services;
using IsuExtra.Models;
using IsuExtra.Repository;
using IsuExtra.Services;
using IsuExtra.Tools;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    public class Tests
    {
        private IsuService _isuService;
        private ElectiveStudentsRepository _studentsRepository;
        private ScheduleManager _scheduleManager;
        private ElectiveManager _electiveManager;
        
        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService();
            _studentsRepository = new ElectiveStudentsRepository();
            _scheduleManager = new ScheduleManager();
            _electiveManager = new ElectiveManager(_isuService, _scheduleManager, _studentsRepository);
            
            ElectiveCourse cyberSecurityCourse = _electiveManager.CreateElectiveCourse("N", "Cyber security basics", "KIB");
            ElectiveShift cyberSecurityShift1 = _electiveManager.CreateShiftGroup(cyberSecurityCourse);
            ElectiveGroup cyberSecurityGroup1 = _electiveManager.CreatePracticeGroup(cyberSecurityShift1, 10);

            ElectiveCourse photonicsCourse = _electiveManager.CreateElectiveCourse("L", "Photonics", "FTE");
            ElectiveShift photonicsShift1 = _electiveManager.CreateShiftGroup(photonicsCourse);
            ElectiveGroup photonicsGroup1 = _electiveManager.CreatePracticeGroup(photonicsShift1, 10);

            ElectiveCourse biotechnologyCourse = _electiveManager.CreateElectiveCourse("T", "Biotechnology", "FBT");
            ElectiveShift biotechnologyShift1 = _electiveManager.CreateShiftGroup(biotechnologyCourse);
            ElectiveGroup biotechnologyGroup1 = _electiveManager.CreatePracticeGroup(biotechnologyShift1, 10);
            
            var time1 = new ScheduleTime(1, "8:20", "9:50");
            var time2 = new ScheduleTime(1, "10:00", "11:30");
            var time3 = new ScheduleTime(1, "11:40", "13:10");
            var time4 = new ScheduleTime(1, "13:30", "15:00");

            Class cyberSecurityLecture1 = _scheduleManager.CreateClass(ClassType.Lecture, "Cyber security basics", time1, cyberSecurityShift1.Name);
            Class cyberSecurityPractice1 = _scheduleManager.CreateClass(ClassType.Practice, "Cyber security basics", time2, cyberSecurityGroup1.Name);

            Class photonicsLecture1 = _scheduleManager.CreateClass(ClassType.Lecture, "Photonics", time3, photonicsShift1.Name);
            Class photonicsPractice1 = _scheduleManager.CreateClass(ClassType.Practice, "Photonics", time4, photonicsGroup1.Name);

            Class biotechnologyLecture1 = _scheduleManager.CreateClass(ClassType.Lecture, "Biotechnology", time1, biotechnologyShift1.Name);
            Class biotechnologyPractice = _scheduleManager.CreateClass(ClassType.Practice, "Biotechnology", time2, biotechnologyGroup1.Name);

        }

        [Test]
        public void SubscribeToMoreThanTwoCourses_ThrowException()
        {
            Group m3208 = _isuService.AddGroup("M3208");
            Student student1 = _isuService.AddStudent(m3208, "Nikita Sarkisov");
            
            var time1 = new ScheduleTime(1, "17:00", "18:30");
            Class studentRegularClass1 = _scheduleManager.CreateClass(ClassType.Practice, "Physics", time1, m3208.Name);

            ElectiveGroup cyberSecurityGroup1 = _electiveManager.GetPracticeGroupByName("KIB 1.1");
            ElectiveGroup photonicsGroup1 = _electiveManager.GetPracticeGroupByName("FTE 1.1");
            ElectiveGroup biotechnologyGroup1 = _electiveManager.GetPracticeGroupByName("FBT 1.1");
            
            Assert.Catch<IsuExtraException>(() =>
            {
                _electiveManager.SubscribeCourse(student1, cyberSecurityGroup1);
                _electiveManager.SubscribeCourse(student1, photonicsGroup1);
                _electiveManager.SubscribeCourse(student1, biotechnologyGroup1);
            });
        }

        [Test]
        public void SubscribeToCourseWithIntersectionInSchedule_ThrowException()
        {
            Group m3208 = _isuService.AddGroup("M3208");
            Student student1 = _isuService.AddStudent(m3208, "Nikita Sarkisov");
            
            var time1 = new ScheduleTime(1, "10:00", "11:30");
            Class studentRegularClass1 = _scheduleManager.CreateClass(ClassType.Practice, "Mathematical analysis", time1, m3208.Name);
            
            ElectiveGroup cyberSecurityGroup1 = _electiveManager.GetPracticeGroupByName("KIB 1.1");

            Assert.Catch<IsuExtraException>(() =>
            {
                _electiveManager.SubscribeCourse(student1, cyberSecurityGroup1);
            });
        }

        [Test]
        public void SubscribeToCourseBelongsToSameFaculty_ThrowException()
        {
            Group n3211 = _isuService.AddGroup("N3211");
            Student student1 = _isuService.AddStudent(n3211, "Nikita Sarkisov");
            
            var time1 = new ScheduleTime(1, "11:40", "13:10");
            Class studentRegularClass = _scheduleManager.CreateClass(ClassType.Lecture, "Programing", time1, n3211.Name);

            ElectiveGroup cyberSecurityGroup1 = _electiveManager.GetPracticeGroupByName("KIB 1.1");
            
            Assert.Catch<IsuExtraException>(() =>
            { 
                _electiveManager.SubscribeCourse(student1, cyberSecurityGroup1);
            });
        }

        [Test]
        public void SubscribeStudentToFullFilledCourse_ThrowException()
        {
            Group m3208 = _isuService.AddGroup("M3208");
            
            ElectiveGroup cyberSecurityGroup1 = _electiveManager.GetPracticeGroupByName("KIB 1.1");

            Assert.Catch(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    Student student = _isuService.AddStudent(m3208, $"Student no.{i}");
                    _electiveManager.SubscribeCourse(student, cyberSecurityGroup1);
            
                }
            });
        }

        [Test]
        public void SubscribeAndUnsubscribeStudentToElective_StudentSubscribedAndUnsubscribed()
        {
            Group m3208 = _isuService.AddGroup("M3208");
            Student student1 = _isuService.AddStudent(m3208, "Nikita Sarkisov");
            
            ElectiveGroup cyberSecurityGroup1 = _electiveManager.GetPracticeGroupByName("KIB 1.1");
            
             _electiveManager.SubscribeCourse(student1, cyberSecurityGroup1);
             Assert.AreEqual(student1, _electiveManager.GetStudentsByGroup(cyberSecurityGroup1).Find(student => student.Equals(student1)));
            
             _electiveManager.UnsubscribeCourse(student1, cyberSecurityGroup1);
             Assert.AreEqual(null, _electiveManager.GetStudentsByGroup(cyberSecurityGroup1).Find(student => student.Equals(student1)));
        }
    }
}