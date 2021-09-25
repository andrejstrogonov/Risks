using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using SQLiteLibrary.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using Riscks.Commands;
using System.Windows;
using ProjectManagerLibrary.WorkWithProjects;
using Riscks.View;
using SQLiteLibrary;
using System.Windows.Controls;

namespace Riscks.ViewModel
{
    public class ProjectViewModel:ViewModel
    {
        public DirectoryTree DirectoryTree { get; set; } 

        private DirectoryNode currentNode;
        public DirectoryNode CurrentNode {
            get
            {
                return currentNode;
            }
            set
            {
                if (!(currentNode is null))
                {
                    currentNode.Unselect(true);
                    //OnPropertyChanged(nameof(CurrentNode));
                }
                currentNode = value;
                currentNode.Select();
                if (value is Node_Session)
                {
                    if(!(OnChangeLevel is null))
                    OnChangeLevel(2);
                    SelectedLevel = 2;
                    OnPropertyChanged(nameof(SelectedLevel));
                }
                else
                if (value is Node_Project)
                {
                    if (!(OnChangeLevel is null))
                        OnChangeLevel(0);
                    SelectedLevel = 0;
                    OnPropertyChanged(nameof(SelectedLevel));
                }
                else
                {
                    if (!(OnChangeLevel is null))
                        OnChangeLevel(1);
                    SelectedLevel = 1;
                    OnPropertyChanged(nameof(SelectedLevel));
                }
                OnPropertyChanged(nameof(CurrentNode));
                //OnPropertyChanged("NeedReload");
            }
        }
        public int SelectedLevel { get; set; }

        public delegate void LevelChanged(int level);
        public event LevelChanged OnChangeLevel;

        public void SelectNode(object sender, RoutedEventArgs e)
        {
            CurrentNode = (DirectoryNode)((TreeViewItem)e.OriginalSource).Header;
        }

        public ProjectViewModel(DirectoryTree directoryTree) {

            DirectoryTree = directoryTree;
        }

        public bool Init()
        {
            CurrentNode = DirectoryTree.SelectedProject;
            CurrentNode = DirectoryTree.LatestNode;
                        //OnPropertyChanged(nameof(DirectoryTree));
            //OnPropertyChanged(nameof(CurrentNode));
            CreateSessionCommand = new CreateSessionCommand(this);
            DeleteSessionCommand = new DeleteSessionCommand(this);
            SaveAsSessionCommand = new SaveAsSessionCommand(this);
            CreateSectionCommand = new CreateSectionCommand(this);
            return true;
        }

        public DeleteSessionCommand DeleteSessionCommand { get; protected set; }
        public CreateSessionCommand CreateSessionCommand { get; protected set; }
        public SaveAsSessionCommand SaveAsSessionCommand { get; protected set; }
        public OpenSessionSelectionCommand OpenCommonAnalysCommand { get; protected set; }
        public CreateSectionCommand CreateSectionCommand { get; protected set; }

        public string ProjectName
        {
            get
            {
                return DirectoryTree.SelectedProject.Name;
            }
            private set { }
        }

        public bool CreateSection()
        {
            Section section = new Section(CurrentNode.HierarhyElem as Project);
            section.CopyUnitsFrom(SampleContext.Path);
            DirectoryTree.RefreshHierarhy();
            CurrentNode = DirectoryTree.LatestNode;
            OnPropertyChanged(nameof(CurrentNode));
            OnPropertyChanged(nameof(DirectoryTree.NodesTree));
            OnPropertyChanged("NeedReload");
            return true;
        }


        public bool CreateSession()
        {
            var lastpath = SampleContext.Path;
            Session session = new Session(CurrentNode.HierarhyElem);
            session.CopyUnitsFrom(lastpath);
            DirectoryTree.RefreshHierarhy();
            CurrentNode = DirectoryTree.LatestNode;
            OnPropertyChanged(nameof(CurrentNode));
            OnPropertyChanged(nameof(DirectoryTree.NodesTree));
            OnPropertyChanged("NeedReload");
            return true;
        }

        public bool CopySession()
        {
            DirectoryNode copy_to;
            SessionDataViewModel sessionData = new SessionDataViewModel(DirectoryTree);
            sessionData.Visibility = Visibility.Visible;
            sessionData.Title = "Копировать расчет " + CurrentNode.Name;
            InputSessionDataView inputSessionDataView = new InputSessionDataView();

            inputSessionDataView.DataContext = sessionData;
            if (inputSessionDataView.ShowDialog() is true)
            {
                string name = sessionData.Name;
                copy_to = sessionData.selectedNode;
                if (copy_to.HierarhyElem.AddCopy(CurrentNode.HierarhyElem as Session, name,sessionData.FieldsToCopy, sessionData.Settings))
                {
                    DirectoryTree.RefreshHierarhy();
                    CurrentNode = DirectoryTree.LatestNode;
                    OnPropertyChanged(nameof(CurrentNode));
                    OnPropertyChanged(nameof(DirectoryTree.NodesTree));
                    OnPropertyChanged("NeedReload");
                    return true;
                }
            }
            return false;
        }

        public bool DeleteNode()
        {
            if (CurrentNode is null)
                return false;

            CurrentNode.HierarhyElem.Delete();
            currentNode = null;
            DirectoryTree.RefreshHierarhy();
            CurrentNode = DirectoryTree.LatestNode;
            OnPropertyChanged(nameof(CurrentNode));
            OnPropertyChanged(nameof(DirectoryTree.NodesTree));
            OnPropertyChanged("NeedReload");
            return true;
        }
    }
}
