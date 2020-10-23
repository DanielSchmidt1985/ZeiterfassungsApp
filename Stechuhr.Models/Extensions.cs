using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stechuhr.Views
{
    static public class Extensions
    {
        public static WorktimeItemCollection FromMonth(this WorktimeItemCollection Worktimes, DateTime Month)
        {
            DateTime from = new DateTime(Month.Year, Month.Month, 1);
            DateTime to = new DateTime(Month.Year, Month.Month, DateTime.DaysInMonth(Month.Year, Month.Month));
            return Worktimes.Where(t => t.Date >= from && t.Date <= to.Date).ToWorkitemCollection();
        }

        public static WorktimeItemCollection ToWorkitemCollection(this IEnumerable<WorktimeItem> Worktimes)
        {
            var wt = new WorktimeItemCollection();
            wt.AddRange(Worktimes);
            return wt;
        }

        public static string Format(this TimeSpan ts)
        {
            string sign = ts.TotalMinutes < 0 ? "-" : "";
            if (ts.TotalMinutes == 0) return "";
            return string.Format("{0}{1:D2}:{2:D2}", sign, Math.Abs(ts.Hours) + 24 * Math.Abs(ts.Days), Math.Abs(ts.Minutes));
        }
    }
}
