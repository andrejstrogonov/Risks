using DataProcessLibrary.InitialData;
using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Flexibility
{
    public class GarantAmountOfProductFlexibility_Parameter : Parameter
    {
        public RiskOfProduct RiskOfProduct { get; set; }

        public GarantAmountOfProductFlexibility_Parameter()
        {
            Sign = 1;
        }

        public override void ChangeData(double newMeaning)
        {
            RiskOfProduct.GarantedAmount = newMeaning;
            //PriceOfProductData.SetPriceOfProduct(RiskOfProduct);
        }

        public override void Init(object o)
        {
            RiskOfProduct = (RiskOfProduct)o;
            IsInteger = RiskOfProduct.Product.IsInteger;
            LastMeaning = RiskOfProduct.GarantedAmount;
            FirstMeaning = RiskOfProduct.GarantedAmount;
            Name =  RiskOfProduct.Product.Name;
            TypeOfParameter = TypeOfParameter.GarantOfProduct;
            UnitSymbol = UnitDAL.GetUnit(RiskOfProduct.UnitOfAmountID);
        }

        public override void Save()
        {
            RiskOfProductDAL.AddRiskOfProduct(RiskOfProduct);
            var product = ProductDAL.GetProduct(RiskOfProduct.ID_Product);
            HistoryItem= Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, RiskOfProduct.ID, nameof(RiskOfProduct.GarantedAmount), this.FirstMeaning, RiskOfProduct.GarantedAmount,
                    product.Name + '\n' + RiskOfProduct.Price + ": Гарант", FirstMeaning.ToString() + '\n' + RiskOfProduct.GarantedAmount);
        }
        public override void Save(ProfitFlexibility profitFlexibility)
        {
            FixedCost fixedCost = new FixedCost();
            fixedCost.TypeOfFixedCosts = TypeOfFixedCosts.outcoming;
            double procent = Math.Round((profitFlexibility.NewMeaning - profitFlexibility.LastMeaning) / profitFlexibility.LastMeaning * 100, 3);
            fixedCost.Name = "Гарант товара " + Name + " на " + procent.ToString() + "%";
            fixedCost.Price = profitFlexibility.PriceOfChange;
            fixedCost.UnitOfPriceID = RiskOfProduct.UnitOfPriceID;
            FixedCostDAL.SetFixedCost(fixedCost);
        }

        public override void TransferFromCommon()
        {
            RiskOfProduct = RiskOfProductDAL.TransferFromCommon(RiskOfProduct);
            LastMeaning = RiskOfProduct.GarantedAmount;
        }
        public override void TransferToCommon()
        {
            RiskOfProduct = RiskOfProductDAL.TransferToCommon(RiskOfProduct);
            LastMeaning = RiskOfProduct.GarantedAmount;
        }
                
    }

    public class GarantAmountOfProductFlexibility_GroupParameter : GroupParameter
    {
        public GarantAmountOfProductFlexibility_GroupParameter()
        {
            
        }

        public override void Init(object o)
        {
            Product p = (Product)o;
            isInteger = p.IsInteger;
            Parameters = new List<Parameter>();
            var risks = RiskOfProductData.GetPricesAndAmountsOfProduct(p);
            foreach (var r in risks)
            {
                var elem = new GarantAmountOfProductFlexibility_Parameter();
                
                elem.Init(r);
                Parameters.Add(elem);
            }
            Name = Parameters.First().Name;
        }
    }

}
