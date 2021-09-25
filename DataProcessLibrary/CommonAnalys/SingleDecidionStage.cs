using ProjectManagerLibrary.WorkWithProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.CommonAnalys
{
    public  static class SingleDecidionStage
    {
        public static bool StopFromInterface { get; set; }
        public delegate void SetState(string state);

        public static event SetState OnSetState;
        public static Dictionary<HierarhyElem_Analys,DecisionGraphic> GetAllGraphics(List<HierarhyElem_Analys> sessions)
        {
            Dictionary<HierarhyElem, DecisionGraphic> pairs = new Dictionary<HierarhyElem, DecisionGraphic>();
            Dictionary<HierarhyElem_Analys, DecisionGraphic> result = new Dictionary<HierarhyElem_Analys, DecisionGraphic>();
            Analys.Analys.StartRisk = 0.7;
            if(sessions is null)
            {
                return result;
            }
            foreach(var s in sessions)
            {
                if (StopFromInterface)
                    return result;
                if(!(OnSetState is null))
                {
                    string typeofelem = (s.HierarhyElem is Section) ? "раздела" : "расчета";
                    typeofelem += " " + s.HierarhyElem.Name;
                    OnSetState("Анализ " + typeofelem);
                }
                if (!pairs.ContainsKey(s.HierarhyElem))
                {
                    pairs[s.HierarhyElem] = s.Analys();
                }
                if (!(pairs[s.HierarhyElem] is null))
                    result[s] = pairs[s.HierarhyElem];
            }
            return result;
        }
    }
}
