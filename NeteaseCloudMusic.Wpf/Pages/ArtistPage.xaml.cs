using NeteaseCloudMusic.Wpf.ViewModel;
using System.Windows.Controls;

namespace NeteaseCloudMusic.Wpf.Pages
{
    /// <summary>
    /// SingerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ArtistPage : Page
    {
        public ArtistPage()
        {
            InitializeComponent();
        }

        public ArtistPage(ArtistViewModel artistVM)
        {
            InitializeComponent();

            this.DataContext = artistVM;
        }

        /// <summary>
        /// 停止 子级 向 TabControl 传递 SelectionChanged 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopSelectionChangedEventBubbling(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
