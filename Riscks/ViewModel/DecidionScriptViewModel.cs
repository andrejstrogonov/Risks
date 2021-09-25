using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataProcessLibrary;
using DataProcessLibrary.CommonAnalys;
using DataProcessLibrary.Analys;
using SQLiteLibrary.DataAccess;
using System.Globalization;
using SQLiteLibrary.Model;

namespace Riscks.ViewModel
{
    public abstract class DecidionScriptViewModel
    {
        public ObservableCollection<ResourceScriptViewModel> Resources { get; set; }
        public class TableItem
        {
            public string Name { get; set; }
            public double Risk { get; set; }
            public double Amount { get; set; }
            public double Value_1 { get; set; }
            public double Value_2 { get; set; }            
        }

        public List<TableItem> MainTableItems { get; set; }

        public bool IsLeftSide { get; set; }

        public double Risk { get; private set; }
        public double Profit { get; private set; }
        public double Profit_Bad { get; private set; }
        public double Profit_Perish { get; private set; }
        public double Delta_VAT { get; private set; }
        public double BudgetCost { get; private set; }
        public double Profit_Before { get; protected set; }
        public double Expenses { get; protected set; }
        public double Proceeds { get; protected set; }
        public double FixedCosts_In { get; protected set; }
        public double FixedCosts_Out { get; protected set; }

        public string Risk_u { get { return Risk.ToString() + "%"; } private set { } }
        private string UnitSymbol = TransferOfUnitsDAL.GetCommonUnitForType(UnitSequenceDAL.GetUnitSequence(SQLiteLibrary.Model.TypeOfUnit.Money).ID).Symbol;
        public string Profit_u { get { return Profit.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string Profit_Bad_u { get { return Profit_Bad.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string Profit_Perish_u { get { return Profit_Perish.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string Delta_VAT_u { get { return Delta_VAT.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string BudgetCost_u { get { return BudgetCost.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string Profit_Before_u { get { return Profit_Before.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string Expenses_u { get { return Expenses.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }
        public string Proceeds_u { get { return Proceeds.ToString("N", CultureInfo.CurrentCulture) + " " + UnitSymbol; } private set { } }

        public double UntappedBugdet { get; private set; }

        public class DiagramItem
        {
            public string Name { get; set; }
            public double Value { get; set; }
            public DiagramItem(string name, double value)
            {
                Name = name;
                Value = value;
            }
        }

        public class Diagram
        {
            public string Name { get; set; }
            public string AxisHeader
            {
                get
                {
                    return "Сумма, " + TransferOfUnitsDAL.GetCommonUnitForType(TypeOfUnit.Money).Symbol;
                }
            }
            public List<DiagramItem> diagramm { get; set; }
            public Diagram(string name)
            {
                Name = name;
                diagramm = new List<DiagramItem>();
                
            }
        }

        public Diagram ProceedsDiagramm { get; set; }
        public Diagram ExpensesDiagramm { get; set; }
        public Diagram ProfitDiagramm { get; set; }

        public void InitDiagramms(string name)
        {
            InitProfitDiagramm(name);
            InitProceedsDiagramm(name);
            InitExpensesDiagramm(name);            
        }
        public void InitProfitDiagramm(string name)
        {
            ProfitDiagramm = new Diagram(name);
            ProfitDiagramm.diagramm.Add(new DiagramItem("1", Proceeds));
            ProfitDiagramm.diagramm.Add(new DiagramItem("2", FixedCosts_In));
            ProfitDiagramm.diagramm.Add(new DiagramItem("3", Expenses));
            ProfitDiagramm.diagramm.Add(new DiagramItem("4", FixedCosts_Out));
            ProfitDiagramm.diagramm.Add(new DiagramItem("5", Delta_VAT));
            ProfitDiagramm.diagramm.Add(new DiagramItem("6", Profit));
            Profit_Before = Profit+Delta_VAT;

        }

        public DecidionScriptViewModel(DecidionScript ds, double Risk, bool isLeft=false)
        {
            IsLeftSide = isLeft;
            this.Risk = Risk;
            Profit = ds.ProfitIfBest;
            Profit_Bad = ds.ProfitIfBad;
            Profit_Perish = ds.ProfitWithPerishable;
            Delta_VAT = ds.delta_VAT;
            BudgetCost = ds.BudgetCost;
            Proceeds = ds.Proceeds;
            Expenses = ds.Expenses;
            FixedCosts_In = ds.FixedCosts_Incoming;
            FixedCosts_Out = ds.FixedCosts_Outcomung;
            Resources = new ObservableCollection<ResourceScriptViewModel>();
            foreach (var rs in ds.ResourcesInUse)
            {
                Resources.Add(new ResourceScriptViewModel(rs.Value));
            }
        }

        public abstract void InitProceedsDiagramm(string name);
        public abstract void InitExpensesDiagramm(string name);
    }

    public class SingleDecidionScriptViewModel:DecidionScriptViewModel
    {
        public SingleDecidionScriptViewModel(SingleDecidionScript ds, double Risk) : base(ds, Risk)
        {
            MainTableItems = new List<TableItem>();
            foreach (var sc in ds.dataScript.PricesOfProducts)
            {
                var rop = new RiskOfProductScriptViewModel(ds.decidion, sc);
                MainTableItems.Add(new TableItem()
                {
                    Name = rop.ProductName,
                    Risk = rop.Risk,
                    Amount = rop.Amount,
                    Value_1=rop.Price,
                    Value_2=rop.Stock

                });
            }
            InitDiagramms("Сценарий для " + Risk.ToString() + "%");
        }
        public override void InitProceedsDiagramm(string name)
        {
            ProceedsDiagramm = new Diagram(name);
            foreach (var p in MainTableItems)
            {
                ProceedsDiagramm.diagramm.Add(new DiagramItem(p.Name, p.Amount * p.Value_1));
            }
        }
        public override void InitExpensesDiagramm(string name)
        {
            ExpensesDiagramm = new Diagram(name);
            foreach (var r in Resources)
            {
                ExpensesDiagramm.diagramm.Add(new DiagramItem(r.ResourceName, UnitDAL.TranslateToCommonUnit(r.resourceScript.Resource.UnitOfPriceID, r.resourceScript.AmountToBuy * r.resourceScript.Resource.Price)));
            }
        }

    }

    public class CommonDecidionScriptViewModel : DecidionScriptViewModel
    {
        public List<ProductScript> Products { get; set; }
        public CommonDecidionScriptViewModel(CommonScript ds, double Risk) : base(ds, Risk)
        {
            MainTableItems = new List<TableItem>();
            foreach(var d in ds.Decidions)
            {
                TableItem ti = new TableItem();
                ti.Name = d.Key.Name;
                ti.Risk = d.Value.RealRisk;
                ti.Amount = d.Key.Number;
                ti.Value_1 = d.Value.Proceeds;
                ti.Value_2 = d.Value.Expenses;
                MainTableItems.Add(ti);
            }

            Products = new List<ProductScript>();
            foreach(var ps in ds.ProductsProceeds)
            {
                Products.Add(ps.Value);
            }

            InitDiagramms("Сценарий для " + Risk.ToString() + "%");
        }

        public override void InitProceedsDiagramm(string name)
        {
            ProceedsDiagramm = new Diagram(name);
            foreach (var p in MainTableItems)
            {
                ProceedsDiagramm.diagramm.Add(new DiagramItem(p.Name, p.Value_1));
            }
        }

        public override void InitExpensesDiagramm(string name)
        {
            ExpensesDiagramm = new Diagram(name);
            foreach (var r in MainTableItems)
            {
                ExpensesDiagramm.diagramm.Add(new DiagramItem(r.Name, r.Value_2));
            }
        }

    }

}
