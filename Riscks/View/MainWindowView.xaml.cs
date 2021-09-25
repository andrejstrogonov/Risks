using ClientServerLibrary;
using Riscks.Resources;
using Riscks.ViewModel;
using SQLiteLibrary;
using SQLiteLibrary.Model;
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
    /// Логика взаимодействия для MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public IndexCommonViewModel IndexCommonViewModel { get; set; }
        //public CommonViewModel CommonViewModel { get; set; }

        public bool NeedStop { get; set; }
        public MainWindowView(DirectoryTree directoryTree, bool needActivate=true)
        {












            NeedStop = true;
            this.Top = 0;
            this.Left = 0;
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            IndexCommonViewModel = new IndexCommonViewModel(directoryTree);
            this.DataContext = IndexCommonViewModel;
            IndexCommonViewModel.ProjectViewModel.PropertyChanged += ProjectViewModel_PropertyChanged;
           
            InitializeComponent();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(
    typeof(FrameworkElement),
    new FrameworkPropertyMetadata(
    System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)
    )
    );
            }
            catch(Exception e)
            {

            }
            //ringDiagramAVC.Init(CommonViewModel.IndexProductViewModel);
            Products.ButtonPressed += MovePages;
            Groups.ButtonPressed += MovePages;
            FlexibilityAnalysViewModel.MainWindowView = this;
        }

        private void ProjectViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IndexCommonViewModel.ProjectViewModel.CurrentNode))
            {
                var res = this.Template.FindName("Menu", this) as MainMenuView;
                if (IndexCommonViewModel.ProjectViewModel.SelectedLevel <2 )
                {
                    ((MenuViewModel)res.DataContext).SelectProject(LastProjectTab);
                }
                else
                {
                    ((MenuViewModel)res.DataContext).SelectSession(LastSessionTab);
                }
            }
        }

        private int LastSessionTab = 0;
        private int LastProjectTab = 3;

        private void React(int cnt, bool needClose)
        {
            if (cnt==2)
            {
                NeedStop = needClose;
                this.Close();
            }
            else
            {
                maincontrol.SelectedIndex = 0;
                var res = this.Template.FindName("Menu", this) as MainMenuView;
                res.Visibility = Visibility.Visible;
            }
        }

        public void ChangeTab(int num)
        {
            if (num == 6)
            {
                var res = this.Template.FindName("Menu", this) as MainMenuView;
                res.Visibility = Visibility.Collapsed;
                settings.OnClose += React;
                settings.Init(IndexCommonViewModel);
                IndexCommonViewModel.IndexUnitViewModel.ReloadDeleting();
                maincontrol.SelectedIndex = 1;
                /*if (mainSettingsWindow.ShowDialog() is true)
                {
                    CommonViewModel.ProjectViewModel.DirectoryTree.SelectedProject.Select();
                        
                    this.Show();
                }
                else
                {                    
                    if (mainSettingsWindow.NeedOpenLauncher)
                    {
                        LauncherWindow launcherWindow = new LauncherWindow();
                        launcherWindow.Show();
                    }
                    this.Close();
                }*/
                return;
            }
            if(IndexCommonViewModel.ProjectViewModel.CurrentNode is Node_Session)
            {
                LastSessionTab = num;
                
            }
            else
            {
                LastProjectTab = num;
            }

            if (num == 1)
            {
              
            }
            else
            if (num == 0)
            {
                TabControl2.SelectedIndex = 0;
            }
            this.TabControl1.SelectedIndex = num;
            this.TabControl1.SelectedItem = this.TabControl1.Items[num];
        }

        public enum Inset
        {
            Products,
            Groups
        }

        public delegate void Navigation(Inset inset);

        public void MovePages(Inset inset)
        {
            if(inset== Inset.Groups)
            {
                IndexCommonViewModel.CurrentElement.IndexGroupViewModel.ReloadProducts();
            }
            TabControl2.SelectedIndex=inset==Inset.Products?0:1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var res = this.Template.FindName("Menu",this) as MainMenuView;
            ((MenuViewModel)res.DataContext).OnChange += ChangeTab;
            if (IndexCommonViewModel.ProjectViewModel.SelectedLevel < 2)
            {
                ((MenuViewModel)res.DataContext).SelectProject(LastProjectTab);
            }
            else
            {
                ((MenuViewModel)res.DataContext).SelectSession(LastSessionTab);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IndexCommonViewModel.SaveChanges();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            License.StopUsing();
        }
    }
}
