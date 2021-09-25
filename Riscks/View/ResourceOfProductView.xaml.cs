using Riscks.ViewModel;
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

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для ResourceOfProductView.xaml
    /// </summary>
    public partial class ResourceOfProductView : UserControl
    {
        public ResourceOfProductView()
        {
            InitializeComponent();
          
        }

        private void dGResourceList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dGResourceList.SelectedItem is null)
                return;

            dGResourceList.ScrollIntoView(dGResourceList.SelectedItem);
            dGResourceList.BeginEdit();
        }
    }
}
