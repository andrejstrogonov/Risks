using DataProcessLibrary;
using DataProcessLibrary.Analys;
using DataProcessLibrary.Flexibility;
using Riscks.Commands;
using SQLiteLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static Riscks.ViewModel.DecidionScriptViewModel;
using ClientServerLibrary;
using License = ClientServerLibrary.License;
using Riscks.View;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using Point = DataProcessLibrary.Point;
using SQLiteLibrary.Model;
using SQLiteLibrary;

namespace Riscks.ViewModel
{
    public class ProductChangeScript
    {
        public string Name { get; set; }
        private double amount;
        private double proceed;
        private double exceed;
        public double profit { get; set; }

        public double Profit_Before { get; set; }
        public double Profit_After { get; set; }

        public string Amount_u
        {
            get
            {
                return amount + "%";
            }
        }
        public string Proceed_u
        {
            get
            {
                return proceed + "%";
            }
        }
        public string Exceed_u
        {
            get
            {
                return exceed + "%";
            }
        }
        public string Profit_u
        {
            get
            {
                return profit + "%";
            }
        }



        public ProductChangeScript(string name, double al, double an, double pl, double pn, double el, double en)
        {
            Name = name;
            amount = Math.Round(((an - al) / al) * 100,2);
            proceed = Math.Round(((pn - pl) / pl) * 100,2);
            exceed = Math.Round(((en - el) / el) * 100,2);
            Profit_Before = pl - el;
            Profit_After = pn - en;
            profit = Math.Round((((Profit_After) - (Profit_Before)) / (Profit_Before)) * 100,2);
        }
    }

    public class FlexibilityScript : ViewModel
    {        
        public List<ProductChangeScript> ProductScripts { get; set; }

        public double profit_before { get; private set; }
        public double profit_after { get; private set; }
        public double profit_afterperiod { get; private set; }
        public double priceofchange { get; private set; }
        public double profitably { get; private set; }
        public double risk{ get; private set; }
        private string unit_symbol;

        public Diagram ProfitDiagramm { get; private set; }
        public Diagram ProductProfitDiagramm_Before { get; private set; }
        public Diagram ProductProfitDiagramm_After { get; private set; }

        public string Profit_Before_u
        {
            get
            {
                return profit_before.ToString("N",CultureInfo.CurrentCulture) + " " + unit_symbol;
            }
        }
        public string Profit_After_u
        {
            get
            {
                return profit_after.ToString("N", CultureInfo.CurrentCulture) + " " + unit_symbol;
            }
        }
        public string Profit_Afterperiod_u
        {
            get
            {
                return profit_afterperiod.ToString("N", CultureInfo.CurrentCulture) + " " + unit_symbol;
            }
        }
        public string PriceOfChange_u
        {
            get
            {
                return priceofchange.ToString("N", CultureInfo.CurrentCulture) + " " + unit_symbol;
            }
        }
        public string Risk_u
        {
            get
            {
                return risk.ToString() + "%";
            }
        }

        public FlexibilityScript(SingleDecidionScript before, SingleDecidionScript after, double priceofchange, ParametersList parametersList)
        {
            risk = before.RealRisk;
            profit_before = before.ProfitIfBest;
            profit_after = after.ProfitIfBest - priceofchange;
            this.priceofchange = priceofchange;
            profit_afterperiod = after.ProfitIfBest;
            if (priceofchange == 0)
            {
                priceofchange = 1;
            }
            profitably = (profit_afterperiod - profit_before) / priceofchange;
            var money = UnitSequenceDAL.GetUnitSequence(SQLiteLibrary.Model.TypeOfUnit.Money);
            unit_symbol = TransferOfUnitsDAL.GetCommonUnitForType(money.ID).Symbol;
            ProductScripts = new List<ProductChangeScript>();
            foreach(var b in before.ProductsProceeds)
            {
                double al = b.Value.Amount;
                double pl = b.Value.Proceeds;
                double el = before.AnalysData.AVCOfProducts[before.AnalysData.AVCOfProducts.Keys.First(p=>p.Name==b.Key)];
                double an = after.ProductsProceeds.First(p => p.Value.Name == b.Value.Name).Value.Amount;
                double pn = after.ProductsProceeds.First(p => p.Value.Name == b.Value.Name).Value.Proceeds;
                double en = after.AnalysData.AVCOfProducts[after.AnalysData.AVCOfProducts.Keys.First(p => p.Name == b.Key)];
                ProductChangeScript changeScript = new ProductChangeScript(b.Value.Name, al, an, pl, pn, el, en);
                ProductScripts.Add(changeScript);
            }

            ProfitDiagramm = new Diagram(risk.ToString() + "%");
            ProfitDiagramm.diagramm.Add(new DiagramItem("1",profit_before));
            ProfitDiagramm.diagramm.Add(new DiagramItem("2", profit_after));
            ProfitDiagramm.diagramm.Add(new DiagramItem("3", profit_afterperiod));

            ProductProfitDiagramm_Before = new Diagram(risk.ToString() + "%");
            ProductProfitDiagramm_After = new Diagram(risk.ToString() + "%");
            foreach (var p in ProductScripts)
            {
                ProductProfitDiagramm_Before.diagramm.Add(new DiagramItem(p.Name, p.Profit_Before));
                ProductProfitDiagramm_After.diagramm.Add(new DiagramItem(p.Name, p.Profit_After));
            }
        }

    }

    public class FlexibilityGraphic : ViewModel
    {
        public GraphicViewModel before_graphic { get; set; }
        public GraphicViewModel after_graphic { get; set; }
        public GraphicViewModel afterperiod_graphic { get; set; }

        private double priceofchange;
        public string AxisHeader
        {
            get
            {
                return "Прибыль, " + TransferOfUnitsDAL.GetCommonUnitForType(TypeOfUnit.Money).Symbol;
            }
        }

        public double StartRisk { get; set; }
        public double FinishRisk { get; set; }
        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public double Interval { get; set; }

        public Dictionary<double, FlexibilityScript> scripts { get; set; }

        private FlexibilityScript currentScript;
        public FlexibilityScript CurrentScript
        {
            get
            {
                return currentScript;
            }
            set
            {
                currentScript = value;
                OnPropertyChanged(nameof(CurrentScript));
            }
        }

        public FlexibilityGraphic(DecisionGraphic before, DecisionGraphic after, double priceofchange, ParametersList parametersList)
        {
            this.priceofchange = priceofchange;
            before_graphic = new GraphicViewModel(before.best_points.ToList());
            afterperiod_graphic = new GraphicViewModel(after.best_points.ToList());
            after_graphic = new GraphicViewModel(after.best_points.Select(p=>new Point(p.X,p.Y-priceofchange)).ToList());
            scripts = new Dictionary<double, FlexibilityScript>();
            foreach(var b in before.decidionGraphic)
            {
                var afterscript = after.decidionGraphic.First(p => p.Key == b.Key);
                scripts[b.Key] = new FlexibilityScript(b.Value as SingleDecidionScript, afterscript.Value as SingleDecidionScript, priceofchange, parametersList);
            }
            if (scripts.Count > 0)
            {
                CurrentScript = scripts.First().Value;
            }
            before_graphic.OnChanged += ChangeCurrentDecidionScript;
            before_graphic.OnChanged += after_graphic.ChangePoint;
            before_graphic.OnChanged += afterperiod_graphic.ChangePoint;

            after_graphic.OnChanged += ChangeCurrentDecidionScript;
            after_graphic.OnChanged += before_graphic.ChangePoint;
            after_graphic.OnChanged += afterperiod_graphic.ChangePoint;

            afterperiod_graphic.OnChanged += ChangeCurrentDecidionScript;
            afterperiod_graphic.OnChanged += after_graphic.ChangePoint;
            afterperiod_graphic.OnChanged += before_graphic.ChangePoint;

            StartRisk = scripts.Min(p => p.Key) - 0.2;
            FinishRisk = scripts.Max(p => p.Key) + 0.2;

        }

        public void ChangeCurrentDecidionScript(DataProcessLibrary.Point point)
        {
            if (point is null)
                return;
            if (scripts.ContainsKey(point.X))
                CurrentScript = scripts[point.X];
        }

    }

    public class FlexibilityAnalysViewModel:ViewModel
    {
        private FlexibilityAnalys flexibilityAnalys;

        private CollectionViewSource flexibilityCollectionView;

        public CollectionViewSource FlexibilityCollectionView {
            get
            {
                return flexibilityCollectionView;
            }
            set
            {
                flexibilityCollectionView = value;
                OnPropertyChanged("FlexibilityCollectionView");
            }
        }

        private ObservableCollection<ProfitFlexibilityViewModel> SelectedFlexibilities { get; set; }
        private CollectionViewSource selectedCollection;
        public CollectionViewSource SelectedCollection
        {
            get
            {
                return selectedCollection;
            }
            set
            {
                selectedCollection = value;
                OnPropertyChanged(nameof(SelectedCollection));
            }
        }

        private string unitSymbol;

        public FlexibilityAnalysViewModel(CommonViewModel commonViewModel):base(commonViewModel)
        {
            FlexibilityCollectionView = new CollectionViewSource()
            {
                Source = new ObservableCollection<ProfitFlexibilityViewModel>()
            };
            SelectedCollection = new CollectionViewSource()
            {
                Source = new ObservableCollection<ProfitFlexibilityViewModel>()
            };
            
            FlexibilityCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("TypeOfParameter"));
            var money = UnitSequenceDAL.GetUnitSequence(SQLiteLibrary.Model.TypeOfUnit.Money);
            unitSymbol = TransferOfUnitsDAL.GetCommonUnitForType(money.ID).Symbol;
            SumOfResults = "0 " + unitSymbol;

            AcceptChangesCommand = new AcceptChangesCommand(this);
            SaveFlexibilityCommand = new SaveFlexibilityCommand(this);
            OpenFlexibilityAnalysCommand = new OpenFlexibilityAnalysCommand(this);
            SetFlexibilityParametersCommand = new SetFlexibilityParametersCommand(this);
        }

        public OpenFlexibilityAnalysCommand OpenFlexibilityAnalysCommand { get; set; }

        public AcceptChangesCommand AcceptChangesCommand { get; set; }
        public SetFlexibilityParametersCommand SetFlexibilityParametersCommand { get; set; }

        public void Execute()
        {
            if (License.LicenseKey == "" || License.LicenseKey is null || !License.ActivateLicense(License.LicenseKey))
            {
                throw new ApplicationException("Лицензия отсутствует");
            }
            string title = "Анализ эластичности расчета ";
            title += CommonViewModel.HierarhyElem.Name;
            ProgressLineViewModel progressLineViewModel = new ProgressLineViewModel(title);
            FlexibilityAnalys.OnSetMaximum += progressLineViewModel.SetMaximum;
            FlexibilityAnalys.OnSetProgress += progressLineViewModel.IncreaseProgress;
            LoadingLineWindow loadingLineWindow = new LoadingLineWindow(progressLineViewModel);
            loadingLineWindow.Show();
            Thread thread = null;
            try
            {
                thread = new Thread(
                () => {

                    try
                    {
                        flexibilityAnalys = new FlexibilityAnalys();
                        loadingLineWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                        
                        ProfitFlexibilities = new ObservableCollection<ProfitFlexibilityViewModel>();
                        foreach (var pf in flexibilityAnalys.ProfitFlexibilities)
                        {
                            var pfvn = new ProfitFlexibilityViewModel(pf);
                            pfvn.PropertyChanged += ChangeSumOfResults;
                            ProfitFlexibilities.Add(pfvn);
                        }
                        FlexibilityCollectionView = new CollectionViewSource()
                        {
                            Source = ProfitFlexibilities
                        };
                        FlexibilityCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("TypeOfParameter"));
                        FlexibilityCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Coefficient_Flexibility", System.ComponentModel.ListSortDirection.Descending));
                        OnPropertyChanged("ProfitFlexibilities");
                        }));
                        progressLineViewModel.StopProgress();
                        loadingLineWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            loadingLineWindow.Close();
                        }));
                        FlexibilityAnalys.OnSetMaximum -= progressLineViewModel.SetMaximum;
                        FlexibilityAnalys.OnSetProgress -= progressLineViewModel.IncreaseProgress;

                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    });
                thread.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                loadingLineWindow.Close();
            }
        }

        public ObservableCollection<ProfitFlexibilityViewModel> ProfitFlexibilities { get; set; }

        public void ChangeSumOfResults(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                sumOfResults = 0;
                foreach (var pf in ProfitFlexibilities)
                    sumOfResults += pf.Result;
                SumOfResults = sumOfResults.ToString() + " " +unitSymbol;
                OnPropertyChanged(nameof(SumOfResults));
            }
        }

        private double sumOfResults;
        public string SumOfResults { get; set; }
        private ParametersList ParametersList;
        public ParametersList GetParametersList()
        {
            Dictionary<Parameter, double> parameters = new Dictionary<Parameter, double>();
            Dictionary<GroupParameter, double> groupParameters = new Dictionary<GroupParameter, double>();
            SelectedFlexibilities = new ObservableCollection<ProfitFlexibilityViewModel>();
            foreach(var p in ProfitFlexibilities)
            {
                if (p.Posibility is true)
                {
                    if (p.ProfitFlexibility.Parameter is Parameter)
                    {
                        parameters[(Parameter)p.ProfitFlexibility.Parameter] = p.NewMeaning;
                    }
                    else
                    {
                        groupParameters[(GroupParameter)p.ProfitFlexibility.Parameter] = (p.NewMeaning - p.ProfitFlexibility.LastMeaning) * p.ProfitFlexibility.Sign / p.ProfitFlexibility.LastMeaning;
                    }
                    SelectedFlexibilities.Add(p);
                }
            }
            SelectedCollection = new CollectionViewSource()
            {
                Source = SelectedFlexibilities
            };
            OnPropertyChanged(nameof(SelectedCollection));
            OnPropertyChanged(nameof(SelectedFlexibilities));
            ParametersList = new ParametersList(parameters, groupParameters);
            return ParametersList;
        }

        public ResultOfChange ResultOfChange { get; set; }


        public FlexibilityGraphic FlexibilityGraphic { get; set; }


        public delegate void SetMaximum(int maximum);
        public static SetMaximum OnSetMaximum;

        public delegate void SetCurrentProgress();
        public static SetCurrentProgress OnSetProgress;

        private bool ThreadFinished = false;
        public static MainWindowView MainWindowView { get; set; }
        public void SetChanges()
        {
            string title = "Применение выбранных параметров расчета ";
            title += CommonViewModel.HierarhyElem.Name;
            ProgressLineViewModel progressLineViewModel = new ProgressLineViewModel(title);
            progressLineViewModel.StopIsVisible = Visibility.Collapsed;
            OnSetMaximum += progressLineViewModel.SetMaximum;
            OnSetProgress += progressLineViewModel.IncreaseProgress;
            LoadingLineWindow loadingLineWindow = new LoadingLineWindow(progressLineViewModel);
            loadingLineWindow.Show();
            Thread thread = null;
            try
            {
                thread = new Thread(
                () => {
                    try
                    {

                        OnSetMaximum(2);                                                

                        var res = GetParametersList();

                
                        double SummOfSpending = 0;
                        foreach (var p in ProfitFlexibilities)
                        {
                            if (p.Posibility is true)
                            {
                                SummOfSpending += p.PriceOfChange.HasValue ? p.PriceOfChange.Value : 0;
                            }
                        }
                
                        var decidion_before = res.FindDecidion_WithoutChanges();
                        OnSetProgress();
                        var decidion_after = res.FindDecidion_WithChanges();


                        progressLineViewModel.StopProgress();

                        loadingLineWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            FlexibilityGraphic = new FlexibilityGraphic(decidion_before, decidion_after, SummOfSpending, ParametersList);

                            OnPropertyChanged(nameof(FlexibilityGraphic));
                            loadingLineWindow.Close();
                            FlexibilityIndex = 1;
                            OnPropertyChanged(nameof(FlexibilityIndex));

                        }));                        
                        OnSetMaximum -= progressLineViewModel.SetMaximum;
                        OnSetProgress -= progressLineViewModel.IncreaseProgress;

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
                thread.Start();
            /*try { 
                var res = GetParametersList();
                double SummOfSpending = 0;
                foreach (var p in ProfitFlexibilities)
                {
                    if (p.Posibility is true)
                    {
                        SummOfSpending += p.PriceOfChange.HasValue ? p.PriceOfChange.Value : 0;
                    }
                }
                var decidion_before = res.FindDecidion_WithoutChanges();
                var decidion_after = res.FindDecidion_WithChanges();

                FlexibilityGraphic = new FlexibilityGraphic(decidion_before, decidion_after, SummOfSpending, ParametersList);
                OnPropertyChanged(nameof(FlexibilityGraphic));
                FlexibilityIndex = 1;
                OnPropertyChanged(nameof(FlexibilityIndex));*/
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public int FlexibilityIndex { get; set; }
        public SaveFlexibilityCommand SaveFlexibilityCommand { get; set; }

        public void SaveChanges()
        {
            var history_items =ParametersList.SaveChanges();
            List<ProfitFlexibility> profitFlexibilities = new List<ProfitFlexibility>();
            foreach(var p in SelectedFlexibilities)
            {
                profitFlexibilities.Add(p.ProfitFlexibility);
            }
            ParametersList.SaveChanges(profitFlexibilities);
            foreach(var hi in history_items)
            {
                SaveTypeHistoryItem(hi.Key, hi.Value);
            }
            CommonViewModel.IndexProductViewModel.Reload();
            CommonViewModel.IndexResourcesViewModel.Reload();
            CommonViewModel.BothFixedCostsViewModel.Reload();
        }

        private void SaveTypeHistoryItem(TypeOfHistoryItem type, List<HistoryItem> items)
        {
            switch (type)
            {
                case TypeOfHistoryItem.Product:
                case TypeOfHistoryItem.ResourceOfProduct:
                case TypeOfHistoryItem.RiskOfProduct:
                    CommonViewModel.HistoryViewModel_Products.AddItems(items);break;
                case TypeOfHistoryItem.Resource:
                    CommonViewModel.HistoryViewModel_Resources.AddItems(items); break;         
                
            }
        }

        public void SaveHistory()
        {

        }
    }
}
