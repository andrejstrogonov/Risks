using ProjectManagerLibrary.WorkWithProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class CreateProjectViewModel:ViewModel
    {
        public NodeProperty_Parent Project_Properies { get; set; }
        public Project Project { get; set; }

        public CreateProjectViewModel():base(null)
        {
            Init();
            Project = new Project(null);
            Project_Properies = new NodeProperty_Parent(Project);
            Project_Properies.Name = Project.Name;
            SelectType(0);

            
        }
        private int selectedtype;
        public void SelectType(int i)
        {
            selectedtype = i;
            first_elem.IsSelected = false;
            second_elem.IsSelected = false;
            third_elem.IsSelected = false;
            switch (i)
            {
                case 0:first_elem.IsSelected = true;break;
                case 1:second_elem.IsSelected = true;break;
                default:third_elem.IsSelected = true;break;
            }
            OnPropertyChanged(nameof(first_elem));
            OnPropertyChanged(nameof(second_elem));
            OnPropertyChanged(nameof(third_elem));
        }

        public void Save()
        {
            Project.Rename(Project_Properies.Name);
            Project.CreateHierarhy(selectedtype);
        }
        
        public void Cansel()
        {
            Project.Delete();
        }

        public DirectoryNode first_tree { get; set; }
        public DirectoryNode second_tree { get; set; }
        public DirectoryNode third_tree { get; set; }

        public class HelpNode : ViewModel
        {
            private bool isSelected;
            public bool IsSelected
            {
                get
                {
                    return isSelected;
                }
                set
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
            public HelpNode():base(null)
            {
                IsSelected = false;
            }

        }


        public HelpNode first_elem { get; set; }
        public HelpNode second_elem { get; set; }
        public HelpNode third_elem { get; set; }
        public void Init()
        {
            first_tree = new DirectoryNode("Проект");
            var node0 = new Node_Project("Проект", new List<DirectoryNode>());
            
            first_tree.Nodes=new List<DirectoryNode>() { node0 };

            second_tree = new DirectoryNode("Проект");

            var node2 = new Node_Session("Расчет 1");
            var node3 = new Node_Session("Расчет 2");
            var node4 = new Node_Session("Расчет 3");
            var node01 = new Node_Project("Проект", new List<DirectoryNode>() { node2, node3, node4 });
            second_tree.Nodes = new List<DirectoryNode>() { node01 };

            third_tree = new DirectoryNode("Проект");
            var node6 = new Node_Section("Раздел 1");
            var node7 = new Node_Section("Раздел 2");

            var node02 = new Node_Project("Проект", new List<DirectoryNode>() { node6, node7 });
            var node9 = new Node_Session("Расчет 1");
            var node10 = new Node_Session("Расчет 2");
            node6.Nodes = new List<DirectoryNode>() { node9, node10 };
            var node11 = new Node_Session("Расчет 3");
            node7.Nodes = new List<DirectoryNode>() { node11};
            
            third_tree.Nodes = new List<DirectoryNode>() { node02 };

            first_elem = new HelpNode();
            second_elem = new HelpNode();
            third_elem = new HelpNode();
            OnPropertyChanged(nameof(first_elem));
            OnPropertyChanged(nameof(second_elem));
            OnPropertyChanged(nameof(third_elem));
            OnPropertyChanged(nameof(first_tree));
            OnPropertyChanged(nameof(second_tree));
            OnPropertyChanged(nameof(third_tree));
        }

    }
}
