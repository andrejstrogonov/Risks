using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using System.Collections.ObjectModel;

namespace Riscks.ViewModel
{
    public class RiskOfProductViewModel : ViewModel
    {
        private ProductNullable product;
        public RiskOfProductNullable riskOfProduct { get; set; }

        public List<string> unitsMoney { get; set; }
        public List<string> unitsNotMoney { get; set; }

        private Dictionary<string, Unit> FromSymbolToUnit;

        public void Init()
        {
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsMoney = new List<string>();
            unitsNotMoney = new List<string>();
            long typeOfUnit = 1;
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            if(!(product.UnitOfMimimumID is null))
            {
                typeOfUnit = UnitDAL.GetUnit(product.UnitOfMimimumID).UnitSequence_ID;
            }
            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID == money.ID)
                {
                    unitsMoney.Add(u.Symbol);
                }
                else
                if (u.UnitSequence_ID == typeOfUnit || riskOfProduct.UnitOfAmountID is null)
                {
                    unitsNotMoney.Add(u.Symbol);
                }
            }
            if (riskOfProduct.UnitOfAmountID is null)
            {
                riskOfProduct.UnitOfAmountID = FromSymbolToUnit[unitsNotMoney.First()].ID;
            }
            Save();
            OnPropertyChanged(nameof(unitsMoney));
            OnPropertyChanged(nameof(unitsNotMoney));
        }
   
        public RiskOfProductViewModel(ProductNullable product, CommonViewModel commonViewModel):base(commonViewModel)
        {
            this.product = product;
            riskOfProduct = new RiskOfProductNullable();
            riskOfProduct.ID_Product = product.ID;
            riskOfProduct.UnitOfPriceID = UnitDAL.GetUnitsMoney().First().ID;
            riskOfProduct.UnitOfAmountID = product.UnitOfMimimumID;
            Init();
        }

        public RiskOfProductViewModel(RiskOfProductNullable riskOfProduct, CommonViewModel commonViewModel):base(commonViewModel)
        {
            this.riskOfProduct = riskOfProduct;
            this.product = ProductDAL.GetProductNullable(riskOfProduct.ID_Product);
            riskOfProduct.UnitOfAmountID = riskOfProduct.UnitOfAmountID;
            if(riskOfProduct.UnitOfAmountID is null)
            {
                riskOfProduct.UnitOfAmountID = product.UnitOfMimimumID;
            }
            Init();
            /*if (riskOfProduct.UnitOfPriceID == 1)
                riskOfProduct.UnitOfPriceID = 6;*/
        }

        public double? GarantedAmount
        {
            get
            {
                return riskOfProduct.GarantedAmount;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = riskOfProduct.GarantedAmount;
                riskOfProduct.GarantedAmount = value;
                var next = riskOfProduct.GarantedAmount;
                OnPropertyChanged("GarantedAmount");
                Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, riskOfProduct.ID, nameof(riskOfProduct.GarantedAmount), last, next,
                    product.Name + '\n' + Price + ": Гарант", last.ToString() + '\n' + next);

                Save();
            }
        }

        public double? MaximumAmount
        {
            get
            {
                return riskOfProduct.MaximumAmount;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = riskOfProduct.MaximumAmount;
                riskOfProduct.MaximumAmount = value;
                var next = value;
                OnPropertyChanged("MaximumAmount");
                Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, riskOfProduct.ID, nameof(riskOfProduct.MaximumAmount), last, next,
                    product.Name+'\n'+Price+ ": Максимум", last.ToString() + '\n' + next);
                Save();
            }
        }

        public double? Price
        {
            get
            {
                return riskOfProduct.Price;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = riskOfProduct.Price;
                riskOfProduct.Price = value;
                var next = value;
                OnPropertyChanged("Price");
                Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, riskOfProduct.ID, nameof(riskOfProduct.Price), last, next,
                    product.Name + '\n' + Price + ": Цена", last.ToString() + '\n' + next);
                Save();
            }
         
        }

        public string UnitOfPriceSymbol
        {
            get
            {
                return UnitDAL.GetUnit(riskOfProduct.UnitOfPriceID).Symbol;
            }
            set
            {
                if (FromSymbolToUnit.ContainsKey(value))
                {
                    var last = riskOfProduct.UnitOfPriceID;
                    var lastval =UnitOfPriceSymbol;
                    riskOfProduct.UnitOfPriceID = FromSymbolToUnit[value].ID;
                    var next = riskOfProduct.UnitOfPriceID;
                    var nextval = UnitOfPriceSymbol;
                    OnPropertyChanged(nameof(UnitOfPriceSymbol));
                    Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, riskOfProduct.ID, nameof(riskOfProduct.UnitOfPriceID), last, next,
                    product.Name + '\n' + Price + ": Е.И. цены", lastval + '\n' + nextval);
                    Save();
                }
            }
        }

        public string UnitOfAmountSymbol
        {
            get
            {
                if (riskOfProduct.UnitOfAmountID is null)
                    return null;
                return UnitDAL.GetUnit(riskOfProduct.UnitOfAmountID).Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    var last = riskOfProduct.UnitOfAmountID;
                    var lastval = UnitOfAmountSymbol;
                    riskOfProduct.UnitOfAmountID = FromSymbolToUnit[value].ID;
                    var next = riskOfProduct.UnitOfAmountID;
                    var nextval = UnitOfAmountSymbol;
                    OnPropertyChanged(nameof(UnitOfAmountSymbol));
                    Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, riskOfProduct.ID, nameof(riskOfProduct.UnitOfPriceID), last, next,
                    product.Name + '\n' + Price + ": Е.И. объема", lastval + '\n' + nextval);
                    Save();
                }
            }
        }

        public void Save()
        {
            RiskOfProductDAL.AddRiskOfProduct(riskOfProduct);
        }

        public void Delete()
        {
            Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, riskOfProduct.ID, "", riskOfProduct.ID, null,
                    product.Name + '\n' + Price + ": Комбинация", "Удалена");
            RiskOfProductDAL.DeleteRiskOfProduct(riskOfProduct);
        }

        public void ReloadUnits(long unitSequence_id)
        {
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsMoney = new List<string>();
            unitsNotMoney = new List<string>();
            var unitmonel = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);

            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID == unitmonel.ID)
                {
                    unitsMoney.Add(u.Symbol);
                }
                else
                if (u.UnitSequence_ID == unitSequence_id || riskOfProduct.UnitOfAmountID is null)
                {
                    unitsNotMoney.Add(u.Symbol);
                }
            }
            if(riskOfProduct.UnitOfAmountID is null)
                riskOfProduct.UnitOfAmountID = FromSymbolToUnit[unitsNotMoney.First()].ID;
            OnPropertyChanged(nameof(UnitOfAmountSymbol));
            OnPropertyChanged(nameof(unitsMoney));
            OnPropertyChanged(nameof(unitsNotMoney));
            Save();
        }
    }
}
