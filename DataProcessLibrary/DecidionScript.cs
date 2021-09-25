using DataProcessLibrary.Analys;
using DataProcessLibrary.CommonAnalys;
using DataProcessLibrary.Data;
using DataProcessLibrary.InitialData;
using SimplexPlan;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataProcessLibrary
{
    [Serializable]
    [XmlInclude(typeof(SingleDecidionScript))]
    [XmlInclude(typeof(CommonScript))]
    public abstract class DecidionScript
    {
        public double RealRisk { get; set; }

        public double ProfitIfBad { get; set; }
        public double ProfitIfBest { get; set; }
        public double ProfitWithPerishable { get; set; }
        public double ProfitMiddle { get; set; }
        public double delta_VAT { get; set; }
        public double BudgetCost { get; set; }
        public double Proceeds { get; set; }
        public double Expenses { get; set; }
        public double FixedCosts_Incoming { get; set; }
        public double FixedCosts_Outcomung { get; set; }
        [XmlIgnore]
        public Dictionary<string,ResourceScript> ResourcesInUse;
        [XmlIgnore]
        public Dictionary<string, ProductScript> ProductsProceeds;


        protected double MiddleProfit(double Risk)
        {
            return ProfitIfBest*(Risk / 100);
        }

        public DecidionScript() { }

        public abstract void Deserialize();
    }
}
