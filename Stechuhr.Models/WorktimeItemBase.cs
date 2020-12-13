using System;
using System.Collections.Generic;
using System.Text;

namespace Stechuhr.Models
{
    public class WorktimeItemBase : IWorktimeSpan, IWorktimeType, ICloneable
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public WorktimeType WorktimeType { get; set; } = WorktimeType.R;

        public DateTime Date { get => StartTime.Date; }
        /// <summary>
        /// Overall Working Time with Pause Time
        /// </summary>
        public TimeSpan WorkTimeSpan => EndTime - StartTime;

        public virtual Object Clone(Object o)
        {
            ((WorktimeItemBase)o).id = Guid.NewGuid().ToString();
            ((WorktimeItemBase)o).StartTime = StartTime;
            ((WorktimeItemBase)o).EndTime = EndTime;
            return o;
        }

        public virtual object Clone()
        {
            return Clone(new WorktimeItemBase());
        }
    }
}
