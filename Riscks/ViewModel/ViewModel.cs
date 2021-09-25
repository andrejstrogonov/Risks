using  System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using Riscks.Commands;
using SQLiteLibrary;
using System.Globalization;
using System.Threading;

namespace Riscks.ViewModel
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public ViewModel(CommonViewModel commonViewModel=null)
        {
            CommonViewModel = commonViewModel;
        }
        public CommonViewModel CommonViewModel;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            try
            {
                
                if (!(PropertyChanged is null) && !(prop is null))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
                
            }
            catch(ApplicationException e)
            {
                throw new LicenseException(this.GetType());
            }
            catch(Exception e){
                return;
            }
        }
        public delegate void ObjectChanged(long id);


        public void Logger(TypeOfHistoryItem typeOfItem, long id, string Property, object Last, object New, string Title, string TitleChange)
        {
            HistoryItem historyItem = new HistoryItem();
            historyItem.TypeOfItem = typeOfItem;
            historyItem.Property = Property;
            historyItem.ID = id;
            historyItem.LastValue = Last;
            historyItem.NewValue = New;
            historyItem.DateTime = DateTime.Now;
            historyItem.Title = Title;
            var str = TitleChange.Split('\n');
            var str_res = "";
            if (str.Count() == 2)
            {
                double n1 = 0;
                if (double.TryParse(str[0], out n1))
                {
                    str_res += n1.ToString("N", CultureInfo.CurrentCulture);
                }
                else
                {
                    str_res += str[0];
                }
                str_res += "\n";

                double n2 = 0;
                if (double.TryParse(str[1], out n2))
                {
                    str_res += n2.ToString("N", CultureInfo.CurrentCulture);
                }
                else
                {
                    str_res += str[1];
                }
            }
            else
            {
                str_res = TitleChange;
            }
            historyItem.TitleChange = str_res;
            switch (typeOfItem)
            {
                case TypeOfHistoryItem.FixedCost:  CommonViewModel.HistoryViewModel_FixedCosts.AddItem(historyItem);break;
                case TypeOfHistoryItem.Resource: CommonViewModel.HistoryViewModel_Resources.AddItem(historyItem);break;
                case TypeOfHistoryItem.ResourceOfProduct:
                case TypeOfHistoryItem.RiskOfProduct:
                case TypeOfHistoryItem.Product: CommonViewModel.HistoryViewModel_Products.AddItem(historyItem);break;
                case TypeOfHistoryItem.Settings: CommonViewModel.HistoryViewModel_Settings.AddItem(historyItem);break;   
            }
            
        }

        public static string BoolToStr(bool? value)
        {
            if (value is true)
                return "Да";
            if (value is false)
                return "Нет";
            return "Не указано";
        }
    }
}
