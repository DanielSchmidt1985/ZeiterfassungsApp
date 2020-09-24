using System;
using System.Collections.Generic;
using System.Text;

namespace Stechuhr.Models
{
    public class WorktimeItemBase : IWorktimeSpan, ICloneable
    {
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        public virtual Object Clone(Object o)
        {
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
