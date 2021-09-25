using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DataProcessLibrary.InitialData;

namespace Riscks.ViewModel
{
    public class ProductViewModel : ViewModel
    {
        public long ID { get; set; }
        public ProductNullable product { get; private set; }

        private IndexResourceOfProductViewModel indexResourceOfProductViewModel;
        public IndexResourceOfProductViewModel IndexResourceOfProductViewModel
        {
            get
            {
                return indexResourceOfProductViewModel;
            }
            set
            {
                indexResourceOfProductViewModel = value;
                OnPropertyChanged("IndexResourceOfProductViewModel");
            }
        }

        public void ReloadInterface()
        {
            OnPropertyChanged(nameof(IndexResourceOfProductViewModel));
            OnPropertyChanged(nameof(IndexRiskOfProductViewModel));
        }

        private IndexRiskOfProductViewModel indexRiskOfProductViewModel;

        public IndexRiskOfProductViewModel IndexRiskOfProductViewModel
        {
            get
            {
                return indexRiskOfProductViewModel;
            }
            set
            {
                indexRiskOfProductViewModel = value;
                OnPropertyChanged("IndexRiskOfProductViewModel");
            }
        }

        public List<string> unitsNotMoneyMinimum { get; set; }
        public List<string> unitsNotMoneyStock { get; set; }
        public List<string> unitsMoneyOther{ get; set; }

        private Dictionary<string, Unit> FromSymbolToUnit;

        public Visibility IsFullVAT
        {
            get
            {
                return InitialData.Settings.VAT_type == VATType.full ? Visibility.Visible : Visibility.Collapsed;
            }
            set
            {

            }
        }

        public List<double> Taxes { get; protected set; }


        private void Init()
        {
            ID = product.ID;
            IsFullVAT = InitialData.Settings.VAT_type == VATType.full ? Visibility.Visible : Visibility.Collapsed;
            Taxes = new List<double>();
            Taxes.Add(0);
            Taxes.Add(10);
            Taxes.Add(20);


            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsNotMoneyMinimum = new List<string>();
            unitsNotMoneyStock = new List<string>();
            unitsMoneyOther = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time);
            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID != money.ID && u.UnitSequence_ID != time.ID)
                {
                    unitsNotMoneyStock.Add(u.Symbol);
                    unitsNotMoneyMinimum.Add(u.Symbol);
                }
                else
                if(u.UnitSequence_ID == money.ID)
                {
                    unitsMoneyOther.Add(u.Symbol);
                }
            }

        }

        public ProductViewModel(Unit defaultUnit, CommonViewModel commonViewModel)
        {
            CommonViewModel = commonViewModel;
            product = new ProductNullable();
            Init();
            indexResourceOfProductViewModel = new IndexResourceOfProductViewModel(product, commonViewModel);
            indexRiskOfProductViewModel = new IndexRiskOfProductViewModel(product,CommonViewModel);
            if (defaultUnit is null)
                defaultUnit = UnitDAL.GetUnitsNotMoney().First();
            unitOFMinimum = defaultUnit;
            unitOfStock = defaultUnit;
            unitOfOther = defaultUnit;
            product.UnitOfMaximumID = defaultUnit.ID;
            product.UnitOfMimimumID = defaultUnit.ID;
            product.UnitOfStockID = defaultUnit.ID;
            product.UnitOfOtherID = defaultUnit.ID;
            product.Name = "";

        }

        public ProductViewModel(ProductNullable product, CommonViewModel commonViewModel)
        {
            CommonViewModel = commonViewModel;
            this.product = product;
            Init();
            indexResourceOfProductViewModel = new IndexResourceOfProductViewModel(product, commonViewModel);
            indexRiskOfProductViewModel = new IndexRiskOfProductViewModel(product, commonViewModel);
            unitOFMinimum = UnitDAL.GetUnit(product.UnitOfMimimumID);
            unitOfStock = UnitDAL.GetUnit(product.UnitOfStockID);
            unitOfOther= UnitDAL.GetUnit(product.UnitOfOtherID);

            OnPropertyChanged(nameof(IndexResourceOfProductViewModel));
            OnPropertyChanged(nameof(IndexRiskOfProductViewModel));
        }

        public string Name {
            get {
                return product.Name;
            }
            set
            {
                string last = product.Name;
                product.Name = value;
                OnPropertyChanged("Name");
                Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.Name), last, product.Name, last + '\n'+ "Наименование", last+'\n'+ product.Name);
                Save();
            }
        }

        public bool? IsInteger {
            get
            {
                return product.IsInteger;
            }
            set
            {
                bool? last = product.IsInteger;
                var lastval = BoolToStr(last);
                product.IsInteger = value;
                OnPropertyChanged("IsInteger");
                var next = product.IsInteger;
                var nextval = BoolToStr(next);
                Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.IsInteger), last, product.IsInteger,product.Name+ '\n'+ "Целочисленность", lastval+'\n'+nextval);


                Save();
            }
        }

        public double? Minimum {
            get
            {
                return product.Minimum;
            }
            set
            {
                if(value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = product.Minimum;
                product.Minimum = value;
                OnPropertyChanged("Minimum");
                Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.Minimum), last, product.Minimum, product.Name + '\n' + "Минимум", last.ToString()+'\n'+product.Minimum);

                Save();
            }
        }

        public long? UnitOfMinimumID
        {
            get
            {
                return product.UnitOfMimimumID;
            }
            set
            {
                product.UnitOfMimimumID = value;
                unitOFMinimum = UnitDAL.GetUnit(value);
                OnPropertyChanged("UnitOfMinimumID");
                Save();
            }
        }

        private Unit unitOFMinimum;

        public Unit UnitOfMinimum
        {
            get
            {
                return unitOFMinimum;
            }
            set
            {
                UnitOfMinimumID = value.ID;
                unitOFMinimum = UnitDAL.GetUnit(value.ID);
                OnPropertyChanged("UnitOfMinimum");
                Save();
            }
        }

        public string UnitOfMinimumSymbol
        {
            get
            {
                if (unitOFMinimum is null)
                    return null;
                return unitOFMinimum.Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    var lastval = product.UnitOfMimimumID;
                    var lastval_sym = UnitOfMinimumSymbol;
                    long last =unitOFMinimum is null ? UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money).ID: unitOFMinimum.UnitSequence_ID;
                    unitOFMinimum = FromSymbolToUnit[value];
                    unitOfStock= FromSymbolToUnit[value];
                    product.UnitOfMimimumID = unitOFMinimum.ID;
                    product.UnitOfStockID = unitOfStock.ID;
                    if (last != unitOFMinimum.UnitSequence_ID)
                    {
                        indexRiskOfProductViewModel.ReloadUnits(unitOFMinimum.UnitSequence_ID);
                    }
                    
                    OnPropertyChanged("UnitOfMinimumSymbol");
                    Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.UnitOfMimimumID), lastval, product.UnitOfMimimumID, product.Name + '\n' + "Е.И. Количества", lastval_sym+'\n'+UnitOfMinimumSymbol);

                    Save();
                }
            }
        }
        public string UnitOfStockSymbol
        {
            get
            {
                return unitOfStock.Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    unitOfStock = FromSymbolToUnit[value];
                    product.UnitOfStockID = unitOfStock.ID;
                    OnPropertyChanged("UnitOfStockSymbol");
                    Save();
                }
            }
        }

        public double? Stock
        {
            get
            {
                return product.Stock;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var lastval = product.Stock;
                product.Stock = value;

                OnPropertyChanged("Stock");
                Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.Stock), lastval, product.Stock, product.Name + '\n' + "Запасы", lastval.ToString()+'\n'+product.Stock );

                Save();
            }
        }

        public long? UnitOfStockID
        {
            get
            {
                return product.UnitOfStockID;
            }
            set
            {
                product.UnitOfStockID = value;
                unitOfStock = UnitDAL.GetUnit(value);
                OnPropertyChanged("UnitOfStockID");
                Save();
            }
        }
        private Unit unitOfStock;
        public Unit UnitOfStock
        {
            get
            {
                return unitOfStock;
            }
            set
            {
                UnitOfStockID = value.ID;
                unitOfStock = UnitDAL.GetUnit(value.ID);
                OnPropertyChanged("UnitOfStock");
                Save();
            }
        }

        public double? OtherCosts
        {
            get
            {
                return product.OtherCosts;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                product.OtherCosts = value;
                OnPropertyChanged("OtherCosts");
                Save();
            }
        }

        public long? UnitOfOtherID
        {
            get
            {
                return product.UnitOfOtherID;
            }
            set
            {
                product.UnitOfOtherID = value;
                unitOfOther = UnitDAL.GetUnit(value);
                OnPropertyChanged("UnitOfOtherID");
                Save();
            }
        }
        private Unit unitOfOther;
        public Unit UnitOfOther
        {
            get
            {
                return unitOfOther;
            }
            set
            {
                UnitOfOtherID = value.ID;
                unitOfOther = UnitDAL.GetUnit(value.ID);
                OnPropertyChanged("UnitOfOther");
                Save();
            }
        }

        public string UnitOfOtherSymbol
        {
            get
            {
                if (unitOfOther is null)
                    return null;
                return unitOfOther.Symbol;
            }
            set
            {
                if (FromSymbolToUnit.ContainsKey(value))
                {
                    unitOfOther = FromSymbolToUnit[value];
                    product.UnitOfOtherID = unitOfOther.ID;
                    OnPropertyChanged("UnitOfOtherSymbol");
                    Save();
                }
            }
        }

        public bool? Perishable
        {
            get
            {
                return product.Perishable;
            }
            set
            {
                bool? last = product.Perishable;
                var lastval = BoolToStr(last);
                product.Perishable = value;

                OnPropertyChanged("Perishable");
                bool? next = product.Perishable;
                var nextval = BoolToStr(next);
                Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.Perishable), last, next, product.Name + '\n' + "Скоропорт", lastval+'\n'+nextval);

                Save();
            }
        }//скоропортящийся

        public double? Rate
        {
            get
            {
                return product.Rate*100;
            }
            set
            {
                var lastval = product.Rate;
                product.Rate = Math.Round((double)value/100,1);

                OnPropertyChanged("Rate");
                Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, nameof(product.Rate), lastval, product.Rate, product.Name + '\n' + "Налог", lastval*100+"%"+'\n'+product.Rate*100+"%");

                Save();
            }
        }//ставка, для налогов


        public bool Equals(ProductViewModel pvm)
        {
            return product.ID == pvm.product.ID;
        }

        public void Save()
        {
            ProductDAL.AddProduct(product);
            //IndexResourceOfProductViewModel.Save();
        }

        public void Delete()
        {
            IndexResourceOfProductViewModel.Delete();
            ProductDAL.DeleteProduct(product);
            Logger(SQLiteLibrary.TypeOfHistoryItem.Product, product.ID, "", product.ID, null,
                "Товар" + '\n' + product.Name, "Удален");
        }

        public void ReloadUnits()
        {
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsNotMoneyMinimum = new List<string>();
            unitsNotMoneyStock = new List<string>();
            unitsMoneyOther = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time);
            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID != money.ID && u.UnitSequence_ID != time.ID)
                {
                    unitsNotMoneyStock.Add(u.Symbol);
                    unitsNotMoneyMinimum.Add(u.Symbol);
                }
                else
                if (u.UnitSequence_ID == money.ID)
                {
                    unitsMoneyOther.Add(u.Symbol);
                }
            }
            IndexRiskOfProductViewModel.ReloadUnits(unitOFMinimum.UnitSequence_ID);
            OnPropertyChanged(nameof(IndexRiskOfProductViewModel));
            IndexResourceOfProductViewModel.ReloadUnits();
            OnPropertyChanged(nameof(IndexResourceOfProductViewModel));
            OnPropertyChanged("unitsNotMoneyMinimum");
            OnPropertyChanged("unitsNotMoneyStock");
            OnPropertyChanged("unitsMoneyOther");
            OnPropertyChanged("unitsNotMoneyStock");
        }

    }
}
