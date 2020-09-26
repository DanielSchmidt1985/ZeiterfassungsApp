using System;
using System.Collections.Generic;

namespace Stechuhr.Settings
{
    public class WorktimeSettings
    {
        public TimeSpan RegularWorkingTime { get; set; }
        public List<DayOfWeek> RegularWorkingDays { get; set; }

        public WorktimeSettings()
        {
            RegularWorkingTime = new TimeSpan(8, 0, 0);
            RegularWorkingDays = new List<DayOfWeek>(new List<DayOfWeek>() 
            { 
                DayOfWeek.Monday, 
                DayOfWeek.Tuesday, 
                DayOfWeek.Wednesday, 
                DayOfWeek.Thursday, 
                DayOfWeek.Friday 
            });
        }
    }
}
