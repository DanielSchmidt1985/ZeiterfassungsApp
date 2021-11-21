using System;

namespace Stechuhr.Utils
{
    class Program
    {
        public static void Main(string [] param)
        {
            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Starte Excel Export ...");

            ExcelExportProvider ep = new ExcelExportProvider();
            ep.ExportToExcel();
        }
    }
}