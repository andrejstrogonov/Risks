using DataProcessLibrary.InitialData;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Flexibility
{
    public class MaximumOfProductFlexibility_Parameter : Parameter
    {
        private RiskOfProduct RiskOfProduct { get; set; }

        public MaximumOfProductFlexibility_Parameter()
        {
            Sign = 1;
        }

        public override void ChangeData(double newMeaning)
        {
            RiskOfProduct.MaximumAmount = newMeaning;
            //PriceOfProductData.SetPriceOfProduct(RiskOfProduct);
        }

        public override void Init(object o)
        {
            RiskOfProduct = (RiskOfProduct)o;
            FirstMeaning = RiskOfProduct.MaximumAmount;
            IsInteger = RiskOfProduct.Product.IsInteger;
            Name = RiskOfProduct.Product.Name;
            LastMeaning = RiskOfProduct.MaximumAmount;
            TypeOfParameter = TypeOfParameter.MaximumOfProduct;
            UnitSymbol = UnitDAL.GetUnit(RiskOfProduct.UnitOfAmountID);
        }

        public override void Save()
        {
            RiskOfProductDAL.AddRiskOfProduct(RiskOfProduct);
            var product = ProductDAL.GetProduct(RiskOfProduct.ID_Product);
            HistoryItem = Logger(SQLiteLibrary.TypeOfHistoryItem.RiskOfProduct, RiskOfProduct.ID, nameof(RiskOfProduct.MaximumAmount), FirstMeaning, RiskOfProduct.MaximumAmount,
                    product.Name + '\n' + RiskOfProduct.Price + ": Максимум", FirstMeaning.ToString() + '\n' + RiskOfProduct.MaximumAmount);
        }

        public override void Save(ProfitFlexibility profitFlexibility)
        {
            FixedCost fixedCost = new FixedCost();
            fixedCost.TypeOfFixedCosts = TypeOfFixedCosts.outcoming;
            double procent = Math.Round((profitFlexibility.NewMeaning - profitFlexibility.LastMeaning) / profitFlexibility.LastMeaning * 100, 3);
            fixedCost.Name = "Максимум товара " + Name +  " на " + procent.ToString()+"%";
            fixedCost.Price = profitFlexibility.PriceOfChange;
            fixedCost.UnitOfPriceID = RiskOfProduct.UnitOfPriceID;
            FixedCostDAL.SetFixedCost(fixedCost);
        }

        public override void TransferFromCommon()
        {
            RiskOfProduct = RiskOfProductDAL.TransferFromCommon(RiskOfProduct);
            LastMeaning = RiskOfProduct.MaximumAmount;
        }
        public override void TransferToCommon()
        {
            RiskOfProduct = RiskOfProductDAL.TransferToCommon(RiskOfProduct);
            LastMeaning = RiskOfProduct.MaximumAmount;
        }
    }



    public class MaximumOfProductFlexibility_GroupParameter : GroupParameter
    {
        public MaximumOfProductFlexibility_GroupParameter()
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
                var elem = new MaximumOfProductFlexibility_Parameter();
                elem.Init(r);
                Parameters.Add(elem);
            }
            Name = Parameters.First().Name;
        }
    }

}
