using Stechuhr.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stechuhr
{
    public class WorktimeItem : WorktimeItemBase
    {
        /// <summary>
        /// Defines Pause as Start to End Sections
        /// </summary>
        public List<PauseItem> Pause { get; set; } = new List<PauseItem>();
        /// <summary>
        /// Defines Pause as TimeSpan
        /// </summary>
        public TimeSpan PauseTime { get; set; } = new TimeSpan();

        /// <summary>
        /// Returns a Timespan of Overall Pause time
        /// </summary>
        public TimeSpan OverallPauseSpan
        {
            get 
            {
                TimeSpan ts = new TimeSpan();
                Pause.ForEach(t => ts += t.TimeSpan);
                return ts + PauseTime;
            }
        }

        public override Object Clone(Object o)
        {
            o = base.Clone(o);
            Pause.ForEach(t => ((WorktimeItem)o).Pause.Add((PauseItem)t.Clone()));
            return o;
        }

        public override object Clone()
        {
            return Clone(new WorktimeItem());
        }
    }

}
