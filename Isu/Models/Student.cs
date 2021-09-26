namespace Isu.Models
{
    public class Student
    {
        private readonly int _id;
        private readonly string _name;
        private Group _group;

        public Student(Group group, string name, int id)
        {
            _id = id;
            _name = name;
            _group = group;
        }

        public int GetStudentId()
        {
            return _id;
        }

        public string GetStudentName()
        {
            return _name;
        }

        public Group GetStudentGroup()
        {
            return _group;
        }
    }
}