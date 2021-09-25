using ProjectManagerLibrary.WorkWithProjects;
using Riscks.Commands;
using SQLiteLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class AnalysSettingsViewModel:ViewModel
    {
        private AnalysSettingsSetuper AnalysSettingsSetuper;
        private AnalysSettings AnalysSettings;
        private HierarhyElem HierarhyElem;

        public AnalysSettingsViewModel(HierarhyElem hierarhyElem):base()
        {
            HierarhyElem = hierarhyElem;
            AnalysSettingsSetuper = HierarhyElem.AnalysSettingsSetuper;
            AnalysSettings = AnalysSettingsSetuper.Get();
            SaveCommand = new SaveAnalysSettingsCommand(this);
            BackCommand = new BackAnalysSettingsCommand(this);
            DefaultCommand = new DefaultAnalysSettingCommand(this);
            OnPropertyChanged(nameof(AnalysSettings));
            OnPropertyChanged(nameof(StartRisk));
            OnPropertyChanged(nameof(FinishRisk));
            OnPropertyChanged(nameof(Param0));
            OnPropertyChanged(nameof(Param1));
            OnPropertyChanged(nameof(Param2));
            OnPropertyChanged(nameof(Param3));
            OnPropertyChanged(nameof(Param4));
            OnPropertyChanged(nameof(Param5));
            OnPropertyChanged(nameof(Param6));
            OnPropertyChanged(nameof(Param7));
            OnPropertyChanged(nameof(Param8));

            OnPropertyChanged(nameof(Graph0));
            OnPropertyChanged(nameof(Graph1));
            OnPropertyChanged(nameof(Graph2));
            OnPropertyChanged(nameof(Graph3));
            OnPropertyChanged(nameof(Graph4));
        }

        public int StartRisk
        {
            get
            {
                return AnalysSettings.StartRisk;
            }
            set
            {
                AnalysSettings.StartRisk = value;
                OnPropertyChanged(nameof(StartRisk));
                OnPropertyChanged(nameof(OverRange));
            }
        }
        public int FinishRisk
        {
            get
            {
                return AnalysSettings.FinishRisk;
            }
            set
            {
                AnalysSettings.FinishRisk = value;
                OnPropertyChanged(nameof(FinishRisk));
                OnPropertyChanged(nameof(OverRange));
            }
        }

        public bool OverRange
        {
            get
            {
                return FinishRisk - StartRisk > 30;
            }
        }

        public bool Param0
        {
            get
            {
                return AnalysSettings.AnalysParams[0];
            }
            set
            {
                AnalysSettings.AnalysParams[0] = value;
                OnPropertyChanged(nameof(Param0));
            }
        }
        public bool Param1
        {
            get
            {
                return AnalysSettings.AnalysParams[1];
            }
            set
            {
                AnalysSettings.AnalysParams[1] = value;
                OnPropertyChanged(nameof(Param1));
            }
        }
        public bool Param2
        {
            get
            {
                return AnalysSettings.AnalysParams[2];
            }
            set
            {
                AnalysSettings.AnalysParams[2] = value;
                OnPropertyChanged(nameof(Param2));
            }
        }
        public bool Param3
        {
            get
            {
                return AnalysSettings.AnalysParams[3];
            }
            set
            {
                AnalysSettings.AnalysParams[3] = value;
                OnPropertyChanged(nameof(Param3));
            }
        }
        public bool Param4
        {
            get
            {
                return AnalysSettings.AnalysParams[4];
            }
            set
            {
                AnalysSettings.AnalysParams[4] = value;
                OnPropertyChanged(nameof(Param4));
            }
        }
        public bool Param5
        {
            get
            {
                return AnalysSettings.AnalysParams[5];
            }
            set
            {
                AnalysSettings.AnalysParams[5] = value;
                OnPropertyChanged(nameof(Param5));
            }
        }
        public bool Param6
        {
            get
            {
                return AnalysSettings.AnalysParams[6];
            }
            set
            {
                AnalysSettings.AnalysParams[6] = value;
                OnPropertyChanged(nameof(Param6));
            }
        }
        public bool Param7
        {
            get
            {
                return AnalysSettings.AnalysParams[7];
            }
            set
            {
                AnalysSettings.AnalysParams[7] = value;
                OnPropertyChanged(nameof(Param7));
            }
        }
        public bool Param8
        {
            get
            {
                return AnalysSettings.AnalysParams[8];
            }
            set
            {
                AnalysSettings.AnalysParams[8] = value;
                OnPropertyChanged(nameof(Param8));
            }
        }

        public bool Graph0
        {
            get
            {
                return AnalysSettings.AnalysGraphics[0];
            }
            set
            {
                AnalysSettings.AnalysGraphics[0] = value;
                OnPropertyChanged(nameof(Graph0));
            }
        }
        public bool Graph1
        {
            get
            {
                return AnalysSettings.AnalysGraphics[1];
            }
            set
            {
                AnalysSettings.AnalysGraphics[1] = value;
                OnPropertyChanged(nameof(Graph1));
            }
        }
        public bool Graph2
        {
            get
            {
                return AnalysSettings.AnalysGraphics[2];
            }
            set
            {
                AnalysSettings.AnalysGraphics[2] = value;
                OnPropertyChanged(nameof(Graph2));
            }
        }
        public bool Graph3
        {
            get
            {
                return AnalysSettings.AnalysGraphics[3];
            }
            set
            {
                AnalysSettings.AnalysGraphics[3] = value;
                OnPropertyChanged(nameof(Graph3));
            }
        }
        public bool Graph4
        {
            get
            {
                return AnalysSettings.AnalysGraphics[4];
            }
            set
            {
                AnalysSettings.AnalysGraphics[4] = value;
                OnPropertyChanged(nameof(Graph4));
            }
        }

        public void SetDefault()
        {
            StartRisk = 70;
            FinishRisk = 100;
            Param0 = true;
            Param1 = true;
            Param2 = true;
            Param3 = true;
            Param4 = true;
            Param5 = true;
            Param6 = true;
            Param7 = true;
            Param8 = true;
            Graph0 = true;
            Graph1 = true;
            Graph2 = true;
            Graph3 = true;
            Graph4 = false;
        }

        public void Save()
        {
            HierarhyElem.SetAnalysSettings(AnalysSettings);
        }

        public void Back()
        {
            AnalysSettings = AnalysSettingsSetuper.GetFromFile();
            OnPropertyChanged(nameof(AnalysSettings));
            OnPropertyChanged(nameof(StartRisk));
            OnPropertyChanged(nameof(FinishRisk));
            OnPropertyChanged(nameof(OverRange));
            OnPropertyChanged(nameof(Param0));
            OnPropertyChanged(nameof(Param1));
            OnPropertyChanged(nameof(Param2));
            OnPropertyChanged(nameof(Param3));
            OnPropertyChanged(nameof(Param4));
            OnPropertyChanged(nameof(Param5));
            OnPropertyChanged(nameof(Param6));
            OnPropertyChanged(nameof(Param7));
            OnPropertyChanged(nameof(Param8));

            OnPropertyChanged(nameof(Graph0));
            OnPropertyChanged(nameof(Graph1));
            OnPropertyChanged(nameof(Graph2));
            OnPropertyChanged(nameof(Graph3));
            OnPropertyChanged(nameof(Graph4));
        }

        public SaveAnalysSettingsCommand SaveCommand { get; set; }
        public BackAnalysSettingsCommand BackCommand { get; set; }
        public DefaultAnalysSettingCommand DefaultCommand { get; set; }

    }


}
