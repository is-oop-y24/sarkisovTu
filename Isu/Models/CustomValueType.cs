using System.Text.RegularExpressions;
using Isu.Tools;

namespace Isu.Models
{
    public class CustomValueType<TValue>
    {
        private TValue _value;

        protected CustomValueType(TValue value)
        {
            const string regexCoursePattern = @"^[1-6]{1}$";
            if (!Regex.IsMatch(value.ToString() ?? throw new IsuException($"Empty argument passed"), regexCoursePattern)) throw new IsuException($"{value} is invalid course value");
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        protected TValue GetValue()
        {
            return _value;
        }
    }
}