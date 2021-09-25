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
    public class MaxAmountOfResourceFlexibility_Parameter : Parameter
    {
        public Resource Resource { get; set; }

        public MaxAmountOfResourceFlexibility_Parameter()
        {
            Sign = 1;
            IsInteger = false;
        }

        public override void ChangeData(double newMeaning)
        {
            Resource.Maximum = newMeaning;
            //ResourceData.SetResource(Resource);
        }

        public override void Init(object o)
        {
            Resource = (Resource)o;
            Name = Resource.Name;
            LastMeaning = Resource.Maximum;
            TypeOfParameter = TypeOfParameter.MaxAmountOfResource;
            UnitSymbol = UnitDAL.GetUnit(Resource.UnitOfMaximumID);
        }

        public override void Save()
        {
            ResourceDAL.SetResource(Resource); 
            HistoryItem = Logger(SQLiteLibrary.TypeOfHistoryItem.Resource, Resource.ID, nameof(Resource.Maximum), FirstMeaning, Resource.Maximum,
                     Resource.Name + '\n' + "Максимум", FirstMeaning.ToString() + '\n' + Resource.Maximum);
        }

        public override void Save(ProfitFlexibility profitFlexibility)
        {
            FixedCost fixedCost = new FixedCost();
            fixedCost.TypeOfFixedCosts = TypeOfFixedCosts.outcoming;
            fixedCost.Name = "Максимум ресурса " + Name + ": c " + profitFlexibility.LastMeaning + " на " + profitFlexibility.NewMeaning;
            fixedCost.Price = profitFlexibility.PriceOfChange;
            fixedCost.UnitOfPriceID = Resource.UnitOfPriceID;
            FixedCostDAL.SetFixedCost(fixedCost);
        }

        public override void TransferFromCommon()
        {
            Resource = ResourceDAL.TransferFromCommon(Resource);
            LastMeaning = Resource.Maximum;
        }

        public override void TransferToCommon()
        {
            Resource = ResourceDAL.TransferToCommon(Resource);
            LastMeaning = Resource.Maximum;
        }
    }
}
