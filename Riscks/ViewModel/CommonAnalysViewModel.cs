using DataProcessLibrary;
using DataProcessLibrary.Analys;
using DataProcessLibrary.CommonAnalys;
using DataProcessLibrary.InitialData;
using ProjectManagerLibrary.WorkWithProjects;
using Riscks.Commands;
using SQLiteLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Point = DataProcessLibrary.Point;

namespace Riscks.ViewModel
{
   /* public class CommonAnalysViewModel:ViewModel
    {
        public static double WidthOfPlot { get; set; }
        public static double HeightOfPlot { get; set; }

        public double Minimum { get; set; }

        public double Maximum { get; set; }

        public double Interval { get; set; }


        public GraphicViewModel main_graphic { get; set; }

        public GraphicViewModel perish_graphic { get; set; }

        public GraphicViewModel bad_graphic { get; set; }

        public GraphicViewModel adviced_graphic { get; set; }

        private HierarhyElem_Analys HierarhyElem_Analys;
        private AnalysSettings AnalysSettings;
        private static SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(239, 237, 240));
        private static SolidColorBrush White = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        private bool[] colors;

        public Visibility bestVis
        {
            get
            {
                return AnalysSettings.AnalysGraphics[1] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility adviceVis
        {
            get
            {
                return AnalysSettings.AnalysGraphics[0] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility badVis
        {
            get
            {
                return AnalysSettings.AnalysGraphics[2] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility perishVis
        {
            get
            {
                return AnalysSettings.AnalysGraphics[3] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param0
        {
            get
            {
                return AnalysSettings.AnalysParams[0] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param1
        {
            get
            {
                return AnalysSettings.AnalysParams[1] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param2
        {
            get
            {
                return AnalysSettings.AnalysParams[2] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param3
        {
            get
            {
                return AnalysSettings.AnalysParams[3] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param4
        {
            get
            {
                return AnalysSettings.AnalysParams[4] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param5
        {
            get
            {
                return AnalysSettings.AnalysParams[5] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param6
        {
            get
            {
                return AnalysSettings.AnalysParams[6] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param7
        {
            get
            {
                return AnalysSettings.AnalysParams[7] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility param8
        {
            get
            {
                return AnalysSettings.AnalysParams[8] ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public SolidColorBrush param0_back
        {
            get
            {
                if (colors[0])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param1_back
        {
            get
            {
                if (colors[1])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param2_back
        {
            get
            {
                if (colors[2])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param3_back
        {
            get
            {
                if (colors[3])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param4_back
        {
            get
            {
                if (colors[4])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param5_back
        {
            get
            {
                if (colors[5])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param6_back
        {
            get
            {
                if (colors[6])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param7_back
        {
            get
            {
                if (colors[7])
                {
                    return Gray;
                }
                return White;
            }
        }
        public SolidColorBrush param8_back
        {
            get
            {
                if (colors[8])
                {
                    return Gray;
                }
                return White;
            }
        }
               
        public Dictionary<double,CommonDecidionGraphic> Decidions { get; set; }

        public CommonDecidionGraphic CurrentDecidion { get; set; }
               
        //public IndexCommonDecidionViewModel CurrentScript_ForCommon { get; set; }
        
        //private List<Session> sessions;

        public bool CanShow { get; set; }

        private HierarhyElem HierarhyElem;

        public CommonAnalysViewModel()
        {
            OpenCommonGraphicCommand = new OpenCommonGraphicCommand(this);
            AnalysSettings = new AnalysSettings();
            ReloadInterface();
        }


        public void Reload(HierarhyElem hierarhyElem)
        {
            HierarhyElem = hierarhyElem;
            AnalysSettings = hierarhyElem.GetProject().AnalysSettingsSetuper.Get();
            HierarhyElem_Analys = InitialData.GetAnalys(hierarhyElem);
            //sessions = null;
            decidionGraphic = HierarhyElem_Analys.GetDecidionGraphic() as CommonDecidionGraphic;
            colors = new bool[9];
            bool ok = true;
            for (int i = 0; i <= 8; i++)
            {
                if (AnalysSettings.AnalysParams[i])
                {
                    colors[i] = ok;
                    ok = !ok;
                }
            }
            if (!(decidionGraphic is null))
            {
                Init();
            }
            else
            {
                Clear();
            }
        }

        public void Execute()
        {
            try
            {
                decidionGraphic = HierarhyElem_Analys.Analys() as CommonDecidionGraphic;

                if (decidionGraphic is null)
                    return;
                Init();


            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public void Clear()
        {
            alldecidions_forcommon = new Dictionary<double, IndexCommonDecidionViewModel>();

            main_graphic = new GraphicViewModel(new ObservableCollection<DataProcessLibrary.Point>());
            perish_graphic = new GraphicViewModel(new ObservableCollection<DataProcessLibrary.Point>());
            bad_graphic = new GraphicViewModel(new ObservableCollection<DataProcessLibrary.Point>());
            adviced_graphic = new GraphicViewModel(new ObservableCollection<DataProcessLibrary.Point>());
            CurrentScript_ForCommon = null;
            Maximum = -999999;
            Minimum = -1000000;
            Interval = 0;

            OnPropertyChanged(nameof(main_graphic));
            OnPropertyChanged(nameof(perish_graphic));
            OnPropertyChanged(nameof(bad_graphic));
            OnPropertyChanged(nameof(adviced_graphic));
            OnPropertyChanged(nameof(CurrentScript_ForCommon));

            OnPropertyChanged(nameof(Maximum));
            OnPropertyChanged(nameof(Minimum));
            OnPropertyChanged(nameof(Interval));
            OnPropertyChanged(nameof(alldecidions_forcommon));
        }

        public void ReloadInterface()
        {
            OnPropertyChanged(nameof(param0));
            OnPropertyChanged(nameof(param1));
            OnPropertyChanged(nameof(param2));
            OnPropertyChanged(nameof(param3));
            OnPropertyChanged(nameof(param4));
            OnPropertyChanged(nameof(param5));
            OnPropertyChanged(nameof(param6));
            OnPropertyChanged(nameof(param7));
            OnPropertyChanged(nameof(param8));
            colors = new bool[9];
            bool ok = true;
            for (int i = 0; i <= 8; i++)
            {
                if (AnalysSettings.AnalysParams[i])
                {
                    colors[i] = ok;
                    ok = !ok;
                }
            }
            OnPropertyChanged(nameof(param0_back));
            OnPropertyChanged(nameof(param1_back));
            OnPropertyChanged(nameof(param2_back));
            OnPropertyChanged(nameof(param3_back));
            OnPropertyChanged(nameof(param4_back));
            OnPropertyChanged(nameof(param5_back));
            OnPropertyChanged(nameof(param6_back));
            OnPropertyChanged(nameof(param7_back));
            OnPropertyChanged(nameof(param8_back));
            OnPropertyChanged(nameof(bestVis));
            OnPropertyChanged(nameof(badVis));
            OnPropertyChanged(nameof(perishVis));
            OnPropertyChanged(nameof(adviceVis));
        }


        public void Init()
        {
            if (decidionGraphic is null || WidthOfPlot == 0)
                return;
            CanShow = true;
            Clear();

            main_graphic = new GraphicViewModel(decidionGraphic.best_points);
            perish_graphic = new GraphicViewModel(decidionGraphic.perish_points);
            bad_graphic = new GraphicViewModel(decidionGraphic.bad_points);
            adviced_graphic = new GraphicViewModel(decidionGraphic.adviced_points);
            foreach (var d in decidionGraphic.decidionGraphic)
            {
                alldecidions_forcommon[d.Key] = new IndexCommonDecidionViewModel(d.Value as CommonScript, d.Key);
            }
            if (alldecidions_forcommon.Count() > 0)
            {
                CurrentScript_ForCommon = alldecidions_forcommon.First().Value;
                OnPropertyChanged(nameof(CurrentScript_ForCommon));
            }



            main_graphic.OnChanged += ChangeCurrentDecidionScript;
            main_graphic.OnChanged += adviced_graphic.ChangePoint;
            main_graphic.OnChanged += bad_graphic.ChangePoint;
            main_graphic.OnChanged += perish_graphic.ChangePoint;

            bad_graphic.OnChanged += ChangeCurrentDecidionScript;
            bad_graphic.OnChanged += adviced_graphic.ChangePoint;
            bad_graphic.OnChanged += main_graphic.ChangePoint;
            bad_graphic.OnChanged += perish_graphic.ChangePoint;

            perish_graphic.OnChanged += ChangeCurrentDecidionScript;
            perish_graphic.OnChanged += main_graphic.ChangePoint;
            perish_graphic.OnChanged += adviced_graphic.ChangePoint;
            perish_graphic.OnChanged += bad_graphic.ChangePoint;

            adviced_graphic.OnChanged += ChangeCurrentDecidionScript;
            adviced_graphic.OnChanged += main_graphic.ChangePoint; ;
            adviced_graphic.OnChanged += bad_graphic.ChangePoint; ;
            adviced_graphic.OnChanged += perish_graphic.ChangePoint;



            OnPropertyChanged(nameof(CanShow));

            Minimum = Math.Round(bad_graphic.Points.Min(p => p.Y));
            Minimum = Math.Round(Minimum * (1 - Math.Sign(Minimum) * 0.05) - 10, 2);
            Maximum = Math.Round(main_graphic.Points.Max(p => p.Y));
            Maximum = Math.Round(Maximum * (1 + Math.Sign(Maximum) * 0.05) + 10, 2);
            double width = WidthOfPlot - 10 - Math.Round(Math.Log10(Maximum) + 1) * 8;
            double min = main_graphic.Points.Min(p => p.X);
            double max = main_graphic.Points.Max(p => p.X);


            StartRisk = min - 1;
            FinishRisk = max + 1;

            OnPropertyChanged(nameof(FinishRisk));
            OnPropertyChanged(nameof(StartRisk));

            double lenOfIntegrval = width / (FinishRisk - StartRisk - 1);
            double y_min = Minimum;
            double y_max = Maximum;
            double y_width = HeightOfPlot;
            double y_len = y_max - y_min;
            if (y_len > 0)
            {
                Interval = Math.Round(lenOfIntegrval * y_len / y_width);
                OnPropertyChanged(nameof(Maximum));
                OnPropertyChanged(nameof(Minimum));
                OnPropertyChanged(nameof(Interval));
                OnPropertyChanged(nameof(main_graphic));
                OnPropertyChanged(nameof(perish_graphic));
                OnPropertyChanged(nameof(bad_graphic));
                OnPropertyChanged(nameof(adviced_graphic));
                OnPropertyChanged(nameof(CurrentScript_ForCommon));
            }
        }


        public OpenCommonGraphicCommand OpenCommonGraphicCommand { get; set; }



        public double SecToInit { get; set; }

        public double SecToDecide { get; set; }
        public double StartRisk { get; private set; }
        public double FinishRisk { get; private set; }
        
        public void ChangeCurrentDecidionScript(Point point)
        {
            if (point is null)
                return;
            CurrentScript_ForCommon = alldecidions_forcommon[point.X];
            OnPropertyChanged(nameof(CurrentScript_ForCommon));
        }
    }*/

}
