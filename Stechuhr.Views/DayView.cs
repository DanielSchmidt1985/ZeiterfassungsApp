using Stechuhr.Models;
using Stechuhr.Settings;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stechuhr.Views
{
    public class DayView : DayViewBase
    {
        public WorktimeProvider WorktimeProvider { get; }
        public WorktimeSettings WorktimeSettings { get; }
        public DateTime Date { get; set; } = DateTime.Today;
        public string sDate { get => Date.ToString("dd. MMM - ddd"); }

        public string sType
        {
            get => wtItem == null ? "" : wtItem.WorktimeType.ToString();
            set 
            {
                if (!Enum.TryParse<WorktimeType>(value, out WorktimeType type)) return;
                if (wtItem == null)
                {
                    wtItem = new WorktimeItem();
                    wtItem.StartTime = Date;
                    wtItem.EndTime = Date;
                    wtItem.WorktimeType = type;
                    FromWorktimeItem(wtItem);
                    WorktimeProvider.LoadWorktimeData();
                    WorktimeProvider.Worktimes.Add(wtItem);
                    WorktimeProvider.SaveWorktimeData();
                    NotifyPropertyChanged();
                }
                else
                {
                    WorktimeProvider.LoadWorktimeData();
                    var i = WorktimeProvider.Worktimes.Find(t => t.id == wtItem.id);
                    if (i != null)
                    {
                        wtItem.WorktimeType = type;
                        i.WorktimeType = type;
                        WorktimeProvider.SaveWorktimeData();
                        FromWorktimeItem(wtItem);
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        private string _sComming;
        public string sComming
        {
            get => _sComming;
            set
            {
                try
                {
                    if (!DateTime.TryParse(value, out DateTime time)) return;
                    time = this.Date.Add(time.TimeOfDay);

                    if (wtItem == null)
                    {
                        wtItem = new WorktimeItem();
                        wtItem.StartTime = time;
                        wtItem.EndTime = time;
                        FromWorktimeItem(wtItem);
                        WorktimeProvider.LoadWorktimeData();
                        WorktimeProvider.Worktimes.Add(wtItem);
                        WorktimeProvider.SaveWorktimeData();
                        NotifyPropertyChanged();
                    }
                    else 
                    {
                        WorktimeProvider.LoadWorktimeData();
                        var i = WorktimeProvider.Worktimes.Find(t => t.id == wtItem.id);
                        if (i != null)
                        {
                            wtItem.StartTime = time;
                            i.StartTime = time;
                            WorktimeProvider.SaveWorktimeData();
                            FromWorktimeItem(wtItem);
                            NotifyPropertyChanged();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private string _sGoing;
        public string sGoing
        {
            get => _sGoing;
            set
            {
                try
                {
                    if (!DateTime.TryParse(value, out DateTime time)) return;
                    time = this.Date.Add(time.TimeOfDay);

                    if (wtItem == null)
                    {
                        sComming = value;
                    }
                    else
                    {
                        WorktimeProvider.LoadWorktimeData();
                        var i = WorktimeProvider.Worktimes.Find(t => t.id == wtItem.id);
                        if (i != null)
                        {
                            wtItem.EndTime = time;
                            i.EndTime = time;
                            WorktimeProvider.SaveWorktimeData();
                            FromWorktimeItem(wtItem);
                            NotifyPropertyChanged();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public TimeSpan WorkingTime
        {
            get
            {
                if (wtItem == null)
                {
                    return new TimeSpan();
                }
                return wtItem.TimeSpan - wtItem.OverallPauseSpan;
            }
        }
        public string sWorkingTime => WorkingTime.Format();

        public TimeSpan PauseTime
        {
            get
            {
                if (wtItem == null)
                {
                    return new TimeSpan();
                }
                return wtItem.OverallPauseSpan;
            }
        }
        public string sPauseTime
        {
            get
            {
                return PauseTime.Format();
            }
            set
            {
                try
                {
                    if (wtItem != null && TimeSpan.TryParse(value, out TimeSpan ts))
                    {
                        WorktimeProvider.LoadWorktimeData();
                        var i = WorktimeProvider.Worktimes.Find(t => t.id == wtItem.id);
                        if (i != null)
                        {
                            wtItem.PauseTime = ts;
                            i.PauseTime = wtItem.PauseTime;
                            WorktimeProvider.SaveWorktimeData();
                            NotifyPropertyChanged();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public TimeSpan Overtime
        {
            get
            {
                if (wtItem == null)
                {
                    return WorktimeSettings.RegularWorkingDays.Any(t => Date.DayOfWeek == t) ?
                               new TimeSpan() - WorktimeSettings.RegularWorkingTime :
                               new TimeSpan();
                }
                return WorktimeSettings.RegularWorkingDays.Any(t => Date.DayOfWeek == t) ?
                            WorkingTime - WorktimeSettings.RegularWorkingTime :
                            WorkingTime;
            }
        }
        public string sOvertime => Overtime.Format();

        private WorktimeItem wtItem = null;

        public DayView(WorktimeProvider WorktimeProvider, WorktimeSettings settings, DateTime Date)
        {
            this.WorktimeProvider = WorktimeProvider;
            this.WorktimeSettings = settings;
            this.Date = Date;
        }
        public DayView(WorktimeProvider WorktimeProvider, WorktimeSettings settings, WorktimeItem wtItem) : this(WorktimeProvider, settings, wtItem.Date)
        {
            FromWorktimeItem(wtItem);
        }

        public void FromWorktimeItem(WorktimeItem wtItem)
        {
            this.wtItem = wtItem;
            this.Date = wtItem.Date;
            this._sComming = wtItem.StartTime.TimeOfDay.Format();
            this._sGoing = wtItem.EndTime.TimeOfDay.Format();
        }

    }
}
