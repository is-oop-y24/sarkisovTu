namespace IsuExtra.Models
{
    public class ElectiveGroup
    {
        public ElectiveGroup(string name, int maxStudentNumber, ElectiveShift shiftRef)
        {
            Name = name;
            MaxStudentNumber = maxStudentNumber;
            ShiftRef = shiftRef;
        }

        public string Name { get; private set; }

        public int MaxStudentNumber { get; private set; }

        public ElectiveShift ShiftRef { get; private set; }
    }
}