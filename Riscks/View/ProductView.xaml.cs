using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Riscks.View.MainWindowView;

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        public event Navigation ButtonPressed;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonPressed(Inset.Groups);
        }

        private void dGProducts_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dGProducts.SelectedItem is null)
                return;
        
            dGProducts.ScrollIntoView(dGProducts.SelectedItem);
            dGProducts.BeginEdit();
           
        }

        private void dGProducts_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void DataGridTemplateColumn_GotFocus(object sender, RoutedEventArgs e)
        {
            var q = 3;
        }
    }
}
