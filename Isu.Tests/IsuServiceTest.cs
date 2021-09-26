using Isu.Models;
using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    public class Tests
    {
        private IIsuService _isuService;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService();
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            Group m3101 = _isuService.AddGroup("M3101");
            _isuService.AddStudent(m3101, "Name Surname");
            Assert.Pass();
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Group m3101 = _isuService.AddGroup("M3101");
            
            Assert.Catch<IsuException>(() =>
            {
                //max student number in group is 35. Try to add 4o students in one group
                for (int i = 0; i < 40; i++)
                {
                    _isuService.AddStudent(m3101, $"Student{i}");
                }
            });
        }

        [Test]
        public void CreateGroupWithInvalidName_ThrowException()
        {
            //group name template: M3XYY, X - course number, YY - group number
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddGroup("13201");
            });
        }
    }
}