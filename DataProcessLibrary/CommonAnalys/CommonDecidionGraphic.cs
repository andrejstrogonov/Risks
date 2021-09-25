using DataProcessLibrary.Analys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.CommonAnalys
{
       
    public class CommonDecidionGraphic:DecisionGraphic
    {
        public CommonDecidionGraphic(Dictionary<double, CommonScript> commonScripts):base()
        {
            decidionGraphic = commonScripts.ToDictionary(p=> Math.Round( p.Key), p=>p.Value as DecidionScript);
            best_points = ToPoints(TypeOfGraphic.best);
            bad_points = ToPoints(TypeOfGraphic.bad);
            perish_points = ToPoints(TypeOfGraphic.perishable);
            adviced_points = GetAdvicesPoints();
            decidionGraphic_Serialize = new List<HelpPairDouble_DecScr>();
            foreach (var d in decidionGraphic)
            {
                decidionGraphic_Serialize.Add(new HelpPairDouble_DecScr() { risk = d.Key, DecidionScript = d.Value });
            }
        }
        public CommonDecidionGraphic() { }
               
    }


}
