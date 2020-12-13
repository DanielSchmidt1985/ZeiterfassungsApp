using System;
using System.Collections.Generic;
using System.Text;

namespace Stechuhr.Models
{
    public interface IWorktimeSpan
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan WorkTimeSpan { get; }
    }
}
