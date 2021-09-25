using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Flexibility
{
    public class ProfitFlexibility
    {
        public IParameter Parameter { get; set; }
        public double Sign { get; set; }
        public double Procent { get; set; }
        public TypeOfParameter TypeOfParameter { get; set; }
        public string Parameter_Change { get; set; }
        public double Profit_Change_Procent { get; set; }
        public double Coefficient_Flexibility { get; set; }
        public double Profit_Change_Absolut { get; set; }
        public bool? Posibility { get; set; }
        public double? PriceOfChange { get; set; }
        public double Result { get; set; }
        public double LastMeaning { get; set; }
        public string UnitOfMeaning { get; set; }
        public double NewMeaning { get; set; }

        public ProfitFlexibility(IParameter parameter) {
            Parameter = parameter;
        }

        public ProfitFlexibility(double last, double next)
        {
            LastMeaning = last;
            NewMeaning = next;
            Profit_Change_Absolut = next - last;
            Profit_Change_Procent = Profit_Change_Absolut / LastMeaning * 100;
        }

        public void Save()
        {
            if(PriceOfChange>0)
                Parameter.Save(this);
        }
    }

    public class ResultOfChange
    {
        public double LastProfit { get; set; }
        public double NewProfit { get; set; }
        public double AbsolutChange { get; set; }
        public double ProcentChange { get; set; }
        public double PriceOfChange { get; set; }
        public double Result { get; set; }
        public double PredictableResult { get; set; }
        public double AbsolutError { get; set; }
    }
}
