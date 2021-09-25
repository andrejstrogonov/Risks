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
using DataProcessLibrary;
using DataProcessLibrary.Analys;
using DataProcessLibrary.CommonAnalys;
using System.Windows;
using Point = DataProcessLibrary.Point;
using ProjectManagerLibrary.WorkWithProjects;
using DataProcessLibrary.InitialData;
using SQLiteLibrary;
using System.Windows.Media;

using ClientServerLibrary;
using License = ClientServerLibrary.License;
using Riscks.View;
using System.Threading;
using System.Windows.Threading;

namespace Riscks.ViewModel
{
    public class AnalysViewModel:ViewModel
    {
        private static double widthOfProt;
        public static double WidthOfPlot {
            get
            {
                return widthOfProt;
            }
            set
            {   if (value > widthOfProt)
                {
                    widthOfProt = value;
                    if (!(OnChangedWidth is null))
                        OnChangedWidth();
                }
            }
        }

        public static double LastMin = 0;
        public static double LastMax = 10000;

        public delegate void WidthChanged();
        public static event WidthChanged OnChangedWidth;
        
        public static double HeightOfPlot { get; set; }

        private double minimum;
        public double Minimum {
            get
            {
                if (minimum > LastMax)
                {
                    OnPropertyChanged(nameof(Maximum));
                    return LastMin;
                }
                LastMin = minimum;
                //LastMin = minimum;
                return minimum;
                
                
            }
            set
            {
                minimum = value;
                //OnPropertyChanged(nameof(Minimum));
            }
        }

        private double maximum;
        public double Maximum
        {
            get
            {
                if (maximum < LastMin)
                {
                    OnPropertyChanged(nameof(Minimum));
                    return LastMax;
                }
                LastMax = maximum;
                //LastMax = maximum;
                return maximum;
                              
            }
            set
            {
                maximum = value;
            }
        }

        public double Interval { get; set; }

        public double StartRisk { get; set; }

        public double FinishRisk { get; set; }
        public GraphicViewModel main_graphic { get;set; }
        public GraphicViewModel per_graphic { get; set; }
        public GraphicViewModel bad_graphic { get; set; }

        public GraphicViewModel adviced_graphic { get; set; }

        public string AxisHeader
        {
            get
            {
                return "Прибыль, "+ TransferOfUnitsDAL.GetCommonUnitForType(TypeOfUnit.Money).Symbol;
            }
        }


        private DecidionScriptViewModel currentScript;
        public DecidionScriptViewModel CurrentScript { get
            {
                return currentScript;
            }
            set
            {
                currentScript = value;
                OnPropertyChanged("CurrentScript");
            }
        }

        private Dictionary<double, DecidionScriptViewModel> alldecidions_forsingle;
        
        private DecisionGraphic decidionGraphic;
        private HierarhyElem_Analys HierarhyElem_Analys;
        private AnalysSettings AnalysSettings;
    
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

        private static SolidColorBrush Gray = new SolidColorBrush(Color.FromRgb(239, 237, 240));
        private static SolidColorBrush White = new SolidColorBrush(Color.FromRgb(255, 255, 255));

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


        public void ReloadInterface()
        {
            OnPropertyChanged(nameof(Maximum));
            OnPropertyChanged(nameof(Minimum));
           /* var max = Maximum;
            var min = Minimum;
            Minimum = LastMin;
            Maximum =  LastMax;
            OnPropertyChanged(nameof(Maximum));
            OnPropertyChanged(nameof(Minimum));*/
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
            OnPropertyChanged(nameof(MainTableIndex));
            OnPropertyChanged(nameof(HelpTableIndex));
            /*if (max > LastMax)
            {
                Maximum = max;
                Minimum = min;
                OnPropertyChanged(nameof(Maximum));
                OnPropertyChanged(nameof(Minimum));
            }
            else
            {
                Minimum = min;
                Maximum = max;
                OnPropertyChanged(nameof(Minimum));
                OnPropertyChanged(nameof(Maximum));
            }
            LastMax = Maximum;
            LastMin = Minimum;*/
            //OnPropertyChanged(nameof(Maximum));
            //OnPropertyChanged(nameof(Minimum));
            OnPropertyChanged(nameof(Interval));

            OnPropertyChanged(nameof(main_graphic));
            OnPropertyChanged(nameof(per_graphic));
            OnPropertyChanged(nameof(bad_graphic));
            OnPropertyChanged(nameof(adviced_graphic));
            OnPropertyChanged(nameof(CurrentScript));
        }

        public AnalysViewModel(CommonViewModel commonViewModel):base(commonViewModel)
        {
            OnChangedWidth += ReloadWidth;
            OpenGraphicCommand = new OpenGraphicCommand(this);
            AnalysSettings = new AnalysSettings();
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
            //Init();
            Reload();
        }

        private bool[] colors;

        public void Reload()
        {
            HierarhyElem_Analys = InitialData.GetAnalys(CommonViewModel.HierarhyElem);
            AnalysSettings = CommonViewModel.HierarhyElem.GetProject().AnalysSettingsSetuper.Get();
            IsEnabled = true;
            decidionGraphic = HierarhyElem_Analys.GetDecidionGraphic();
            Init();
            if (!(decidionGraphic is null))
            {
                LoadHierarhyElem();
            }

        }


        public bool IsEnabled { get; set; }


        

        public void Execute()
        {
            if (License.LicenseKey == "" || License.LicenseKey is null || !License.ActivateLicense(License.LicenseKey))
            {
                throw new ApplicationException("Лицензия отсутствует");
            }
            try
            {
                string title = "Анализ ";
                title += HierarhyElem_Analys.HierarhyElem is Session ? "расчета" : HierarhyElem_Analys.HierarhyElem is Section ? "раздела" : "проекта";
                title += " " + HierarhyElem_Analys.HierarhyElem.Name;
                ProgressLineViewModel progressLineViewModel = new ProgressLineViewModel(title);
                SimplexStage.OnSetMaximum += progressLineViewModel.SetMaximum;
                SimplexStage.OnProgress += progressLineViewModel.IncreaseProgress;
                SingleDecidionStage.OnSetState += progressLineViewModel.SetState;
                LoadingLineWindow loadingLineWindow = new LoadingLineWindow(progressLineViewModel);
                loadingLineWindow.Show();
                Thread thread = null;
                try
                {
                    thread = new Thread(
                    () => {

                        try
                        {
                            try
                            {
                                decidionGraphic = HierarhyElem_Analys.Analys();
                            }
                            catch(Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                            if (decidionGraphic is null)
                            {
                                progressLineViewModel.StopProgress();
                                loadingLineWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                                {
                                    loadingLineWindow.Close();
                                }));
                                SimplexStage.OnSetMaximum -= progressLineViewModel.SetMaximum;
                                SimplexStage.OnProgress -= progressLineViewModel.IncreaseProgress;
                                return;
                            }
                        
                        loadingLineWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            LoadHierarhyElem();
                        }));
                        }
                        catch (Exception e)
                        {
                            
                        }
                        progressLineViewModel.StopProgress();
                        loadingLineWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            loadingLineWindow.Close();
                        }));
                        SimplexStage.OnSetMaximum -= progressLineViewModel.SetMaximum;
                        SimplexStage.OnProgress -= progressLineViewModel.IncreaseProgress;
                    });
                    thread.Start();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    loadingLineWindow.Close();
                }
                
                //ReloadInterface();
                

            }
            catch (Exception e)
            { 
                MessageBox.Show(e.Message);
            }
        }

        public int HelpTableIndex { get;  set; }
        public int MainTableIndex { get;  set; }

        /*public void Clear()
        {
            main_graphic.Clear();
            per_graphic.Clear();
            bad_graphic.Clear();
            adviced_graphic.Clear();
            alldecidions_forsingle.Clear();
            CurrentScript = null;
            Maximum = -999999;
            Minimum = -1000000;
            Interval = 0;
            //OnPropertyChanged(nameof(main_graphic));
            //OnPropertyChanged(nameof(per_graphic));
            //OnPropertyChanged(nameof(bad_graphic));
            //OnPropertyChanged(nameof(adviced_graphic));
            //OnPropertyChanged(nameof(CurrentScript));


            OnPropertyChanged(nameof(Maximum));
            OnPropertyChanged(nameof(Minimum));
            OnPropertyChanged(nameof(Interval));

            //alldecidions_forsingle.Clear();
        }*/

        public void LoadHierarhyElem()
        {
            if (decidionGraphic is null)
                return;
            bool issession = HierarhyElem_Analys.HierarhyElem is Session;
            main_graphic=new GraphicViewModel(decidionGraphic.best_points.ToList());
            per_graphic = new GraphicViewModel(decidionGraphic.perish_points.ToList());
            bad_graphic = new GraphicViewModel(decidionGraphic.bad_points.ToList());
            adviced_graphic = new GraphicViewModel(decidionGraphic.adviced_points.ToList());
            main_graphic.OnChanged += per_graphic.ChangePoint;
            main_graphic.OnChanged += bad_graphic.ChangePoint;
            main_graphic.OnChanged += adviced_graphic.ChangePoint;
            main_graphic.OnChanged += ChangeCurrentDecidionScript;

            per_graphic.OnChanged += main_graphic.ChangePoint;
            per_graphic.OnChanged += adviced_graphic.ChangePoint;
            per_graphic.OnChanged += bad_graphic.ChangePoint;
            per_graphic.OnChanged += ChangeCurrentDecidionScript;

            bad_graphic.OnChanged += adviced_graphic.ChangePoint;
            bad_graphic.OnChanged += main_graphic.ChangePoint;
            bad_graphic.OnChanged += per_graphic.ChangePoint;
            bad_graphic.OnChanged += ChangeCurrentDecidionScript;

            adviced_graphic.OnChanged += main_graphic.ChangePoint;
            adviced_graphic.OnChanged += per_graphic.ChangePoint;
            adviced_graphic.OnChanged += bad_graphic.ChangePoint;
            adviced_graphic.OnChanged += ChangeCurrentDecidionScript;

            alldecidions_forsingle.Clear();
            var min = decidionGraphic.decidionGraphic.Min(p => p.Key);
            var max = decidionGraphic.decidionGraphic.Max(p => p.Key);
            foreach (var d in decidionGraphic.decidionGraphic)
            {
                //var procent = (max - d.Key) / (max - min) * 100;
                if (issession)
                    alldecidions_forsingle[d.Key] = new SingleDecidionScriptViewModel(d.Value as SingleDecidionScript, d.Key);
                else
                {
                    alldecidions_forsingle[d.Key] = new CommonDecidionScriptViewModel(d.Value as CommonScript, d.Key);
                }
            }                     

            if (alldecidions_forsingle.Count() > 0)
                CurrentScript = alldecidions_forsingle.First().Value;
            ReloadWidth();
            ReloadInterface();
        }

        public void ReloadWidth()
        {
            if (main_graphic is null)
                return;
            if (main_graphic.Points.Count() > 0)
            {
                minimum = Math.Round(bad_graphic.Points.Min(p => p.Y));
                maximum = Math.Round(main_graphic.Points.Max(p => p.Y));

                var delta = (maximum - minimum + 10)*0.2;

                minimum = minimum-delta;
                maximum = maximum+delta;
                double width = WidthOfPlot - 10 - Math.Round(Math.Log10(Math.Abs(maximum)) + 1) * 8;
                double min = main_graphic.Points.Min(p => p.X);
                double max = main_graphic.Points.Max(p => p.X);

                StartRisk = min - 0.2;
                FinishRisk = max + 0.2;

                OnPropertyChanged(nameof(FinishRisk));
                OnPropertyChanged(nameof(StartRisk));

                double lenOfIntegrval = width / ((max>min+1)? FinishRisk - StartRisk - 1:1);
                double y_min = minimum;
                double y_max = maximum;
                double y_width = HeightOfPlot;
                double y_len = y_max - y_min;
                if (y_len > 0 && width > 0)
                {
                    Interval = Math.Round(lenOfIntegrval * y_len / y_width);
                    OnPropertyChanged(nameof(Maximum));
                    OnPropertyChanged(nameof(Minimum));
                    OnPropertyChanged(nameof(Interval));

                }

                IsEnabled = true;
                OnPropertyChanged("IsEnabled");
            }
            else
            {
                double width = WidthOfPlot - 10 - Math.Round(Math.Log10(Math.Abs(maximum)) + 1) * 8;
                double lenOfIntegrval = width / (FinishRisk - StartRisk - 1);
                double y_min = minimum;
                double y_max = maximum;
                double y_width = HeightOfPlot;
                double y_len = y_max - y_min;
                if (y_len > 0 && width > 0)
                {
                    Interval = Math.Round(lenOfIntegrval * y_len / y_width);
                    OnPropertyChanged(nameof(Maximum));
                    OnPropertyChanged(nameof(Minimum));
             
                    OnPropertyChanged(nameof(Interval));
                }
            }
        }

        public void Init()
        {
            main_graphic = new GraphicViewModel(new List<Point>());
            per_graphic = new GraphicViewModel(new List<Point>());
            bad_graphic = new GraphicViewModel(new List<Point>());
            adviced_graphic = new GraphicViewModel(new List<Point>());
            alldecidions_forsingle = new Dictionary<double, DecidionScriptViewModel>();
            CurrentScript = null;
            maximum = LastMax;
            minimum = LastMin;
            StartRisk = AnalysSettings.StartRisk - 0.2;
            FinishRisk = AnalysSettings.FinishRisk + 0.2;
            Interval = 0;
            main_graphic.OnChanged += per_graphic.ChangePoint;
            main_graphic.OnChanged += bad_graphic.ChangePoint;
            main_graphic.OnChanged += adviced_graphic.ChangePoint;
            main_graphic.OnChanged += ChangeCurrentDecidionScript;

            per_graphic.OnChanged += main_graphic.ChangePoint;
            per_graphic.OnChanged += adviced_graphic.ChangePoint;
            per_graphic.OnChanged += bad_graphic.ChangePoint;
            per_graphic.OnChanged += ChangeCurrentDecidionScript;

            bad_graphic.OnChanged += adviced_graphic.ChangePoint;
            bad_graphic.OnChanged += main_graphic.ChangePoint;
            bad_graphic.OnChanged += per_graphic.ChangePoint;
            bad_graphic.OnChanged += ChangeCurrentDecidionScript;

            adviced_graphic.OnChanged += main_graphic.ChangePoint;
            adviced_graphic.OnChanged += per_graphic.ChangePoint;
            adviced_graphic.OnChanged += bad_graphic.ChangePoint;
            adviced_graphic.OnChanged += ChangeCurrentDecidionScript;

            bool issession = HierarhyElem_Analys.HierarhyElem is Session;
            if (issession)
            {
                MainTableIndex = 0;
                HelpTableIndex = 0;
            }
            else
            {
                MainTableIndex = 1;
                HelpTableIndex = 1;
            }

        }

        public double SecToInit { get; set; }

        public double SecToDecide { get; set; }

        public OpenGraphicCommand OpenGraphicCommand { get; set; }

        public DecisionGraphic FindDecidionForSingle(Analys analys)
        {
            var res =  analys.FindDecidion();
            SecToDecide = analys.SecToDecide;
            return res;
        }

        public Analys Possibility(ref string error)
        {
            HierarhyElem_Analys.Analys();
            Analys analys = new Analys();
            var result = analys.Prepare();
            error = result.Value;
            if(result.Key)
                return analys;
            return null;
        }
        /*
        public DecidionGraphic FindDecidionForCommon(List<Session> sessions)
        {
            Analys.StartRisk = 0.7;
            CommonAnalys commonAnalys = new CommonAnalys(sessions);
            return commonAnalys.CreateCommonDecidionGraphic();
        }
        */
        public void ChangeCurrentDecidionScript(DataProcessLibrary.Point point)
        {
            if (point is null)
                return;
            if(alldecidions_forsingle.ContainsKey(point.X))
                CurrentScript = alldecidions_forsingle[point.X];
        }
    }
}
