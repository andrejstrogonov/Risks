using DataProcessLibrary.CommonAnalys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagerLibrary;
using DataProcessLibrary.Analys;
using SQLiteLibrary.DataAccess;

namespace Riscks.ViewModel
{
    /*public class IndexCommonDecidionViewModel:ViewModel
    {
        private CommonScript CommonScripx;
        public double Risk { get; private set; }
        public double Profit { get; private set; }
        public double Profit_Bad { get; private set; }
        public double Profit_Perish { get; private set; }
        public double Delta_VAT { get; private set; }
        public double BudgetCost { get; private set; }
        public double Profit_Before { get; private set; }
        public double Expenses { get; private set; }
        public double Proceeds { get; private set; }

        public string Risk_u { get { return Risk.ToString() + "%"; } private set { } }
        private string UnitSymbol = TransferOfUnitsDAL.GetCommonUnitForType_Smallest(SQLiteLibrary.Model.TypeOfUnit.Money).Symbol;
        public string Profit_u { get { return Profit.ToString() + " " + UnitSymbol; } private set { } }
        public string Profit_Bad_u { get { return Profit_Bad.ToString() + " " + UnitSymbol; } private set { } }
        public string Profit_Perish_u { get { return Profit_Perish.ToString() + " " + UnitSymbol; } private set { } }
        public string Delta_VAT_u { get { return Delta_VAT.ToString() + " " + UnitSymbol; } private set { } }
        public string BudgetCost_u { get { return BudgetCost.ToString() + " " + UnitSymbol; } private set { } }
        public string Profit_Before_u { get { return Profit_Before.ToString() + " " + UnitSymbol; } private set { } }
        public string Expenses_u { get { return Expenses.ToString() + " " + UnitSymbol; } private set { } }
        public string Proceeds_u { get { return Proceeds.ToString() + " " + UnitSymbol; } private set { } }

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

        private void InitProfitDiagramm(string name)
        {
            ProfitDiagramm = new Diagram(name);

            Proceeds = 0;
            foreach (var p in Products)
            {
                Proceeds += UnitDAL.TranslateToCommonUnit(p.riskOfProduct.UnitOfPriceID, p.Amount * p.Price);
            }

            ProfitDiagramm.diagramm.Add(new DiagramItem("1", Proceeds));

            double summ1 = 0;
            foreach (var d in FixedCostDAL.GetFixedCosts(SQLiteLibrary.Model.TypeOfFixedCosts.incoming, false))
            {
                summ1 += UnitDAL.TranslateToCommonUnit((long)d.UnitOfPriceID, d.Price.HasValue ? d.Price.Value : 0);
            }
            ProfitDiagramm.diagramm.Add(new DiagramItem("2", summ1));

            Expenses = 0;
            foreach (var r in Resources)
            {
                Expenses += UnitDAL.TranslateToCommonUnit(r.resourceScript.Resource.UnitOfPriceID, r.resourceScript.AmountToBuy * r.resourceScript.Resource.Price);
            }
            ProfitDiagramm.diagramm.Add(new DiagramItem("3", Expenses));

            double summ3 = 0;
            foreach (var d in FixedCostDAL.GetFixedCosts(SQLiteLibrary.Model.TypeOfFixedCosts.outcoming))
            {
                summ3 += UnitDAL.TranslateToCommonUnit((long)d.UnitOfPriceID, d.Price.HasValue ? d.Price.Value : 0);
            }
            ProfitDiagramm.diagramm.Add(new DiagramItem("4", summ3));
            ProfitDiagramm.diagramm.Add(new DiagramItem("5", Delta_VAT));
            ProfitDiagramm.diagramm.Add(new DiagramItem("6", Proceeds + summ1 - Expenses - summ3 - Delta_VAT));
            Profit_Before = Proceeds + summ1 - Expenses - summ3;

        }

        private void InitProceedsDiagramm(string name)
        {
            ProceedsDiagramm = new Diagram(name);
            foreach (var p in Products)
            {
                ProceedsDiagramm.diagramm.Add(new DiagramItem(p.ProductName, UnitDAL.TranslateToCommonUnit(p.riskOfProduct.UnitOfPriceID, p.Amount * p.Price)));
            }
        }

        private void InitExpensesDiagramm(string name)
        {
            ExpensesDiagramm = new Diagram(name);
            foreach (var r in Resources)
            {
                ExpensesDiagramm.diagramm.Add(new DiagramItem(r.ResourceName, UnitDAL.TranslateToCommonUnit(r.resourceScript.Resource.UnitOfPriceID, r.resourceScript.AmountToBuy * r.resourceScript.Resource.Price)));
            }
        }



        private Dictionary<string, DecidionScriptViewModel> IndexDecidions;
        
        private DecidionScriptViewModel currentDecidion;
        public DecidionScriptViewModel CurrentDecidion
        {
            get
            {
                return currentDecidion;
            }
            set
            {
                currentDecidion = value;
                OnPropertyChanged("CurrentDecidion");
            }
        }


        public IndexCommonDecidionViewModel(CommonScript commonScript,double Risk)
        {
            CommonScript = commonScripx;
            this.Risk = Risk;
            Profit = commonScript.ProfitIfBest;
            Profit_Bad = commonScript.ProfitIfBad;
            Profit_Perish = commonScript.ProfitWithPerishable;
            Delta_VAT = commonScript.delta_VAT;
            IndexDecidions = new Dictionary<string, DecidionScriptViewModel>();

            foreach (var ds in commonScript.Decidions)
            {
                var name = ds.Key.Name;
                if(ds.Value is CommonScript)
                    IndexDecidions[name] = new DecidionScriptViewModel(ds.Value as CommonScript, Risk);
                else
                {
                    IndexDecidions[name] = new DecidionScriptViewModel(ds.Value as SingleDecidionScript, Risk);
                }
            }
            BudgetCost = commonScript.BudgetCost;
            InitDiagramms("Сценарий для " + Risk.ToString() + "%");
        }

    }
*/}
