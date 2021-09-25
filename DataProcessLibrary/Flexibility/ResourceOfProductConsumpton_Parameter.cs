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
    public class ResourceOfProductConsumpton_Parameter : Parameter
    {
        public ResourceOfProduct ResourceOfProduct { get; set; }

        public ResourceOfProductConsumpton_Parameter()
        {
            Sign = -1;
            IsInteger = false;
        }
               
        public override void ChangeData(double newMeaning)
        {
            ResourceOfProduct.AmountOfResource = newMeaning;
            //ResourceOfProductData.SetResourceOfProduct(ResourceOfProduct);
        }

        public override void Init(object o)
        {
            ResourceOfProduct = (ResourceOfProduct)o;
            Name = ProductData.GetProduct( ResourceOfProduct.ID_Product).Name+": "+ResourceData.GetResource(ResourceOfProduct.ID_Resource).Name;
            LastMeaning = ResourceOfProduct.AmountOfResource;
            TypeOfParameter = TypeOfParameter.ResourceOfProductConsumption;
            UnitSymbol = UnitDAL.GetUnit(ResourceOfProduct.UnitOfResourceID);
        }

        public override void Save()
        {
            var resource = ResourceOfProduct.Resource;
            var prod = ResourceOfProduct.Product;
            ResourceOfProduct.Resource = null;
            ResourceOfProduct.Product = null;
            ResourceOfProductDAL.AddResourceOfProduct(ResourceOfProduct);
            ResourceOfProduct.Resource = resource;
            ResourceOfProduct.Product = prod;

            HistoryItem = Logger(SQLiteLibrary.TypeOfHistoryItem.ResourceOfProduct, ResourceOfProduct.ID, nameof(ResourceOfProduct.AmountOfResource), FirstMeaning, ResourceOfProduct.AmountOfResource,
                       ResourceOfProduct.Product.Name + '\n' + resource.Name, FirstMeaning.ToString() + '\n' + ResourceOfProduct.AmountOfResource);

        }

        public override void Save(ProfitFlexibility profitFlexibility)
        {
            FixedCost fixedCost = new FixedCost();
            fixedCost.TypeOfFixedCosts = TypeOfFixedCosts.outcoming;
            fixedCost.Name = "Состав товара " + Name + ": c " + profitFlexibility.LastMeaning + " на " + profitFlexibility.NewMeaning;
            fixedCost.Price = profitFlexibility.PriceOfChange;
            fixedCost.UnitOfPriceID = ResourceDAL.GetResource(ResourceOfProduct.ID_Resource).UnitOfPriceID;
            FixedCostDAL.SetFixedCost(fixedCost);
        }

        public override void TransferFromCommon()
        {
            ResourceOfProduct = ResourceOfProductDAL.TransferFromCommon(ResourceOfProduct);
            LastMeaning = ResourceOfProduct.AmountOfResource;
        }

        public override void TransferToCommon()
        {
            ResourceOfProduct = ResourceOfProductDAL.TransferToCommon(ResourceOfProduct);
            LastMeaning = ResourceOfProduct.AmountOfResource;
        }
    }
}
