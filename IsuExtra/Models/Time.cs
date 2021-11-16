using IsuExtra.Tools;

namespace IsuExtra.Models
{
    public class Time
    {
        public Time(int hours, int minutes)
        {
            if (hours > 23 || minutes > 59)
            {
                throw new IsuExtraException("Invalid time specified");
            }

            Hours = hours;
            Minutes = minutes;
        }

        public int Hours { get; private set;  }
        public int Minutes { get; private set; }

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}", this.Hours, this.Minutes);
        }
    }
}