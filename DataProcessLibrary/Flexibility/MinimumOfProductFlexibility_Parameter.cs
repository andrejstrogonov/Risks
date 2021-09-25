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
    public class MinimumOfProductFlexibility_Parameter : Parameter
    {
        public Product Product { get; set; }

        public MinimumOfProductFlexibility_Parameter()
        {
            Sign = -1;
        }

        public override void ChangeData(double newMeaning)
        {
            Product.Minimum = newMeaning;
            //ProductData.SetProduct(Product);
        }

        public override void Init(object o)
        {
            Product = (Product)o;
            FirstMeaning = Product.Minimum;
            IsInteger = Product.IsInteger;
            LastMeaning = Product.Minimum;
            Name = Product.Name;
            TypeOfParameter = TypeOfParameter.MinimumOfProduct;
            UnitSymbol = UnitDAL.GetUnit(Product.UnitOfMimimumID);
        }

        public override void Save()
        {
            ProductDAL.AddProduct(Product);
            HistoryItem = Logger(SQLiteLibrary.TypeOfHistoryItem.Product, Product.ID, nameof(Product.Minimum), FirstMeaning, Product.Minimum, Product.Name + '\n' + "Минимум", FirstMeaning.ToString() + '\n' + Product.Minimum);

        }

        public override void Save(ProfitFlexibility profitFlexibility)
        {
            FixedCost fixedCost = new FixedCost();
            fixedCost.TypeOfFixedCosts = TypeOfFixedCosts.outcoming;
            fixedCost.Name = "Минимум товара " + Name + ": c " + profitFlexibility.LastMeaning + " на " + profitFlexibility.NewMeaning;
            fixedCost.Price = profitFlexibility.PriceOfChange;
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money).ID;
            fixedCost.UnitOfPriceID = TransferOfUnitsDAL.GetCommonUnitForType( money).ID;
            FixedCostDAL.SetFixedCost(fixedCost);
        }

        public override void TransferFromCommon()
        {
            Product = ProductDAL.TransferFromCommon(Product);
            LastMeaning = Product.Minimum;
        }

        public override void TransferToCommon()
        {
            Product = ProductDAL.TransferToCommon(Product);
            LastMeaning = Product.Minimum;
        }
    }
}
