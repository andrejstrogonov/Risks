using Riscks.Resources;
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
    /// Логика взаимодействия для ResourceView.xaml
    /// </summary>
    public partial class ResourceView : UserControl
    {
        public ResourceView()
        {
            InitializeComponent();
           
        }

        private void BarSeries_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void dataGridResources_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGridResources.SelectedItem is null)
                return;

            dataGridResources.ScrollIntoView(dataGridResources.SelectedItem);
            dataGridResources.BeginEdit();
        }
        /*
private void BarSeries_SizeChanged(object sender, SizeChangedEventArgs e)
{
double maxLen = (Math.Log10(((IndexResourcesViewModel)DataContext).CurrentResource.MaxNumberInUsing+1)+1) * WindowConstants.LabelFontSize;
if (!WidthWasUpdated)
{
BarSeries.Width = e.NewSize.Width + maxLen;
WidthWasUpdated = true;
}
else
{
WidthWasUpdated = false;
}
}*/
    }
}
