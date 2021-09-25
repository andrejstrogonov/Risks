using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class FixedCostViewModel:ViewModel
    {
        private FixedCost fixedCost;

        public bool IsFixedLine { get; set; }

        public List<string> unitsMoney { get; set; }
        private Dictionary<string, Unit> FromSymbolToUnit;

        public TypeOfFixedCosts TypeOfFixedCosts {
            get
            {
                return fixedCost.TypeOfFixedCosts;
            }
            private set { } }

        public void Init()
        {
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsMoney = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            foreach (var u in UnitDAL.GetUnits())
            {
                FromSymbolToUnit[u.Symbol] = u;
                if (u.UnitSequence_ID == money.ID)
                {
                    unitsMoney.Add(u.Symbol);
                }
            }
            OnPropertyChanged(nameof(unitsMoney));
        }



        public FixedCostViewModel(FixedCost fixedCost, CommonViewModel commonViewModel):base(commonViewModel)
        {
            this.fixedCost = fixedCost;
            if (fixedCost.TypeOfFixedCosts == TypeOfFixedCosts.admortization)
                IsFixedLine = true;
            Init();
        }

        public FixedCostViewModel(TypeOfFixedCosts typeOfFixedCosts, CommonViewModel commonViewModel):base(commonViewModel)
        {
            fixedCost = new FixedCost();
            fixedCost.TypeOfFixedCosts = typeOfFixedCosts;
            if (typeOfFixedCosts == TypeOfFixedCosts.admortization)
                IsFixedLine = true;
            fixedCost.Name = "";
            fixedCost.Description = "";
            fixedCost.UnitOfPriceID = UnitDAL.GetUnitsMoney().First().ID;
            Init();
        }

        private string TypeToStr()
        {
            if (fixedCost.TypeOfFixedCosts == TypeOfFixedCosts.outcoming)
                return "расход";
            return "доход";
        }

        public string Name
        {
            get
            {
                return fixedCost.Name;
            }
            set
            {
                var last = fixedCost.Name;
                fixedCost.Name = value;
                OnPropertyChanged(nameof(Name));
                var next = fixedCost.Name;

                Logger(SQLiteLibrary.TypeOfHistoryItem.FixedCost, fixedCost.ID, nameof(fixedCost.Name), last, next,
                    last +'\n'+"Название", last + '\n'  + next);

                Save();
            }
        }

        public double? Price
        {
            get
            {
                return fixedCost.Price;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = fixedCost.Price;
                fixedCost.Price = value;
                OnPropertyChanged(nameof(Price));
                var next = fixedCost.Price;

                Logger(SQLiteLibrary.TypeOfHistoryItem.FixedCost, fixedCost.ID, nameof(fixedCost.Price), last, next,
                    fixedCost.Name + '\n' + "Стоимость", last.ToString() + '\n' + next);

                Save();
            }
        }

        public double PriceInCommonUnit
        {
            get
            {
                if (Price is null)
                    return 0;
                return UnitDAL.TranslateToCommonUnit((long)fixedCost.UnitOfPriceID, (double)Price);
            }
        }

        public bool? IsDecrease
        {
            get
            {
                return fixedCost.IsDecrease;
            }
            set
            {
                var last = fixedCost.IsDecrease;
                var lastval = BoolToStr(last);
                fixedCost.IsDecrease = value;
                var next = fixedCost.IsDecrease;
                var nextval = BoolToStr(next);
                Logger(SQLiteLibrary.TypeOfHistoryItem.FixedCost, fixedCost.ID, nameof(fixedCost.IsDecrease), last, next,
                    fixedCost.Name +'\n'+"Снижает НБ", lastval + '\n' + nextval);
                
                OnPropertyChanged(nameof(IsDecrease));
            }
        }

        public string Unit
        {
            get
            {
                return UnitDAL.GetUnit(fixedCost.UnitOfPriceID).Symbol;
            }
            set
            {
                if (FromSymbolToUnit.ContainsKey(value))
                {
                    var last = fixedCost.UnitOfPriceID;
                    var lastval = Unit;
                    fixedCost.UnitOfPriceID = FromSymbolToUnit[value].ID;
                    OnPropertyChanged(nameof(Unit));

                    var next= fixedCost.UnitOfPriceID;
                    var nextval = Unit;

                    Logger(SQLiteLibrary.TypeOfHistoryItem.FixedCost, fixedCost.ID, nameof(fixedCost.Name), last, next,
                    fixedCost.Name + '\n' + "Е.И. цены", lastval + '\n'+ nextval);
                    Save();
                }
            }
        }

        public void Save()
        {
            FixedCostDAL.SetFixedCost(fixedCost);
        }

        public void Delete()
        {
            FixedCostDAL.DeleteFixedCost(fixedCost.ID);
            Logger(SQLiteLibrary.TypeOfHistoryItem.FixedCost, fixedCost.ID, "", fixedCost.ID, null,
                   TypeToStr()+'\n'+ fixedCost.Name ,"Удален");

        }

        public void ReloadUnits()
        {
            Init();
        }
    }
}
