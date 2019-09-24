using NeteaseCloudMusic.Wpf.Pages;
using System;

namespace NeteaseCloudMusic.Wpf.Config
{
    internal class AppConfig
    {
        public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}AppConfig.json";

        public System.Windows.Rect MainRestoreBounds { get; set; } = new System.Windows.Rect { X = 100, Y = 100, Width = 800, Height = 600 };
        public System.Windows.WindowState MainWindowState { get; set; } = System.Windows.WindowState.Normal;

        public Type LastPage { get; set; } = typeof(LocalMusicPage);
        public int LastSelectedIndex { get; set; } = 1;

        public string SaveFolder { get; set; } = string.Empty;
    }
}
