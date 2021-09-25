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
using static Riscks.View.MainWindowView;

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для GroupsView.xaml
    /// </summary>
    public partial class GroupsView : UserControl
    {
        public GroupsView()
        {
            InitializeComponent();
        }

        public event Navigation ButtonPressed;

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(Inset.Products);
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid.SelectedItem is null)
                return;
            grid.ScrollIntoView(grid.SelectedItem);
            grid.Focus();
            grid.BeginEdit();
        }
    }
}
