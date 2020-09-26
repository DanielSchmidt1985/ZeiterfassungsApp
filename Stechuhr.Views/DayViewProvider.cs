using Stechuhr.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stechuhr.Views
{
    public class DayViewProvider
    {
        public string FilePath { get; set; }
        
        public WorktimeSettings WorktimeSettings { get; }
        public WorktimeProvider WorktimeProvider { get; }


        public DayViewProvider(WorktimeProvider WorktimeProvider, WorktimeSettings WorktimeSettings)
        {
            this.WorktimeProvider = WorktimeProvider;
            this.WorktimeSettings = WorktimeSettings;
        }

        public List<DayView> CreateMonthView(DateTime Month)
        {
            // Load stamped worktime data
            WorktimeProvider.LoadWorktimeData();

            // Make a empty Month
            List<DayView> days = new List<DayView>();
            for (int i = 1; i < DateTime.DaysInMonth(Month.Year, Month.Month) + 1; i++)
            {
                days.Add(new DayView(WorktimeProvider, WorktimeSettings, new DateTime(Month.Year, Month.Month, i)));

            }

            // Get stamped data from the month
            WorktimeItemCollection Worktimes = WorktimeProvider.Worktimes.FromMonth(Month);
            // and sort in 
            foreach (WorktimeItem item in Worktimes)
            {
                DayView day = days.Find(t => t.Date.Day == item.Date.Day);
                if (day != null)
                {
                    day.FromWorktimeItem(item);
                }
            }

            return days;
        }
    }
}
