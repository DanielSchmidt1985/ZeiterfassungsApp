using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Stechuhr
{
    public class WorktimeProvider
    {
        public string FileName { get; set; } = "WorktimeData.json";
        public string FilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), FileName);
            }
        }

        public WorktimeItemCollection LoadWorktimeData()
        {
            try
            {
                return JsonSerializer.Deserialize<WorktimeItemCollection>(File.ReadAllText(FilePath, Encoding.Default));
            }
            catch (Exception)
            {
                return new WorktimeItemCollection();
            }
        }

        public void SaveWorktimeData(WorktimeItemCollection wtC)
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.WriteIndented = true;
            File.WriteAllText(FilePath, JsonSerializer.Serialize<WorktimeItemCollection>(wtC, jsonSerializerOptions));
        }

    }
}
