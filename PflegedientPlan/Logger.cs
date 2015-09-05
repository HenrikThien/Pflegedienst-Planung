using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan
{
    static class Logger
    {
        private static string ERROR_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VIS VITALIS\\error_log_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

        public async static Task InitAsync()
        {
            await Task.Run(() =>
            {
                if (!File.Exists(ERROR_FILE))
                {
                    File.Create(ERROR_FILE);
                }
            });
        }

        public async static Task WriteException(string exception)
        {
            await Task.Run(() =>
            {
                File.AppendAllText(ERROR_FILE, exception + Environment.NewLine);
            });
        }
    }
}
