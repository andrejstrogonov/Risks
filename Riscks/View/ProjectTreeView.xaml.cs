using ProjectManagerLibrary.WorkWithProjects;
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
    /// Логика взаимодействия для ProjectTreeView.xaml
    /// </summary>
    public partial class ProjectTreeView : UserControl
    {
        public ProjectTreeView()
        {
            InitializeComponent();
        }

        private void DirectoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DirectoryTreeView.SelectedItem is Node_Project)
            {
                tabControl.SelectedIndex = 0;
            }
            else
            if(DirectoryTreeView.SelectedItem is Node_Section)
            {
                tabControl.SelectedIndex = 1;
            }
            else
            {
                tabControl.SelectedIndex = 2;
            }
        }

        private void DirectoryTreeView_Selected(object sender, RoutedEventArgs e)
        {
            ((ProjectViewModel)DataContext).SelectNode(sender, e);
        }
    }
}
