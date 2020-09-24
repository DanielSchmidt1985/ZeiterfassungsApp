using Newtonsoft.Json;
using Stechuhr.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Stechuhr
{
    public class WorktimeProvider
    {
        public WorktimeItemCollection Worktimes { get; private set; }

        public WorktimeItem CurrentWorktime { get; private set; } = null;
        public PauseItem CurrentPause { get; private set; } = null;
        public WorktimeStatus Status { get; private set; } = WorktimeStatus.NotWorking;

        public string FilePath { get; private set; }

        public TimeSpan CurrentWorktimeSpan()
        {
            if (CurrentWorktime == null) return new TimeSpan(0);

            DateTime to = CurrentPause == null ? DateTime.Now : CurrentPause.StartTime;
            TimeSpan wts = CurrentWorktime.StartTime - to;
            CurrentWorktime.Pause.ForEach(t => wts -= t.StartTime - t.EndTime);

            return wts;
        }

        public bool LoadWorktimeData(string FilePath)
        {
            try
            {
                this.FilePath = FilePath;
                Worktimes = JsonConvert.DeserializeObject<WorktimeItemCollection>(File.ReadAllText(FilePath, Encoding.Default));
                if (Worktimes == null) Worktimes = new WorktimeItemCollection();
                return true;
            }
            catch (Exception)
            {
                Worktimes = new WorktimeItemCollection();
                return false;
            }
        }
        public void SaveWorktimeData()
        {
            JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings();
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Worktimes, jsonSerializerOptions));
        }

        public WorktimeStatus Stamping()
        {
            Status = WorktimeStatus.InvalidCommand;
            if (CurrentWorktime == null)
            {
                CurrentWorktime = new WorktimeItem();
                Status = WorktimeStatus.Working;
            }
            else if (CurrentPause != null)
            {
                CurrentPause.EndTime = DateTime.Now;
                CurrentWorktime.EndTime = CurrentPause.StartTime;
                CurrentPause = null;
                Worktimes.Add(CurrentWorktime);
                CurrentWorktime = null;
                Status = WorktimeStatus.NotWorking;
            }
            else if (CurrentWorktime != null)
            {
                CurrentWorktime.EndTime = DateTime.Now;
                Worktimes.Add(CurrentWorktime);
                CurrentWorktime = null;
                Status = WorktimeStatus.NotWorking;

            }
            
            if (Status == WorktimeStatus.InvalidCommand)
            {
                throw new InvalidOperationException();
            }

            if (Status == WorktimeStatus.NotWorking)
            {
                WorktimeItem lastWt = Worktimes.Last();
                CheckDateSwitches();
                JoinOnDay(lastWt);
            }

            return Status;
        }

        /// <summary>
        /// If there more then one WorktimeItem on a day then we join them together and add a pause between
        /// </summary>
        /// <param name="lastWt"></param>
        public void JoinOnDay(WorktimeItem lastWt)
        {
            var toJoin = Worktimes.FindLast(t => t.StartTime.Date == lastWt.StartTime.Date && t.Equals(lastWt) == false);
            if (toJoin == null) return;
            PauseItem pause = new PauseItem();
            pause.StartTime = toJoin.EndTime;
            pause.EndTime = lastWt.StartTime;
            lastWt.StartTime = toJoin.StartTime;
            lastWt.Pause.AddRange(toJoin.Pause);
            lastWt.Pause.Add(pause);
            Worktimes.Remove(toJoin);
        }

        private bool CheckDateSwitches()
        {
            WorktimeItem lastWt = Worktimes.Last();

            // Splitten wenn Datumsübergreifend
            if (lastWt.StartTime.Date != lastWt.EndTime.Date)
            {
                Worktimes.Add((WorktimeItem)SplitOnDate(lastWt));
                return true;
            }

            // Noch Pausen splitten und den richtigen Tagen zuordnen
            // ...
            // 

            return false;
        }

        public WorktimeItemBase SplitOnDate(WorktimeItemBase toSplit)
        {
            WorktimeItemBase nWt = (WorktimeItemBase)toSplit.Clone();
            nWt.StartTime = nWt.EndTime.Date;
            toSplit.EndTime = toSplit.StartTime.Date + new TimeSpan(23, 59, 59);
            return nWt;
        }

        
    }
}
