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
    /// Логика взаимодействия для GistogrammDouble.xaml
    /// </summary>
    public partial class GistogrammDouble : UserControl
    {        
        public class Elem
        {
            public static int cnt=0;
            public string Name { get; set; }
            public Prop Value { get; set; } 
            public Elem()
            {
                Name = (cnt*10).ToString()+" р.";
                Value = new Prop() { First = cnt, Second = cnt + 1 };
                cnt++;
            }
        }

        public GistogrammDouble()
        {
            //items = new List<RiskOfProductForDiagram>();            
            InitializeComponent();
            //doubleColumnSeries.ItemsSource = elems;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

       // public List<RiskOfProductForDiagram> items { get; set; }
        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /*if (DataContext is IndexRiskOfProductViewModel)
            {
                var newitems = ((IndexRiskOfProductViewModel)DataContext).DiagramProps;
                items = newitems;
                doubleColumnSeries.Refresh();
            }*/
            
        }
    }
}
