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
        public WorktimeStatus Status { get; private set; } = WorktimeStatus.NotWorking;

        public string FilePath { get; private set; }

        public TimeSpan TodayWorktimeSpan
        {
            get
            {
                WorktimeItem wt = CurrentWorktime;
                DateTime to;

                if (wt != null)
                {
                    JoinOnDay(wt);
                    to = DateTime.Now;
                }
                else
                {
                    wt = Worktimes.Skip(Worktimes.Count - 2)
                                  .ToList()
                                  .FindLast(t => t.StartTime.Date == DateTime.Now.Date);
                    if (wt == null) return new TimeSpan(0);
                    to = wt.EndTime;
                }

                TimeSpan wts = wt.StartTime - to;
                wt.Pause.ForEach(t => wts -= t.StartTime - t.EndTime);

                return wts;
            }
        }
        public TimeSpan TodayPauseSpan
        {
            get
            {
                WorktimeItem wt = CurrentWorktime;

                if (wt != null)
                {
                    JoinOnDay(wt);
                }
                else
                {
                    wt = Worktimes.Skip(Worktimes.Count - 2)
                                  .ToList()
                                  .FindLast(t => t.StartTime.Date == DateTime.Now.Date);
                    if (wt == null) return new TimeSpan(0);
                }

                TimeSpan wts = new TimeSpan();
                wt.Pause.ForEach(t => wts += t.EndTime - t.StartTime);

                return wts;
            }
        }
        public DateTime TodayWorktimeStart
        {
            get
            {
                if (CurrentWorktime != null)
                {
                    JoinOnDay(CurrentWorktime);
                    return CurrentWorktime.StartTime;
                }
                var wt = Worktimes.Skip(Worktimes.Count - 2).ToList().FindLast(t => t.EndTime.Date == DateTime.Now.Date);
                if (wt == null) return DateTime.MinValue;
                return wt.StartTime;
            }
        }
        public DateTime TodayWorktimeEnd
        {
            get
            {
                if (CurrentWorktime != null)
                {
                    return DateTime.Now;
                }
                var wt = Worktimes.Skip(Worktimes.Count - 2).ToList().FindLast(t => t.EndTime.Date == DateTime.Now.Date);
                if (wt == null) return DateTime.MinValue;
                return wt.EndTime;
            }
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
            try
            {
                File.Copy(FilePath, Path.Combine(Path.GetDirectoryName(FilePath), "Backup-" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".bak"), true);
            }
            catch (Exception)
            { }
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
            var toJoin = Worktimes.Skip(Worktimes.Count - 2)
                                  .ToList()
                                  .FindLast(t => t.StartTime.Date == lastWt.StartTime.Date && t.Equals(lastWt) == false);
            if (toJoin == null) return;
            PauseItem pause = new PauseItem();
            pause.StartTime = toJoin.EndTime;
            pause.EndTime = lastWt.StartTime;
            lastWt.StartTime = toJoin.StartTime;
            lastWt.Pause.AddRange(toJoin.Pause);
            lastWt.Pause.Add(pause);
            Worktimes.Remove(toJoin);
        }
        public WorktimeItemBase SplitOnDate(WorktimeItemBase toSplit)
        {
            WorktimeItemBase nWt = (WorktimeItemBase)toSplit.Clone();
            nWt.StartTime = nWt.EndTime.Date;
            toSplit.EndTime = toSplit.StartTime.Date + new TimeSpan(23, 59, 59);
            return nWt;
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

    }
}
