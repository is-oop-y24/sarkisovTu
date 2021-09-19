namespace Isu.Models
{
    public class Group
    {
        private const int MaxStudentNumber = 35;
        private readonly string _name;
        private int _currentStudentNumber;

        public Group(string name)
        {
            _name = name;
            _currentStudentNumber = 0;
        }

        public bool HasFreePlaces()
        {
            return _currentStudentNumber < MaxStudentNumber;
        }

        public void TakeStudentPlace()
        {
            _currentStudentNumber++;
        }

        public string GetName()
        {
            return _name;
        }
    }
}