namespace Isu.Models
{
    public class Student
    {
        private readonly int _id;
        private readonly string _name;
        private Group _group;

        public Student(int id, string name, Group group)
        {
            _id = id;
            _name = name;
            _group = group;
        }

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Group Group
        {
            get { return _group; }
        }
    }
}