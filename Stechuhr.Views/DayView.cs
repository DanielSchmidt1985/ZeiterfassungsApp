using Stechuhr.Models;
using Stechuhr.Settings;
using System;
using System.Dynamic;
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
            get => _wtItem == null ? "" : _wtItem.WorktimeType.ToString();
            set 
            {
                if (!Enum.TryParse<WorktimeType>(value, out WorktimeType type)) return;
                if (_wtItem == null)
                {
                    _wtItem = new WorktimeItem();
                    _wtItem.StartTime = Date;
                    _wtItem.EndTime = Date;
                    _wtItem.WorktimeType = type;
                    FromWorktimeItem(_wtItem);
                    WorktimeProvider.LoadWorktimeData();
                    WorktimeProvider.Worktimes.Add(_wtItem);
                    WorktimeProvider.SaveWorktimeData();
                    NotifyPropertyChanged();
                }
                else
                {
                    WorktimeProvider.LoadWorktimeData();
                    var i = WorktimeProvider.Worktimes.Find(t => t.id == _wtItem.id);
                    if (i != null)
                    {
                        _wtItem.WorktimeType = type;
                        i.WorktimeType = type;
                        WorktimeProvider.SaveWorktimeData();
                        FromWorktimeItem(_wtItem);
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

                    if (_wtItem == null)
                    {
                        _wtItem = new WorktimeItem();
                        _wtItem.StartTime = time;
                        _wtItem.EndTime = time;
                        FromWorktimeItem(_wtItem);
                        WorktimeProvider.LoadWorktimeData();
                        WorktimeProvider.Worktimes.Add(_wtItem);
                        WorktimeProvider.SaveWorktimeData();
                        NotifyPropertyChanged();
                    }
                    else 
                    {
                        WorktimeProvider.LoadWorktimeData();
                        var i = WorktimeProvider.Worktimes.Find(t => t.id == _wtItem.id);
                        if (i != null)
                        {
                            _wtItem.StartTime = time;
                            i.StartTime = time;
                            WorktimeProvider.SaveWorktimeData();
                            FromWorktimeItem(_wtItem);
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

                    if (_wtItem == null)
                    {
                        sComming = value;
                    }
                    else
                    {
                        WorktimeProvider.LoadWorktimeData();
                        var i = WorktimeProvider.Worktimes.Find(t => t.id == _wtItem.id);
                        if (i != null)
                        {
                            _wtItem.EndTime = time;
                            i.EndTime = time;
                            WorktimeProvider.SaveWorktimeData();
                            FromWorktimeItem(_wtItem);
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
                if (_wtItem == null)
                {
                    return new TimeSpan();
                }
                return _wtItem.WorkTimeSpan - _wtItem.OverallPauseSpan;
            }
        }
        public string sWorkingTime => WorkingTime.Format();

        public TimeSpan PauseTime
        {
            get
            {
                if (_wtItem == null)
                {
                    return new TimeSpan();
                }
                return _wtItem.OverallPauseSpan;
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
                    if (_wtItem != null && TimeSpan.TryParse(value, out TimeSpan ts))
                    {
                        WorktimeProvider.LoadWorktimeData();
                        var i = WorktimeProvider.Worktimes.Find(t => t.id == _wtItem.id);
                        if (i != null)
                        {
                            _wtItem.PauseTime = ts;
                            i.PauseTime = _wtItem.PauseTime;
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
                if (_wtItem == null)
                {
                    TimeSpan ret = new TimeSpan();
                    if (!WorktimeSettings.RegularWorkingDays.Any(t => Date.DayOfWeek == t)) return ret;
                    if (Date >= DateTime.Today) return ret;
                    return ret - WorktimeSettings.RegularWorkingTime;
                }
                TimeSpan BaseTime;
                long rwt = WorktimeSettings.RegularWorkingTime.Ticks;
                switch (_wtItem.WorktimeType)
                {
                    case WorktimeType.R:
                        BaseTime = new TimeSpan();
                        break;
                    case WorktimeType.U:
                        BaseTime = new TimeSpan(rwt);
                        break;
                    case WorktimeType.UH:
                        BaseTime = new TimeSpan(rwt / 2);
                        break;
                    case WorktimeType.K:
                        BaseTime = new TimeSpan(rwt);
                        break;
                    case WorktimeType.KA:
                    case WorktimeType.F:
                        BaseTime = new TimeSpan(rwt);
                        break;
                    case WorktimeType.KAH:
                        BaseTime = new TimeSpan(rwt / 2);
                        break;

                    default:
                        break;
                }
                return WorktimeSettings.RegularWorkingDays.Any(t => Date.DayOfWeek == t) ?
                            WorkingTime - WorktimeSettings.RegularWorkingTime + BaseTime :
                            WorkingTime;
            }
        }
        public string sOvertime => Overtime.Format();

        private WorktimeItem _wtItem = null;
        public WorktimeItem wtItem { get => _wtItem; set => FromWorktimeItem(value); }
        public bool isWtItem { get => _wtItem != null; }

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
            this._wtItem = wtItem;
            this.Date = wtItem.Date;
            this._sComming = wtItem.StartTime.TimeOfDay.Format();
            this._sGoing = wtItem.EndTime.TimeOfDay.Format();
        }

    }
}
