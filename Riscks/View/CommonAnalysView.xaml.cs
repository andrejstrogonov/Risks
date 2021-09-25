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
    /// Логика взаимодействия для CommonAnalysView.xaml
    /// </summary>
    public partial class CommonAnalysView : UserControl
    {
        public CommonAnalysView()
        {
            InitializeComponent();
        }

        private void X_Axis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AnalysViewModel.WidthOfPlot == 0)
            {
                AnalysViewModel.WidthOfPlot = e.NewSize.Width;
                ((AnalysViewModel)DataContext).ReloadWidth();
            }
            /*if (X_Axis.Minimum is null || X_Axis.Maximum is null)
                return;
            double width = e.NewSize.Width;
            double min = (double)X_Axis.Minimum;
            double max = (double)X_Axis.Maximum;
            if (max - min <= 0)
                return;
            double lenOfIntegrval = width / (max - min);
            double y_min = (double)Y_Axis.Minimum;
            double y_max = (double)Y_Axis.Maximum;
            double y_width = Y_Axis.ActualWidth;
            double y_len = y_max - y_min;
            if (y_len <= 0)
                return;
            double interval = lenOfIntegrval * y_len / y_width;
            Y_Axis.Interval = interval;*/
        }

        private void Y_Axis_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnalysViewModel.HeightOfPlot = e.NewSize.Height;
        }
        
    }
}
