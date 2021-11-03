namespace IsuExtra.Models
{
    public class ScheduleTime
    {
        public ScheduleTime(int day, string startTime, string endTime)
        {
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
        }

        public int Day { get; private set; }

        public string StartTime { get; private set; }

        public string EndTime { get; private set; }
    }
}