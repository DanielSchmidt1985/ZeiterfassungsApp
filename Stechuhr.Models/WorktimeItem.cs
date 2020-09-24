using Stechuhr.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stechuhr
{
    public class WorktimeItem : WorktimeItemBase
    {
        public List<PauseItem> Pause { get; set; } = new List<PauseItem>();

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
