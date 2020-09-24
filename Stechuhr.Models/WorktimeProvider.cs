using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Stechuhr
{
    public class WorktimeProvider
    {
        public WorktimeItemCollection Worktimes { get; private set; }
       
        private WorktimeItem CurrentWorktime { get; set; } = null;
        private PauseItem CurrentPause { get; set; } = null;

        public string FilePath { get; private set; }

        public bool LoadWorktimeData(string FilePath)
        {
            try
            {
                this.FilePath = FilePath;
                Worktimes = JsonSerializer.Deserialize<WorktimeItemCollection>(File.ReadAllText(FilePath, Encoding.Default));
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
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.WriteIndented = true;
            File.WriteAllText(FilePath, JsonSerializer.Serialize<WorktimeItemCollection>(Worktimes, jsonSerializerOptions));
        }

        public WorktimeCommandResult StartWorking()
        {
            if (CurrentWorktime == null)
            {
                CurrentWorktime = new WorktimeItem();
                return WorktimeCommandResult.StartWorking;
            }
            if (CurrentPause != null)
            {
                CurrentPause.EndTime = DateTime.UtcNow;
                CurrentWorktime.Pause.Add(CurrentPause);
                CurrentPause = null;
                return WorktimeCommandResult.EndPause;
            }
            return WorktimeCommandResult.InvalidCommand;
        }
        public WorktimeCommandResult EndWorking()
        {
            if (CurrentPause != null)
            {
                CurrentWorktime.EndTime = CurrentPause.StartTime;
                CurrentPause = null;
                Worktimes.Add(CurrentWorktime);
                CurrentWorktime = null;
                return WorktimeCommandResult.EndWorking;
            }
            if (CurrentWorktime != null)
            {
                CurrentWorktime.EndTime = DateTime.UtcNow;
                Worktimes.Add(CurrentWorktime);
                CurrentWorktime = null;
                return WorktimeCommandResult.EndWorking;
            }
            return WorktimeCommandResult.InvalidCommand;
        }
        public WorktimeCommandResult StartPause()
        {
            if (CurrentWorktime == null)
            {
                return WorktimeCommandResult.InvalidCommand;
            }
            if (CurrentPause == null)
            {
                CurrentPause = new PauseItem();
                return WorktimeCommandResult.StartPause;
            }
            return WorktimeCommandResult.InvalidCommand;
        }
        public WorktimeCommandResult EndPause()
        {
            if (CurrentWorktime == null)
            {
                return WorktimeCommandResult.InvalidCommand;
            }
            if (CurrentPause != null)
            {
                CurrentPause.EndTime = DateTime.UtcNow;
                CurrentWorktime.Pause.Add(CurrentPause);
                CurrentPause = null;
                return WorktimeCommandResult.EndPause;
            }
            return WorktimeCommandResult.InvalidCommand;
        }

    }
}
