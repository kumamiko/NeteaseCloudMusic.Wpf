using NeteaseCloudMusic.Wpf.ViewModel;
using System.Windows.Controls;

namespace NeteaseCloudMusic.Wpf.Pages
{
    /// <summary>
    /// AlbumPage.xaml 的交互逻辑
    /// </summary>
    public partial class AlbumPage : Page
    {
        public AlbumPage()
        {
            InitializeComponent();
        }

        public AlbumPage(AlbumViewModel albumVM)
        {
            InitializeComponent();

            this.DataContext = albumVM;
        }
    }
}
