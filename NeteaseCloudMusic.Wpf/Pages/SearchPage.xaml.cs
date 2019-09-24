using System.Windows.Controls;

namespace NeteaseCloudMusic.Wpf.Pages
{
    /// <summary>
    /// SearchPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private void StopSelectionChangedEventBubbling(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
