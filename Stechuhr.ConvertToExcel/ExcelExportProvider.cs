using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Stechuhr;
using System.Globalization;
using Stechuhr.Settings;
using Stechuhr.Views;
using System.Drawing;

namespace Stechuhr.Utils
{
    public class ExcelExportProvider
    {
        public void ExportToExcel()
        {
            string path = "";

            try
            {
                string template = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stechuhr", "Zeiterfassung.Report.Template.xlsx");
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stechuhr", "Zeiterfassung.Report.xlsx");
                File.Copy(template, path, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }

            WorktimeProvider worktimeProvider = new WorktimeProvider();
            worktimeProvider.LoadWorktimeData();

            WorktimeSettings settings = new WorktimeSettings();
            DayViewProvider viewProvider = new DayViewProvider(worktimeProvider, settings);
            var Items = viewProvider.CreateOverallView();
            //Items.Reverse();

            var Excel = new Application();
            Workbook wb = Excel.Workbooks.Open(path);
            Worksheet ws = Excel.ActiveSheet as Worksheet;

            for (int i = 0; i < Items.Count; i++)
            {
                string c;
                var r = 1 + (Items.Count - i);
                var item = Items[i];

                c = "A";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.Date.DayOfWeek.ToString();

                c = "B";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.Date.ToString("d");

                c = "C";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.sComming;

                c = "D";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.sGoing;

                c = "E";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.sPauseTime;

                c = "F";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.Overtime.TotalMinutes > 0 ? item.sOvertime : "";

                c = "G";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.Overtime.TotalMinutes < 0 ? item.sOvertime.Substring(1) : "";

                c = "H";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.sWorkingTime;

                c = "I";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.Overtime.TotalMinutes != 0 ? 
                                                            viewProvider.GetOvertime(Items.Where(t => t.Date <= item.Date).ToList()).TotalHours:
                                                            "";

                c = "J";
                ws.Range[$"{c}{r}:{c}{r}"].FormulaR1C1 = item.sType;

                String range = $"A{r}:J{r}";
                if (!settings.RegularWorkingDays.Contains(item.Date.DayOfWeek))
                {
                    ExcelRangeFormatieren(ref ws, range, Color.LightGray);
                }
                else if (item.sType == "F")
                {
                    ExcelRangeFormatieren(ref ws, range, Color.LightYellow);
                }
                else if (item.sType == "K")
                {
                    ExcelRangeFormatieren(ref ws, range, Color.LightCoral);
                }
                else if (item.sType == "U")
                {
                    ExcelRangeFormatieren(ref ws, range, Color.LightGreen);
                }
                else if (item.sType == "KA" || item.sType == "KAH")
                {
                    ExcelRangeFormatieren(ref ws, range, Color.LightBlue);
                }
                else
                {
                    ExcelRangeFormatieren(ref ws, range, Color.White);
                }


            }

            wb.Save();
            Excel.Visible = true;
        }

        public void ExcelRangeFormatieren(ref Worksheet ws, string Range, Color BackColor)
        {
            ws.Range[Range].Interior.Color = ColorTranslator.ToOle(BackColor);
            ws.Range[Range].Borders.Color = ColorTranslator.ToOle(Color.Gray);
            ws.Range[Range].Interior.Pattern = XlPattern.xlPatternSolid;
        }
    }
}