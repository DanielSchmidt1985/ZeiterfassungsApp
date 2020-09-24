using Stechuhr.Models;
using System;

namespace Stechuhr
{
    public class PauseItem : WorktimeItemBase
    {
        public override Object Clone(Object o)
        {
            o = base.Clone(o);
            return o;
        }

        public override object Clone()
        {
            return Clone(new PauseItem());
        }
    }
}