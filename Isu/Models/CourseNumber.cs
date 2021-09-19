namespace Isu.Models
{
    public class CourseNumber : CustomValueType<int>
    {
        private CourseNumber(int value)
            : base(value) { }
        public static implicit operator CourseNumber(int value) { return new CourseNumber(value); }
        public static implicit operator long(CourseNumber custom) { return custom.GetValue(); }
    }
}