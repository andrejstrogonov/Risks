using Riscks.Commands;
using SQLiteLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class IndexGroupViewModel:ViewModel
    {
        public ObservableCollection<GroupViewModel> Groups { get; set; }

        private GroupViewModel currentGroup;
        public GroupViewModel CurrentGroup
        {
            get
            {
                return currentGroup;
            }
            set
            {
                currentGroup = value;
                OnPropertyChanged("CurrentGroup");
                if (!(currentGroup is null))
                    currentGroup.Save();
            }
        }

        public IndexGroupViewModel(CommonViewModel commonViewModel)
        {
            CommonViewModel = commonViewModel;
            Groups = new ObservableCollection<GroupViewModel>();
            foreach(var g in GroupDAL.GetGroups())
            {
                Groups.Add(new GroupViewModel(g));
            }
            if (Groups.Count > 0)
                CurrentGroup = Groups.First();
            OnPropertyChanged(nameof(Groups));
            DeleteCommand = new DeleteGroupCommand(this);
            AddCommand = new AddGroupCommand(this);
        }

        public void Reload()
        {
            Groups.Clear();
            foreach (var g in GroupDAL.GetGroups())
            {
                Groups.Add(new GroupViewModel(g));
            }
            if (Groups.Count > 0)
                CurrentGroup = Groups.First();
            OnPropertyChanged(nameof(Groups));
        }

        public void ReloadProducts()
        {
            foreach(var g in Groups)
            {
                g.ReloadProducts();
            }
        }

        public void ReloadUnits()
        {
            foreach(var g in Groups)
            {
                g.ReloadUnits();
            }
        }

        public DeleteGroupCommand DeleteCommand { get; set; }
        public AddGroupCommand AddCommand { get; set; }

        public void AddGroup()
        {
            GroupViewModel groupViewModel = new GroupViewModel();
            Groups.Add(groupViewModel);
            CurrentGroup = groupViewModel;
            OnPropertyChanged(nameof(Groups));
        }

        public void DeleteGroup()
        {
            if (!(CurrentGroup is null))
            {
                CurrentGroup.Delete();
                Groups.Remove(CurrentGroup);
                OnPropertyChanged("CurrentGroup");
            }
        }
    }
}
