using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenshenApp.UserControls
{
    /// <summary>
    /// WorldLeftPanel.xaml 的交互逻辑
    /// </summary>
    public partial class WorldRightPanel : UserControl
    {
        public event Action<int> selectChanged;

        public WorldRightPanel()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = sender as ListBox;
            selectChanged?.Invoke(s.SelectedIndex);
        }
    }
}
