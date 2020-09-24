using System;

namespace Stechuhr
{
    public class PauseItem
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
    }
}