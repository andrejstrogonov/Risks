using DataProcessLibrary.InitialData;
using Riscks.Commands;
using Riscks.Resources;
using Riscks.View;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Riscks.Commands;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

namespace Riscks.ViewModel
{
    public class ResourceIntake:ViewModel
    {
        private ResourceOfProductViewModel ResourceOfProductViewModel;
        public long Product_ID { get; set; }
        public string ProductName {
            get
            {
                var product = ProductDAL.GetProduct(Product_ID);
                if (product is null)
                    return "";
                return product.Name;
            }
        }

        private long resource_id;

        public void reload_amount()
        {
            var rops = ResourceOfProductDAL.GetResourceOfProduct(Product_ID, resource_id);
            amount = 0;
            foreach (var rop in rops)
            {
                if (!(rop.AmountOfResource is null || rop.UnitOfResourceID is null))
                    amount += UnitDAL.TranslateToCommonUnit((long)rop.UnitOfResourceID, (double)rop.AmountOfResource);
            }
            if (amount > 0)
            {
                long unit_id = (long)rops.Where(rop => !(rop.AmountOfResource is null)).First().UnitOfResourceID;
                Unit unit = UnitDAL.GetUnit(unit_id);
                amount = UnitDAL.TranslateFromCommonUnit(TransferOfUnitsDAL.GetCommonUnitForType(unit.UnitSequence_ID).ID, amount);
            }
            OnPropertyChanged(nameof(Amount));
            OnPropertyChanged(nameof(ProductName));
        }
        private double amount;
        public double Amount {
            get
            {
                return amount;
            }
        }
        public ResourceIntake(long prod_id, long res_id) {
            //ResourceOfProductViewModel = CommonViewModel.IndexProductViewModel.Products.Where(p => p.ID == prod_id).First().IndexResourceOfProductViewModel.ResourcesOfProduct.Where(r => r.ID_Resource == res_id).First();
            //ResourceOfProductViewModel.PropertyChanged += ResourceOfProductViewModel_PropertyChanged;
            Product_ID = prod_id;
            resource_id = res_id;
            reload_amount();
        }

        private void ResourceOfProductViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            reload_amount();
        }
    }

    public class ResourceViewModel : ViewModel
    {
        private Dispatcher Dispatcher;
        private ResourceNullable resource;

        public long Resource_ID
        {
            get
            {
                return resource.ID;
            }
        }

        public List<double> Taxes { get; protected set; }

        public ObservableCollection<ResourceIntake> Using { get; set; }

        public string DiagramHeader
        {
            get
            {
                return Name +", "+ TransferOfUnitsDAL.GetCommonUnitForUnit((long)resource.UnitOfStockID).Symbol;
            }
        }

        public class TripleDiagramItem
        {
            private int num;
            public string Name { get; set; }
            public Prop Value { get; set; }
            public ResourceViewModel ResourceViewModel { get; private set; }
            public TripleDiagramItem(ResourceViewModel resourceViewModel, int num)
            {
                this.num = num;
                ResourceViewModel = resourceViewModel;
                resourceViewModel.PropertyChanged += ResourceViewModel_PropertyChanged;
                Reload();
            }

            private void ResourceViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if(e.PropertyName=="Maximum" || e.PropertyName=="Stock" || e.PropertyName==nameof(ResourceViewModel.UnitOfMaximumSymbol))
                    Reload();
            }

            public void Reload()
            {
                switch (num)
                {
                    case 1:
                        {
                            Name = " ";
                            Value = new Prop();
                            Value.First = ResourceViewModel.Stock is null? 0: (double)ResourceViewModel.Stock;
                            Value.Second = 0;
                            break;
                        }
                    case 2:
                        {
                            Name = "  ";
                            Value = new Prop();
                            Value.First = 0;

                            var max_ = (ResourceViewModel.Maximum is null ? 0 : (double)ResourceViewModel.Maximum);
                            
                            var stock_ = ResourceViewModel.Stock is null ? 0 : (double)ResourceViewModel.Stock;
                            if (ResourceViewModel.Maximum == double.PositiveInfinity)
                            {
                                if (stock_ == 0)
                                {
                                    max_ = 100;
                                }
                                else
                                {
                                    max_ = stock_ * 2;
                                }
                            }
                            Value.Second = max_<stock_?max_:max_-stock_;
                            break;
                        }
                    case 3:
                        {
                            Name = "Доступный"+'\n'+"максимум";
                            Value = new Prop();
                            var max_ = (ResourceViewModel.Maximum is null ? 0 : (double)ResourceViewModel.Maximum);
                            var stock_ = ResourceViewModel.Stock is null ? 0 : (double)ResourceViewModel.Stock;
                            if (ResourceViewModel.Maximum == double.PositiveInfinity)
                            {
                                if (stock_ == 0)
                                {
                                    max_ = 100;
                                }
                                else
                                {
                                    max_ = stock_ * 2;
                                }
                            }
                            Value.First = Math.Min(max_,stock_);
                            Value.Second =Math.Max(0,max_-stock_);
                            break;
                        }
                }
            }
        }
        
        public List<TripleDiagramItem> TripleDiagramm { get; set; }

        public List<string> unitsNotMoneyMaximum { get; set; }
        //public List<string> unitsNotMoneyStock { get; set; }
        public List<string> unitsMoney { get; set; }

        private Dictionary<string, Unit> FromSymbolToUnit;

        public double PriceInCommonUnit
        {
            get
            {
                return UnitDAL.TranslateToCommonUnit((long)resource.UnitOfPriceID, resource.Price.HasValue ? resource.Price.Value:0);
            }
        }

        private void Init()
        {
            Taxes = new List<double>();
            Taxes.Add(0);
            Taxes.Add(10);
            Taxes.Add(20);

            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsNotMoneyMaximum = new List<string>();
            //unitsNotMoneyStock = new List<string>();
            unitsMoney = new List<string>();

            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time);
            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID != money.ID && u.UnitSequence_ID != time.ID)
                {
                    unitsNotMoneyMaximum.Add(u.Symbol);
                    //unitsNotMoneyStock.Add(u.Symbol);
                }
                else
                if(u.UnitSequence_ID==money.ID)
                {
                    unitsMoney.Add(u.Symbol);
                }
            }
            ReloadUsing();
            unitOfMaximum = UnitDAL.GetUnit(resource.UnitOfMaximumID);
            unitOfPrice = UnitDAL.GetUnit(resource.UnitOfPriceID);
        }

        public event ObjectChanged ResourceChanged;

        public ResourceViewModel(Unit defaultUnitPrice, Unit defaultUnitAmount, CommonViewModel commonViewModel):base(commonViewModel)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            this.resource = new ResourceNullable();
            resource.UnitOfPriceID = defaultUnitPrice.ID;
            resource.UnitOfMaximumID = defaultUnitAmount.ID;
            resource.UnitOfStockID = defaultUnitAmount.ID;
            unitOfMaximum = defaultUnitAmount;
            unitOfPrice = defaultUnitPrice;
            resource.Name = "";
            Save(true);
            Init();
            OnPropertyChanged(nameof(UnitOfMaximumSymbol));
            OnPropertyChanged(nameof(UnitOfPriceSymbol));

            TripleDiagramm = new List<TripleDiagramItem>();
            TripleDiagramm.Add(new TripleDiagramItem(this, 1));
            TripleDiagramm.Add(new TripleDiagramItem(this, 2));
            TripleDiagramm.Add(new TripleDiagramItem(this, 3));
            SetInfinityCommand = new SetInfinityCommand(this);
        }

        public SetInfinityCommand SetInfinityCommand { get; set; }

        public ResourceViewModel(ResourceNullable resource, CommonViewModel commonViewModel) : base(commonViewModel)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            this.resource = resource;
            Init();

            TripleDiagramm = new List<TripleDiagramItem>();
            TripleDiagramm.Add(new TripleDiagramItem(this, 1));
            TripleDiagramm.Add(new TripleDiagramItem(this, 2));
            TripleDiagramm.Add(new TripleDiagramItem(this, 3));
            SetInfinityCommand = new SetInfinityCommand(this);
        }

        public string Name {
            get
            {
                return resource.Name;
            }
            set
            {
                var last = resource.Name;
                resource.Name = value;
                var next = resource.Name;
                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.Name), last, next,
                    last +'\n'+"Наименование", last + '\n'  + next);
                Save(true);
                OnPropertyChanged(nameof(Name));
            }
        }

        private Unit unitOfMaximum;
        public string UnitOfMaximumSymbol
        {
            get
            {
                if (unitOfMaximum is null)
                    return null;
                return unitOfMaximum.Symbol;
            }
            set
            {
                if (FromSymbolToUnit.ContainsKey(value))
                {
                    var last = resource.UnitOfMaximumID;
                    var lastval = UnitOfMaximumSymbol;
                    resource.UnitOfMaximumID = FromSymbolToUnit[value].ID;
                    resource.UnitOfStockID = resource.UnitOfMaximumID;
                    unitOfMaximum = UnitDAL.GetUnit(resource.UnitOfMaximumID);
                    var next = resource.UnitOfMaximumID;
                    var nextval = UnitOfMaximumSymbol;

                    Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.UnitOfMaximumID), last, next,
                    resource.Name + '\n' + "Е.И. количества" , lastval + '\n' + nextval);
                    Save(true);
                    OnPropertyChanged("UnitOfMaximumSymbol");
                }
            }
        }

        /*public string UnitOfStockSymbol
        {
            get
            {
                return UnitDAL.GetUnit(resource.UnitOfStockID).Symbol;
            }
            set
            {
                if (FromSymbolToUnit.ContainsKey(value))
                {
                    resource.UnitOfStockID = FromSymbolToUnit[value].ID;
                    OnPropertyChanged("UnitOfStockSymbol");
                    Save();
                }
            }
        }*/
        private Unit unitOfPrice;
        public string UnitOfPriceSymbol
        {
            get
            {
                if (unitOfPrice is null)
                    return null;
                return unitOfPrice.Symbol;
            }
            set
            {
                if (FromSymbolToUnit.ContainsKey(value))
                {
                    var last = resource.UnitOfPriceID;
                    var lastval = UnitOfPriceSymbol;

                    resource.UnitOfPriceID = FromSymbolToUnit[value].ID;
                    unitOfPrice = UnitDAL.GetUnit(resource.UnitOfPriceID);

                    var next = resource.UnitOfPriceID;
                    var nextval = UnitOfPriceSymbol;                    

                    Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.UnitOfPriceID), last, next,
                     resource.Name +'\n'+ "Е.И. стоимости", lastval + '\n'  + nextval);
                    Save(true);
                    OnPropertyChanged(nameof(UnitOfPriceSymbol));
                }
            }
        }

        public void SetMaximumInfinity()
        {
            Maximum = double.PositiveInfinity;
        }

        public double? Maximum {
            get { return resource.Maximum; }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = resource.Maximum;
                resource.Maximum = value;

                var next = resource.Maximum;

                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.Maximum), last, next,
                   resource.Name+'\n'+ "Максимум", last.ToString() + '\n' + next);

                Save();
                OnPropertyChanged(nameof(Maximum));
            }
        }

        public double? Price {
            get { return resource.Price; }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = resource.Price;
                resource.Price = value;
                var next = resource.Price;

                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.Price), last, next,
                    resource.Name+'\n'+ "Стоимость", last.ToString() + '\n'  + next);

                Save(true);
                OnPropertyChanged(nameof(Price));
            }
        }


        public double? Stock {
            get { return resource.Stock; }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = resource.Stock;
                resource.Stock = value;

                var next = resource.Stock;

                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.Stock), last, next,
                    resource.Name+'\n' +"Запасы", last.ToString() + '\n' + next);

                Save();
                OnPropertyChanged("Stock");
            }
        }

        public bool? IsInteger {
            get { return resource.IsInteger; }
            set
            {
                var last = resource.IsInteger;
                var lastval = BoolToStr(last);
                resource.IsInteger = value;
                var next = resource.IsInteger;
                var nextval = BoolToStr(next);

                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.IsInteger), last, next,
                    resource.Name+'\n'+"Целочисленность"  , lastval + '\n' + nextval);


                Save();

                OnPropertyChanged("IsInteger");
            }
        }

        public bool? Perishable
        {
            get
            {
                return resource.Perishable;
            }
            set
            {
                var last = resource.Perishable;
                var lastval = BoolToStr(last);

                resource.Perishable = value;

                var next = resource.Perishable;
                var nextval = BoolToStr(next);

                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.Perishable), last, next,
                    resource.Name + '\n' + "Скоропорт", lastval + '\n'  + nextval);

                Save();

                OnPropertyChanged("Perishable");
            }
        }

        public double? InputTax {
            get {
                return resource.InputTax*100;
            }
            set
            {
                var last = resource.InputTax;
                var lastval = last*100 + "%";

                resource.InputTax = Math.Round( (double)value/100,1);

                var next = resource.InputTax;
                var nextval = next*100 +"%";

                Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, nameof(resource.InputTax), last, next,
                    resource.Name+'\n'+"Налог", lastval + '\n' + nextval);

                Save();

                OnPropertyChanged("InputTax");
            }
        }//входной ндс

        public bool Equals(ResourceViewModel rvm)
        {
            return resource.ID == rvm.resource.ID;
        }

        public void Delete()
        {
            
            CommonViewModel.IndexProductViewModel.OnProductsChanged -= ReloadUsing;
            long id = resource.ID;
            ResourceDAL.DeleteResource(resource.ID);
            Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, resource.ID, "", resource.ID, null,
                    "Ресурс" + '\n' + resource.Name, "Удален");
            Thread thread = new Thread(() =>
            {
                if (!(ResourceChanged is null))
                ResourceChanged(id);
            });
            thread.Start();
        }

        public void Save(bool needreload=false)
        {
            ResourceDAL.SetResource(resource);
            Thread thread = new Thread(() =>
              {
                  if (needreload && !(resource is null) && !(ResourceChanged is null))
                      ResourceChanged(resource.ID);
                  //MessageBox.Show("Thread finished");
              });
            thread.Start();
        }

        public void ReloadUnits()
        {
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsNotMoneyMaximum = new List<string>();
            //unitsNotMoneyStock = new List<string>();
            unitsMoney = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID != money.ID)
                {
                    unitsNotMoneyMaximum.Add(u.Symbol);
                    //unitsNotMoneyStock.Add(u.Symbol);
                }
                else
                {
                    unitsMoney.Add(u.Symbol);
                }
            }
           // OnPropertyChanged("unitsNotMoneyStock");
            OnPropertyChanged("unitsNotMoneyMaximum");
            OnPropertyChanged("unitsMoney");
        }
        
        public double MaxNumberInUsing { get; private set; }

        public void ReloadUsing()
        {
            if(Using is null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Using = new ObservableCollection<ResourceIntake>();
                }));
                
            }

            for(int i = 0; i < Using.Count(); i++)
            {
                if (ProductDAL.GetProduct(Using[i].Product_ID) is null)
                {
                    Dispatcher.Invoke(new Action(() =>
                   {
                       Using.RemoveAt(i);
                   }));
                
                    i--;
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Using[i].reload_amount();
                    }));
                    
                }
            }
            foreach(var p in ProductDAL.GetProducts())
            {
                if (!Using.Any(pr => pr.Product_ID == p.ID))
                {
                    ResourceIntake ri = new ResourceIntake(p.ID, Resource_ID);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Using.Add(ri);
                    }));
                    
                }
            }
        }

    }
}
