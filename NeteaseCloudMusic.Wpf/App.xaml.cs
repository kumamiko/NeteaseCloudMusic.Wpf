using NeteaseCloudMusic.Wpf.Config;
using System;
using System.Windows;
using System.Windows.Threading;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();
            DispatcherUnhandledException += UnhandledExceptionHandler;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            GlobalData.Init();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            GlobalData.Save();

            base.OnExit(e);
        }

        private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine();
        }
    }
}
