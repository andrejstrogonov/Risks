using DataProcessLibrary.Analys;
using DataProcessLibrary.CommonAnalys;
using ProjectManagerLibrary.WorkWithProjects;
using SQLiteLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataProcessLibrary
{
    [Serializable]
    public class HierarhyElem_Analys
    {
        public HierarhyElem HierarhyElem { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
             

        [XmlIgnore]
        public AnalysSetuper AnalysSetuper { get; set; }

        public DecisionGraphic GetDecidionGraphic()
        {
            AnalysScript res = null;
            if(HierarhyElem is Session)
                res= AnalysSetuper.Get(HierarhyElem.DateOfChange_History, HierarhyElem.AnalysSettingsSetuper.Get().StartRisk, HierarhyElem.AnalysSettingsSetuper.Get().FinishRisk);
            else
            {
                res = AnalysSetuper.GetCommon(HierarhyElem.DateOfChange_History, HierarhyElem.AnalysSettingsSetuper.Get().StartRisk, HierarhyElem.AnalysSettingsSetuper.Get().FinishRisk, HierarhyElem.GetProject());

            }
            if (res is null)
                return null;
            return res.DecidionGraphic;
        }


        public HierarhyElem_Analys Copy()
        {
            HierarhyElem_Analys h_analys = new HierarhyElem_Analys();
            h_analys.HierarhyElem = HierarhyElem;
            h_analys.AnalysSetuper = AnalysSetuper;
            HierarhyElem.Activate();
            h_analys.AnalysSetuper.Path = HierarhyElem.AnalysPath;
            h_analys.Children = Children;
            h_analys.Children_Analys = Children_Analys;
            return h_analys;
        }

        public HierarhyElem_Analys Find(HierarhyElem hierarhyElem)
        {
            if (HierarhyElem == hierarhyElem)
            {
                return this;
            }
            foreach(var c in Children)
            {
                var res = c.Find(hierarhyElem);
                if(!(res is null))
                {
                    return res;
                }
            }
            return null;
        }

        public List<HierarhyElem_Analys> Children_Analys { get; set; }

        public List<HierarhyElem_Analys> Children { get; set; }

        public void ReloadChildren()
        {
            Children = new List<HierarhyElem_Analys>();
            foreach (var c in HierarhyElem.HierarhyElems)
            {
                Children.Add(new HierarhyElem_Analys(c.Value));
            }
        }

        public HierarhyElem_Analys(HierarhyElem hierarhyElem)
        {
            HierarhyElem = hierarhyElem;
            AnalysSetuper = new AnalysSetuper();
            hierarhyElem.Activate();
            AnalysSetuper.Path = hierarhyElem.AnalysPath;
            SimplexStage.StartRisk = (double)HierarhyElem.GetProject().AnalysSettingsSetuper.Get().StartRisk/100;
            SimplexStage.FinishRisk = (double)HierarhyElem.GetProject().AnalysSettingsSetuper.Get().FinishRisk/100;
            HierarhyElem.OnChange += ReloadChildren;
            ReloadChildren();
        }

        public HierarhyElem_Analys() { }

        public void SetName(int number)
        {
            Number = number;
            Name = HierarhyElem.Name + " " + number.ToString();
        }

        public DecisionGraphic Analys()
        {
            AnalysSetuper.Path = HierarhyElem.AnalysPath;
            SimplexStage.StartRisk = (double)HierarhyElem.GetProject().AnalysSettingsSetuper.Get().StartRisk/100;
            SimplexStage.FinishRisk = (double)HierarhyElem.GetProject().AnalysSettingsSetuper.Get().FinishRisk/100;
            AnalysScript analysScript=null;
            if(HierarhyElem is Session)
                analysScript = AnalysSetuper.Get(HierarhyElem.DateOfChange_History, HierarhyElem.AnalysSettingsSetuper.Get().StartRisk, HierarhyElem.AnalysSettingsSetuper.Get().FinishRisk);
            else
                analysScript = AnalysSetuper.GetCommon(HierarhyElem.DateOfChange_History, HierarhyElem.AnalysSettingsSetuper.Get().StartRisk, HierarhyElem.AnalysSettingsSetuper.Get().FinishRisk, HierarhyElem.GetProject());

            if (analysScript is null || analysScript.DecidionGraphic is null)
            {                
                DecisionGraphic decidionGraphic=null;
                if (HierarhyElem is Session)
                {
               
                    HierarhyElem.Activate();
                    Analys.Analys analys = new Analys.Analys();
                    try
                    {
                        decidionGraphic = analys.FindDecidion();                        
                    }
                    catch(Exception e)
                    {
                        AnalysSetuper.SaveNew(null, HierarhyElem.GetProject());
                        throw e;
                    }
                }  
                else
                {
                    try
                    {
                        var alldecidions = SingleDecidionStage.GetAllGraphics(Children_Analys);
                        decidionGraphic= GetCommonDecidionGraphic(alldecidions);
                    }
                    catch(Exception e)
                    {
                        AnalysSetuper.SaveNew(null, HierarhyElem.GetProject());
                        throw e;
                    }
                }
                AnalysSetuper.SaveNew(decidionGraphic, HierarhyElem.GetProject());
                return decidionGraphic;
            }
            else
            {
                return analysScript.DecidionGraphic;
            }            
        }

        private CommonDecidionGraphic GetCommonDecidionGraphic(Dictionary<HierarhyElem_Analys, DecisionGraphic> alldecidions)
        {
            if (alldecidions.Count() > 0)
            {
                //PrintDecidions(alldecidions);
                alldecidions = PrecountStage.PrecountAllDecidions(alldecidions);
                GenerateStage.NeedAllPoints = alldecidions.Count() <= 3;
                var allcombinations = GenerateStage.GetCombinations(alldecidions);
                return CreatingGraphicStage.GetCommonDecidionGraphic(alldecidions, allcombinations);
            }
            return null;
        }

        private CommonDecidionGraphic GetCommonDecidionGraphic(Dictionary<HierarhyElem_Analys, CommonDecidionGraphic> alldecidions)
        {
            if (alldecidions.Count() > 0)
            {
                //PrintDecidions(alldecidions);
                GenerateStage.NeedAllPoints = alldecidions.Count() <= 3;
                var allcombinations = GenerateStage.GetCombinations_Common(alldecidions);
                return CreatingGraphicStage.GetCommonDecidionGraphic(alldecidions.ToDictionary(p => p.Key, p => p.Value as DecisionGraphic), allcombinations);
            }
            return null;
        }
        
        public void LoadFromCommonAnalysSettings(SerializeHierarhy serializeHierarhy)
        {
            if (serializeHierarhy is null)
                return;
            Children_Analys = new List<HierarhyElem_Analys>();
            foreach (var c in Children)
            {
                if (serializeHierarhy.children.Any(p => p.Name == c.HierarhyElem.Name))
                {
                    var equal = serializeHierarhy.children.Where(p => p.Name == c.HierarhyElem.Name).First();
                    c.LoadFromCommonAnalysSettings(equal);
                    for (int i = 1; i <= equal.Amount; i++)
                    {
                        var copy = c.Copy();
                        copy.SetName(i);
                        Children_Analys.Add(copy);
                    }
                }
            }
            
        }
    }
}
