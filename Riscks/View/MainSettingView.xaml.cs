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
    /// Логика взаимодействия для MainSettingView.xaml
    /// </summary>
    public partial class MainSettingView : UserControl
    {
        private IndexCommonViewModel IndexCommonViewModel;

        public delegate void Close(int cnt, bool needClose);
        public event Close OnClose;

        public MainSettingView()
        {
            InitializeComponent();
        }
        private IndexUnitViewModel indexUnit;
        public void Init(IndexCommonViewModel IndexCommonViewModel)
        {
            tmpProject = null;
            NextWindow = -1;
            this.IndexCommonViewModel = IndexCommonViewModel;
            var analyssettings = new AnalysSettingsViewModel(IndexCommonViewModel.ProjectViewModel.DirectoryTree.SelectedProject.HierarhyElem);
            var commonanalyssettings = new HierarhySelectViewModel(IndexCommonViewModel.ProjectViewModel.DirectoryTree.SelectedProject.HierarhyElem);
            var launchervm = new LauncherViewModel();
            indexUnit = IndexCommonViewModel.IndexUnitViewModel;
            indexUnit.OnSaved += IndexUnit_OnSaved;
            unitSettings.DataContext = indexUnit;
            mainButtonsList.SelectedIndex = 0;
            projSetList.SelectedIndex = 0;
            prSetInfo.DataContext = IndexCommonViewModel.ProjectViewModel.DirectoryTree.SelectedProject.Property;
            prSetAnalys.DataContext = analyssettings;
            prSetComAn.DataContext = commonanalyssettings;
            tiOpenProject.DataContext = launchervm;
        }

        private void IndexUnit_OnSaved()
        {
            indexUnit.OnSaved -= IndexUnit_OnSaved;
            MainWindowView mainWindowView = new MainWindowView(IndexCommonViewModel.directoryTree);
            mainWindowView.Show();
            NextWindow = -1;
            //this.DialogResult = false;
            Closing(2,false);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = true;
            NextWindow = 0;
            if(!(OnClose is null))
            Closing(1);
        }

        public bool NeedOpenLauncher = false;
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = false;
            NeedOpenLauncher = true;
            LauncherWindow launcherWindow = new LauncherWindow();
            NextWindow = 0;
            launcherWindow.Show();
            Closing(2);
        }



        private int NextWindow = -1;

        private void btnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            tmpProject.Save();
            DirectoryTree directoryTree = new DirectoryTree();
            Node_Project node_Project = new Node_Project(tmpProject.Project_Properies.Name);
            directoryTree.SelectProject(node_Project);

            MainWindowView mainWindowView = new MainWindowView(directoryTree);
            mainWindowView.Show();
            tmpProject = null;
            NextWindow = 0;
            //this.DialogResult = false;
            Closing(2,false);
        }

        private void Closing(int cnt, bool needClose=true)
        {
            if (!(tmpProject is null))
            {
                tmpProject.Cansel();
            }
            if (NextWindow == -1)
            {
                if (!(OnClose is null))
                    OnClose(cnt,needClose);
            }
            else
            {
                if (!(OnClose is null))
                    OnClose(cnt, needClose);
                IndexCommonViewModel.CurrentElement.ReloadInterface();
            }
        }

        private CreateProjectViewModel tmpProject;
        private void btnCreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            if (tmpProject is null)
            {
                tmpProject = new CreateProjectViewModel();
            }
            tiCreateProject.DataContext = tmpProject;
            tab_control.SelectedItem = tiCreateProject;
        }

        private void btnProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            tab_control.SelectedItem = tiProjectSettings;
        }

        private void mainButtonsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tab_control.SelectedIndex = mainButtonsList.SelectedIndex >= 3 ? mainButtonsList.SelectedIndex - 1 : mainButtonsList.SelectedIndex;
            if (tab_control.SelectedItem == tiCreateProject)
            {
                if (tmpProject is null)
                {
                    tmpProject = new CreateProjectViewModel();
                }
                tiCreateProject.DataContext = tmpProject;
                tab_control.SelectedItem = tiCreateProject;
            }
        }

        private void projSetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            projSettTC.SelectedIndex = projSetList.SelectedIndex;
        }

        private void projsListToOpen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DirectoryTree directoryTree = new DirectoryTree();
            Node_Project node_Project = new Node_Project(((ProjectInfo)projsListToOpen.SelectedItem).Name);
            directoryTree.SelectProject(node_Project);
            MainWindowView mainWindowView = new MainWindowView(directoryTree);
            mainWindowView.Show();
            NextWindow = 0;
            //this.DialogResult = false;
            Closing(2,false);
        }

        private void studySetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
