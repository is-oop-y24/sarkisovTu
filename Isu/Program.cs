using System;
using System.Collections.Generic;
using Isu.Models;
using Isu.Services;

namespace Isu
{
    internal class Program
    {
        private static void Main()
        {
            var service = new IsuService();
            CourseNumber firstYearCourse = 1;
            Group m3101 = service.AddGroup("M3101");
            Group m3102 = service.AddGroup("M3102");
            Student isStudent1 = service.AddStudent(m3101, "Name Surname");
            int isStudent1Id = isStudent1.GetStudentId();

            Student queryStudentById = service.GetStudent(isStudent1Id);
            Console.WriteLine(queryStudentById.GetStudentName());

            Student queryStudentByName = service.FindStudent("Name Surname");
            Console.WriteLine(queryStudentByName.GetStudentName());

            List<Student> queryStudentsByGroupName = service.FindStudents("M3101");
            queryStudentsByGroupName.ForEach(student =>
            {
                Console.WriteLine($"{student.GetStudentGroup().GetName()} - {student.GetStudentName()}");
            });

            List<Student> queryStudentsByCourseNumber = service.FindStudents(firstYearCourse);
            queryStudentsByCourseNumber.ForEach(student =>
            {
                Console.WriteLine($"{student.GetStudentGroup().GetName()} - {student.GetStudentName()}");
            });

            Group queryGroup = service.FindGroup("M3101");
            Console.WriteLine(queryGroup.GetName());

            List<Group> queryGroupsByCourseNumber = service.FindGroups(firstYearCourse);
            queryGroupsByCourseNumber.ForEach(group =>
            {
                Console.WriteLine(group.GetName());
            });

            service.ChangeStudentGroup(isStudent1, m3102);
        }
    }
}
