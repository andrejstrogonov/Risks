using Riscks.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class MenuItemViewModel:ViewModel
    {
        private bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }


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
                if (value)
                    OnChange(Number);
                OnPropertyChanged(nameof(isSelected));
            }
        }
        private int Number;
        public MenuItemViewModel(int num) {
            Number = num;
            isSelected = false;
            isEnabled = true;
            SelectMenuItemCommand = new SelectMenuItemCommand(this);
        }

        public delegate void SelectionChanged(int num);
        public event SelectionChanged OnChange;

        public SelectMenuItemCommand SelectMenuItemCommand { get; set; }
    }

    public class MenuViewModel: ViewModel
    {
        private List<MenuItemViewModel> items;
        private int currentItem;

        public delegate void SelectionChanged(int num);
        public event SelectionChanged OnChange;


        public MenuViewModel()
        {
            currentItem = 0;
            items = new List<MenuItemViewModel>();
            for (int i = 0; i < 7; i++)
            {
                var item = new MenuItemViewModel(i);
                item.OnChange += ChangeSelection;
                items.Add(item);
            }
            items[currentItem].IsSelected = true;
        }

        public void ChangeSelection(int num)
        {
            if (num == 6)//вызов меню
            {
                items[6].IsSelected = false;
                OnChange(num);
            }
            else
            if (num != currentItem)
            {
                items[currentItem].IsSelected = false;
                currentItem = num;
                if(!items[currentItem].IsSelected)
                    items[currentItem].IsSelected = true;
                OnChange(num);
            }
        }

        public MenuItemViewModel Menu
        {
            get
            {
                return items[6];
            }
            set
            {
                items[6] = value;
                OnPropertyChanged(nameof(Menu));
            }
        }


        public MenuItemViewModel Products
        {
            get
            {
                return items[0];
            }
            set
            {
                items[0] = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public MenuItemViewModel Resources
        {
            get
            {
                return items[1];
            }
            set
            {
                items[1] = value;
                OnPropertyChanged(nameof(Resources));
            }
        }
        public MenuItemViewModel FixedCosts
        {
            get
            {
                return items[2];
            }
            set
            {
                items[2] = value;
                OnPropertyChanged(nameof(FixedCosts));
            }
        }
        public MenuItemViewModel Analys
        {
            get
            {
                return items[3];
            }
            set
            {
                items[3] = value;
                OnPropertyChanged(nameof(Analys));
            }
        }
        public MenuItemViewModel Flex
        {
            get
            {
                return items[4];
            }
            set
            {
                items[4] = value;
                OnPropertyChanged(nameof(Flex));
            }
        }
        public MenuItemViewModel PDF
        {
            get
            {
                return items[5];
            }
            set
            {
                items[5] = value;
                OnPropertyChanged(nameof(PDF));
            }
        }

        public void SelectProject(int LastProjectTab)
        {
            PDF.IsEnabled = true;
            Analys.IsEnabled = true;
            Flex.IsEnabled = false;
            Products.IsEnabled = false;
            Resources.IsEnabled = false;
            FixedCosts.IsEnabled = false;
            ChangeSelection(LastProjectTab);
        }

        public void SelectSession(int LastSessionTab)
        {
            PDF.IsEnabled = true;
            Analys.IsEnabled = true;
            Flex.IsEnabled = true;
            Products.IsEnabled = true;
            Resources.IsEnabled = true;
            FixedCosts.IsEnabled = true;
            ChangeSelection(LastSessionTab);
        }
    }
}
