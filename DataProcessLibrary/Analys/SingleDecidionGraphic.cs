using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Analys
{
    [Serializable]
    public class SingleDecidionGraphic:DecisionGraphic
    {
        public SingleDecidionGraphic()
        {

        }
        public SingleDecidionGraphic(Dictionary<double, SingleDecidionScript> decidionGraphic):base()
        {
            this.decidionGraphic = RecountRisks(decidionGraphic);
            best_points = ToPoints(TypeOfGraphic.best);
            bad_points = ToPoints(TypeOfGraphic.bad);
            perish_points = ToPoints(TypeOfGraphic.perishable);
            adviced_points = GetAdvicesPoints();
            SetRecomended();
            decidionGraphic_Serialize = new List<HelpPairDouble_DecScr>();
            foreach(var d in decidionGraphic)
            {
                decidionGraphic_Serialize.Add(new HelpPairDouble_DecScr() { risk=d.Key, DecidionScript=d.Value});
            }
        }

        public Dictionary<double, DecidionScript> RecountRisks(Dictionary<double, SingleDecidionScript> decidionGraphic)
        {
            Dictionary<double, DecidionScript> newdict = new Dictionary<double, DecidionScript>();
            foreach (var d in decidionGraphic)
            {
                if (!newdict.ContainsKey(d.Value.RealRisk) || newdict.ContainsKey(d.Value.RealRisk) && newdict[d.Value.RealRisk].ProfitIfBest < d.Value.ProfitIfBest)
                {
                    newdict[Math.Round(d.Value.RealRisk)] = d.Value;
                }
            }
            return newdict;
        }  
   

    }
}
