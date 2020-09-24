using System;
using System.Collections.Generic;
using System.Text;

namespace Stechuhr
{
    public class WorktimeItem
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public List<PauseItem> Pause { get; set; } = new List<PauseItem>();

    }

}
