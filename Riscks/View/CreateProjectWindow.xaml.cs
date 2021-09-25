using ClientServerLibrary;
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
using System.Windows.Shapes;

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для CreateProjectWindow.xaml
    /// </summary>
    /// 

    

    public partial class CreateProjectWindow : Window
    {

        private CreateProjectViewModel tmpProject;
        public CreateProjectWindow()
        {
            if (License.LicenseKey == "" || License.LicenseKey is null || !License.ActivateLicense(License.LicenseKey))
            {
                throw new ApplicationException("Лицензия отсутствует");
            }
            try
            {
                tmpProject = new CreateProjectViewModel();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            this.DataContext = tmpProject;
            InitializeComponent();
            
        }

  

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            ShowInTaskbar = true;
        }

        private void first_type_Click(object sender, RoutedEventArgs e)
        {
            ((CreateProjectViewModel)DataContext).SelectType(0);
        }

        private void second_type_Click(object sender, RoutedEventArgs e)
        {
            ((CreateProjectViewModel)DataContext).SelectType(1);
        }

        private void third_type_Click(object sender, RoutedEventArgs e)
        {
            ((CreateProjectViewModel)DataContext).SelectType(2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((CreateProjectViewModel)DataContext).Cansel();
            tmpProject = null;
            LauncherWindow launcherWindow = new LauncherWindow();
            launcherWindow.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ((CreateProjectViewModel)DataContext).Save();
            DirectoryTree directoryTree = new DirectoryTree();
            Node_Project node_Project = new Node_Project(((CreateProjectViewModel)DataContext).Project_Properies.Name);
            directoryTree.SelectProject(node_Project);
            MainWindowView mainWindowView = new MainWindowView(directoryTree);
            tmpProject = null;
            mainWindowView.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!(tmpProject is null))
            {
                ((CreateProjectViewModel)DataContext).Cansel();
            }
        }
    }
}
