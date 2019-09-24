using NeteaseCloudMusic.Wpf.ViewModel;
using System.Windows.Controls;

namespace NeteaseCloudMusic.Wpf.Pages
{
    /// <summary>
    /// MvPage.xaml 的交互逻辑
    /// </summary>
    public partial class MvPage : Page
    {
        public MvPage()
        {
            InitializeComponent();
        }

        public MvPage(MvViewModel mvVM)
        {
            InitializeComponent();

            this.DataContext = mvVM;
        }
    }
}
