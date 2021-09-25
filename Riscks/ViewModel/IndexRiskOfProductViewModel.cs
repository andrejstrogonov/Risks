using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using Riscks.Commands;
using Riscks.View;
using System.Globalization;

namespace Riscks.ViewModel
{
    public class RiskOfProductForDiagram:ViewModel
    {
        public RiskOfProductViewModel rop { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public Prop Prop { get; set; }

        public double Value { get; set; }
        
        public RiskOfProductForDiagram(RiskOfProductViewModel rop)
        {
            this.rop = rop;
            Prop = new Prop();
            Value = 0;
            rop.PropertyChanged += Rop_PropertyChanged;
            Reload();
        }


        private void Rop_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Reload();
            if (!(OnChanged is null))
                OnChanged();
        }

        public delegate void Changed();
        public event Changed OnChanged;

        public void Reload()
        {
            Prop = new Prop()
            {
                First = UnitDAL.TranslateToCommonUnit((long)rop.riskOfProduct.UnitOfAmountID, (rop.GarantedAmount.HasValue ? rop.GarantedAmount.Value : 0)),
                Second = UnitDAL.TranslateToCommonUnit((long)rop.riskOfProduct.UnitOfAmountID, (rop.MaximumAmount.HasValue ? rop.MaximumAmount.Value : 0))
            };
            Value = Math.Max(Prop.First, Prop.Second);
            OnPropertyChanged(nameof(Value));
            //OnPropertyChanged(nameof(Value));
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            Name = UnitDAL.TranslateToCommonUnit((long)rop.riskOfProduct.UnitOfPriceID, (double)(rop.Price.HasValue ? rop.Price.Value : 0)).ToString("N", CultureInfo.CurrentCulture) + " " + TransferOfUnitsDAL.GetCommonUnitForType(money.ID).Symbol;
            //OnPropertyChanged(nameof(Name));
            ID = rop.riskOfProduct.ID;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(Name));
        }
    }
    public class IndexRiskOfProductViewModel : ViewModel
    {
        private ProductNullable product;
        public ObservableCollection<RiskOfProductViewModel> RisksOfProduct { get; set; }

        public string DiagramHeader
        {
            get
            {
                return product.Name + ", " + TransferOfUnitsDAL.GetCommonUnitForUnit(product.UnitOfStockID.Value).Symbol;
            }
        }
        public List<RiskOfProductForDiagram> DiagramProps{ get; set; }
        public double DiagramMin { get; set; }
        public double DiagramMax { get; set; }


        private RiskOfProductViewModel currentRiskOfProduct;
        public RiskOfProductViewModel CurrentRiskOfProduct {
            get
            {
                return currentRiskOfProduct;
            }
            set
            {
                currentRiskOfProduct = value;
                OnPropertyChanged("CurrentRiskOfProduct");
                //if (!(value is null))
                //  currentRiskOfProduct.Save();

            }
        }

  
        public IndexRiskOfProductViewModel(ProductNullable product, CommonViewModel commonViewModel):base(commonViewModel)
        {
            this.product = product;
            ReloadRisks();
            AddRisk = new AddRiskCommand(this);
            DeleteRisk = new DeleteRiskCommand(this);
        }


        private void RecountMax()
        {
            DiagramMax = 0;
            foreach(var d in DiagramProps)
            {
                if (d.Value > DiagramMax)
                {
                    DiagramMax = d.Value;
                }
            }
            DiagramMax = DiagramMax * 1.02;
            OnPropertyChanged(nameof(DiagramMax));
        }
 
        

        public void AddRiskOfProduct()
        {
            RiskOfProductViewModel rop = new RiskOfProductViewModel(product,CommonViewModel);
            currentRiskOfProduct = rop;
            currentRiskOfProduct.Save();
            RisksOfProduct.Add(currentRiskOfProduct);            


            OnPropertyChanged("RisksOfProduct");
            OnPropertyChanged("CurrentRiskOfProduct");

            var diagramROP = new RiskOfProductForDiagram(currentRiskOfProduct);
            diagramROP.OnChanged += RecountMax;
           // diagramROP.PropertyChanged += DiagramROP_PropertyChanged;
            DiagramProps.Add(diagramROP);
            RecountMax();
            OnPropertyChanged(nameof(DiagramProps));
        }

        public void DeleteCurrentRisk()
        {
            if (!(CurrentRiskOfProduct is null))
            {
                DiagramProps.Remove(DiagramProps.First(p => p.ID == currentRiskOfProduct.riskOfProduct.ID));
                RecountMax();
                OnPropertyChanged(nameof(DiagramProps));
                currentRiskOfProduct.Delete();
                RisksOfProduct.Remove(currentRiskOfProduct);
                OnPropertyChanged("RisksOfProduct");
                OnPropertyChanged("CurrentRiskOfProduct");
            }
        }

        public AddRiskCommand AddRisk { get; protected set; }
        public DeleteRiskCommand DeleteRisk { get; protected set; }
        public void ReloadUnits(long unitSequence_id)
        {
            foreach (var r in RisksOfProduct)
                r.ReloadUnits(unitSequence_id);
        }
   
        public void ReloadRisks()
        {
            RisksOfProduct = new ObservableCollection<RiskOfProductViewModel>();
            DiagramProps = new List<RiskOfProductForDiagram>();
            foreach (var rop in RiskOfProductDAL.GetPricesOfProduct(product))
            {
                var ropvm = new RiskOfProductViewModel(rop,CommonViewModel);
                RisksOfProduct.Add(ropvm);
                var diagramROP = new RiskOfProductForDiagram(ropvm);
                diagramROP.OnChanged += RecountMax;
                DiagramProps.Add(diagramROP);
            }
            RecountMax();
            //OnPropertyChanged(nameof(DiagramProps));
            //OnPropertyChanged(nameof(RisksOfProduct));

            if (RisksOfProduct.Count > 0)
                currentRiskOfProduct = RisksOfProduct.First();
            OnPropertyChanged(nameof(CurrentRiskOfProduct));
        }
    }
}
