namespace IsuExtra.Models
{
    public class ScheduleTime
    {
        public ScheduleTime(int day, Time startTime, Time endTime)
        {
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
        }

        public int Day { get; private set; }

        public Time StartTime { get; private set; }

        public Time EndTime { get; private set; }
    }
}