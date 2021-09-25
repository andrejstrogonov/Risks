using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using Riscks.Commands;
using DataProcessLibrary;
using DataProcessLibrary.Analys;
using SQLiteLibrary;

namespace Riscks.ViewModel
{
    public class MainSettingsViewModel:ViewModel
    {
        private Settings settings;
        
        public Dictionary<VATType, string> Vattypes { get; set; }

        public Settings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
                SettingsDAL.SaveSettings(settings);
                OnPropertyChanged("Settings");
            }
        }



        public MainSettingsViewModel()
        {
            settings = SettingsDAL.GetSettings();
            Vattypes = new Dictionary<VATType, string>();
            Vattypes[VATType.main_6] ="УСН 6%";
            Vattypes[VATType.main_15] = "УСН 15%";
            Vattypes[VATType.full] = "ПСН";
        }

        public double Budget
        {
            get
            {
                return settings.Budget;
            }
            set
            {
                Settings.Budget = value;
                SettingsDAL.SaveSettings(settings);
                OnPropertyChanged("Budget");
            }
        }

        public VATType VATType
        {
            get
            {
                return settings.VAT_type;
            }
            set
            {
                Settings.VAT_type = value;
                SettingsDAL.SaveSettings(settings);
                OnPropertyChanged("VATType");
            }
        }

        public KeyValuePair<VATType,string> VATTypeName
        {
            get
            {
                return new KeyValuePair<VATType, string>(settings.VAT_type,Vattypes[settings.VAT_type]);
            }
            set
            {
                VATType = value.Key;
                OnPropertyChanged("VATTypeName");
            }
        }

        public void Save()
        {
            SettingsDAL.SaveSettings(settings);
        }
    }
}
