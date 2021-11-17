using System;
using Isu.Models;
using IsuExtra.Tools;

namespace IsuExtra.Models
{
    public class Class
    {
        private readonly ClassType _type;
        private readonly string _name;
        private readonly ScheduleTime _time;
        private readonly string _groupId;

        public Class(ClassType type, string name, ScheduleTime time, string groupId)
        {
            _type = type;
            _name = name;
            _time = time;
            _groupId = groupId;
        }

        public string Group
        {
            get { return _groupId; }
        }

        public ScheduleTime Time
        {
            get { return _time; }
        }
    }
}