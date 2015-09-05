using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PflegedientPlan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Init();
        }

        private async void Init()
        {
            await Logger.InitAsync();

            //await Task.Run(() =>
            //{
            //    try
            //    {
            //        string sourcePath = System.IO.Directory.GetCurrentDirectory();
            //        appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //        string fileName = "Storage.mdf";

            //        System.Diagnostics.Debug.WriteLine(appDataPath + "\\" + fileName);

            //        System.IO.File.Copy(sourcePath + "\\" + fileName, appDataPath + "\\" + fileName, false);
            //    }
            //    catch (Exception ex)
            //    {
            //        WriteException(ex);
            //    }
            //});
        }

        private async void WriteException(Exception ex)
        {
            await Logger.WriteException(ex.ToString());
        }
    }
}
