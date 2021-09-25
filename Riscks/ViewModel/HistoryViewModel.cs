using ProjectManagerLibrary.WorkWithProjects;
using SQLiteLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Riscks.ViewModel
{
    public class HistoryItemViewModel
    {
        private HistoryItem HistoryItem;

        public HistoryItemViewModel(HistoryItem historyItem)
        {
            HistoryItem = historyItem;
        }

        public string TitleBold
        {
            get
            {
                return HistoryItem.Title.Split('\n').First();
            }
        }

        public string TitleNoBold
        {
            get
            {
                if(HistoryItem.Title.Split('\n').Count()>1)
                return HistoryItem.Title.Split('\n')[1];
                return "";
            }
        }

        public string Date
        {
            get
            {
                return HistoryItem.DateTime.ToShortDateString();
            }
        }

        public string Time
        {
            get
            {
                return HistoryItem.DateTime.ToShortTimeString();
            }
        }

        public DateTime DateTime
        {
            get
            {
                return HistoryItem.DateTime;
            }
        }

        public string TitleChange
        {
            get
            {
                return HistoryItem.TitleChange;
            }
        }
    }

    public class HistoryViewModel:ViewModel
    {
        private TypeOfHistoryItem TypeOfHistoryItem;
        private HierarhyElem HierarhyElem;
        public HistoryViewModel(TypeOfHistoryItem typeOfHistoryItem, HierarhyElem hierarhyElem)
        {
            HierarhyElem = hierarhyElem;
            TypeOfHistoryItem = typeOfHistoryItem;
            titles = new ObservableCollection<HistoryItemViewModel>();
            foreach(var t in HierarhyElem.HistorySetuper.Get().GetHistory(TypeOfHistoryItem))
            {
                titles.Insert(0,new HistoryItemViewModel(t));
            }
            historyCollectionView = new CollectionViewSource()
            {
                Source = titles
            };
            historyCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Date"));
            historyCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTime", System.ComponentModel.ListSortDirection.Descending));
        }

        private CollectionViewSource historyCollectionView;

        public CollectionViewSource HistoryCollectionView
        {
            get
            {
                return historyCollectionView;
            }
            set
            {
                historyCollectionView = value;
                OnPropertyChanged(nameof(HistoryCollectionView));
            }
        }


        private ObservableCollection<HistoryItemViewModel> titles;

        public void AddItem(HistoryItem historyItem)
        {
            var history = HierarhyElem.HistorySetuper.Get();
            history.Add(historyItem);
            titles.Insert(0, new HistoryItemViewModel(historyItem));
            OnPropertyChanged(nameof(titles));
        }

        public void AddItems(List<HistoryItem> items)
        {
            var history = HierarhyElem.HistorySetuper.Get();
            foreach(var i in items)
            {
                history.Add(i);
                titles.Insert(0, new HistoryItemViewModel(i));
            }            
            OnPropertyChanged(nameof(titles));
        }

        public void Reload()
        {
            titles = new ObservableCollection<HistoryItemViewModel>();
            foreach (var t in HierarhyElem.HistorySetuper.Get().GetHistory(TypeOfHistoryItem))
            {
                titles.Insert(0,new HistoryItemViewModel(t));
            }
            historyCollectionView = new CollectionViewSource()
            {
                Source = titles
            };
            historyCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Date"));
            historyCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTime", System.ComponentModel.ListSortDirection.Descending));

            OnPropertyChanged(nameof(titles));
            OnPropertyChanged(nameof(HistoryCollectionView));
        }

    }
}
