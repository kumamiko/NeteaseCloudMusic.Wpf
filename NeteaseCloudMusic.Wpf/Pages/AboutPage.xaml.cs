using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NeteaseCloudMusic.Wpf.Pages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                txtVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            };
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            try
            {
                Process.Start(new ProcessStartInfo(hyperlink.Tag.ToString()));
            }
            catch { }
        }
    }
}
