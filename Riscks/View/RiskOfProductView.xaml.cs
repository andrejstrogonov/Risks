using Riscks.ViewModel;
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

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для RiskOfProductView.xaml
    /// </summary>
    public partial class RiskOfProductView : UserControl
    {
        public RiskOfProductView()
        {
            InitializeComponent();
            
        }

        private void dGRiskTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dGRiskTable.SelectedItem is null)
                return;

            dGRiskTable.ScrollIntoView(dGRiskTable.SelectedItem);
            dGRiskTable.BeginEdit();
        }
    }
}
