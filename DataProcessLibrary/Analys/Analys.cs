using DataProcessLibrary.CommonAnalys;
using DataProcessLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Analys
{
    public class Analys
    {
        public static double StartRisk = 0.7;
        private AnalysData AnalysData;

        public double SecToInit { get; set; }
        public double SecToDecide { get; set; }


        public Analys(bool needInit=true)
        {
            AnalysData = new AnalysData(needInit);
        }
     
        public KeyValuePair<bool,string> Prepare()
        {
            var possible = PreCountStage.IsPossible(AnalysData);
            if (possible.Key)
            {
                PreCountStage.PreCountData(ref AnalysData);
                StoreStage.ProcessWithStock(ref AnalysData);
                TAXStage.SetTax(ref AnalysData);
                //GeneratorDataScriptsStage.GenerateDataScripts(ref AnalysData);
               
            }
            return possible;
        }

        public SingleDecidionGraphic FindDecidion()
        {            
            var error = Prepare();
            if (!error.Key)
            {
                throw new Exception(error.Value);
            }
            var time1 = DateTime.Now;
            //var poss = Prepare();
            SecToInit = (DateTime.Now - time1).TotalSeconds;
            var time2 = DateTime.Now;
                var res = SimplexStage.FindDecidions(AnalysData);
                SecToDecide = (DateTime.Now - time2).TotalSeconds;
                return res;
        }

        public SingleDecidionScript FindDecidionForRisk(double Risk)//для эластичности
        {
            return SimplexStage.FlexibilityAnalys( AnalysData, Risk);
        }
    }
}
