using ClientServerLibrary;
using Riscks.Commands;
using Riscks.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Riscks.ViewModel.LauncherViewModel;

namespace Riscks.ViewModel
{
    public class ProjectInfo
    {
        public string Time { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public Node_Project Project { get; set; }
        public int DisplayIndex { get; set; }

        public string FullPath { get; set; }

        public ProjectInfo()
        {
            DeleteProjectCommand = new DeleteProjectCommand(this);
            SelectProjectCommand = new SelectProjectCommand(this);
        }

        public void DeleteProject()
        {
            Project.DeleteProject();
            if(!(OnProjectDeleted is null))
                OnProjectDeleted(this);
        }

        public void Select()
        {
            if (!(OnProjectDeleted is null))
                OnProjectSelected(this);
        }

        public event DeleteProject OnProjectDeleted;
        public event SelectProject OnProjectSelected;
        
        public SelectProjectCommand SelectProjectCommand { get; set; }
        public DeleteProjectCommand DeleteProjectCommand { get; set; }
    }

    public class LauncherViewModel:ViewModel
    {
        
        public List<ProjectInfo> ProjectsInfo { get; set; }

        public DirectoryTree DirectoryTree { get; set; }

        public ProjectInfo SelectedProject
        {
            set
            {
                if (value is null)
                    return;                
            }
        }

        public delegate void SelectProject(ProjectInfo projectInfo);

        public void Select(ProjectInfo projectInfo)
        {
            DirectoryTree.SelectProject(projectInfo.Project);
            MainWindowView mainWindowView = new MainWindowView(DirectoryTree);
            mainWindowView.Show();
            if(!(LauncherWindow is null))
                LauncherWindow.Close();
        }

        public delegate void DeleteProject(ProjectInfo projectInfo);

        public void DeleteProjectItem(ProjectInfo projectInfo)
        {
            ReloadProjects();
        }

        public LauncherWindow LauncherWindow { get; set; }
        public LauncherViewModel()
        {
            ReloadProjects();
        }

        public void ReloadProjects()
        {
            DirectoryTree = new DirectoryTree();
            ProjectsInfo = new List<ProjectInfo>();
            foreach (var p in DirectoryTree.Projects)
            {
                ProjectInfo projectInfo = new ProjectInfo();
                projectInfo.OnProjectDeleted += DeleteProjectItem;
                projectInfo.OnProjectSelected += Select;
                projectInfo.Name = p.Name;
                projectInfo.Date = p.DateOfChange.ToShortDateString();
                projectInfo.Time = p.DateOfChange.ToShortTimeString();
                projectInfo.Project = p as Node_Project;
                projectInfo.DisplayIndex = ProjectsInfo.Count();
                projectInfo.FullPath = (new DirectoryInfo(p.HierarhyElem.DirectoryPath).FullName);
                ProjectsInfo.Add(projectInfo);
            }
            SelectedProject = null;
            OnPropertyChanged(nameof(ProjectsInfo));
        }

    }
}
