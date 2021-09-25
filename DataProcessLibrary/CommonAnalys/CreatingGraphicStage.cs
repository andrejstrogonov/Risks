using ProjectManagerLibrary.WorkWithProjects;
using SimplexPlan;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataProcessLibrary.CommonAnalys
{
    [Serializable]
    public class CommonScript:DecidionScript
    {
        public new double MiddleProfit(double Risk)
        {
            return (ProfitIfBest-ProfitIfBad) * (Risk / 100) +ProfitIfBad;
        }

        [XmlIgnore]
        public Dictionary<HierarhyElem_Analys, DecidionScript> Decidions { get; set; }

        public class KeyValuePair_Decidions
        {
            public HierarhyElem_Analys Key { get; set; }
            public DecidionScript Value { get; set; }
            public KeyValuePair_Decidions() { }
        }

        public List<KeyValuePair_Decidions> Decidions_Ser { get; set; }

        public CommonScript(Dictionary<HierarhyElem_Analys, DecisionGraphic> DecidionGraphics, Combination combination):base()
        {
            RealRisk = combination.Point.X;
            ProfitIfBest = combination.Point.Y;
            Decidions = new Dictionary<HierarhyElem_Analys, DecidionScript>();
            foreach(var d in DecidionGraphics)
            {
                Decidions[d.Key] = DecidionGraphics[d.Key].decidionGraphic[Math.Round(combination.Points[d.Key].X)];
            }
            ProfitIfBad = GetBadPoint();
            ProfitWithPerishable = GetPerishPoint();
            delta_VAT = GetDelta_VAT();
            ProfitMiddle = MiddleProfit(combination.Point.X/100);
            BudgetCost = GetBugdetCost();
            Proceeds = GetProceeds();
            Expenses = GetExpenses();
            FixedCosts_Incoming = GetFixedCosts_In();
            FixedCosts_Outcomung = GetFixedCosts_Out();
            ResourcesInUse = GetResourcesInUse();
            ProductsProceeds = GetProductProceeds();

            Decidions_Ser = new List<KeyValuePair_Decidions>();
            foreach(var d in Decidions)
            {
                Decidions_Ser.Add(new KeyValuePair_Decidions() { Key = d.Key, Value = d.Value });
            }
        }

       
        public CommonScript()
        {

        }

        private double GetPerishPoint()
        {
            double res = 0;
            foreach(var s in Decidions)
            {
                res += s.Value.ProfitWithPerishable;
            }
            return res;
        }

        private double GetBadPoint()
        {
            double res = 0;
            foreach (var s in Decidions)
            {
                res += s.Value.ProfitIfBad;
            }
            return res;
        }

        private double GetDelta_VAT()
        {
            double res = 0;
            foreach (var s in Decidions)
            {
                res += s.Value.delta_VAT;
            }
            return res;
        }

        private double GetBugdetCost()
        {
            double res = 0;
            foreach (var s in Decidions)
            {
                res += s.Value.BudgetCost;
            }
            return res;
        }

        private double GetProceeds()
        {
            double res = 0;
            foreach (var s in Decidions)
            {
                res += s.Value.Proceeds;
            }
            return res;
        }

        private double GetExpenses()
        {
            double res = 0;
            foreach (var s in Decidions)
            {
                res += s.Value.Expenses;
            }
            return res;
        }

        private double GetFixedCosts_In()
        {
            double res = 0;
            foreach (var s in Decidions)
            {
                res += s.Value.FixedCosts_Incoming;
            }
            return res;
        }

        private double GetFixedCosts_Out()
        {
                double res = 0;
                foreach (var s in Decidions)
                {
                    res += s.Value.FixedCosts_Outcomung;
                }
                return res;
        }

        private Dictionary<string, ResourceScript> GetResourcesInUse()
        {
            Dictionary<string, ResourceScript> res = new Dictionary<string, ResourceScript>();
            foreach(var s in Decidions)
            {
                foreach(var r in s.Value.ResourcesInUse)
                {
                    if (res.ContainsKey(r.Key))
                    {
                        //подправить
                        res[r.Key] = new ResourceScript(res[r.Key].Resource, res[r.Key].AmountToBuy + r.Value.AmountToBuy, res[r.Key].AmountFromStock + r.Value.AmountFromStock, r.Value.Unit);
                    }
                    else
                    {
                        res[r.Key] = r.Value;
                    }
                }
            }
            return res;
        }

        private Dictionary<string, ProductScript> GetProductProceeds()
        {
            Dictionary<string, ProductScript> res = new Dictionary<string, ProductScript>();
            foreach (var s in Decidions)
            {
                foreach (var r in s.Value.ProductsProceeds)
                {
                    if (res.ContainsKey(r.Key))
                    {
                        res[r.Key].Amount += r.Value.Amount;
                        res[r.Key].Proceeds += r.Value.Proceeds;

                    }
                    else
                    {
                        res[r.Key] = r.Value;
                    }
                }
            }
            return res;
        }


        public override void Deserialize()
        {
            Decidions = new Dictionary<HierarhyElem_Analys, DecidionScript>();
            foreach(var d in Decidions_Ser)
            {
                d.Value.Deserialize();
                Decidions[d.Key] = d.Value;
            }
            ProductsProceeds = GetProductProceeds();
            ResourcesInUse = GetResourcesInUse();
        }
       
    }

    public static class CreatingGraphicStage
    {
        private static ObservableCollection<Combination> CreateGraphic(List<Combination> combinations)
        {
            Dictionary<double, Combination> combsDict = new Dictionary<double, Combination>();
            foreach(var c in combinations)
            {
                Point commonPoint = c.Point;
                if (combsDict.ContainsKey(commonPoint.X))
                {
                    if (combsDict[commonPoint.X].Point.Y < commonPoint.Y)
                        combsDict[commonPoint.X] = c;
                }
                else
                    combsDict[commonPoint.X] = c;
            }
            List<Combination> combs = new List<Combination>();
            foreach(var p in combsDict)
            {
                combs.Add(p.Value);
            }
            combs.Sort((Combination c1, Combination c2) =>
            {
                if (c1.Point.X > c2.Point.X)
                    return 1;
                return -1;
            });
            ObservableCollection<Combination> combsObs = new ObservableCollection<Combination>();
            for (int i = 0; i < combs.Count(); i++)
                combsObs.Add(combs[i]);
            return combsObs;
        }
        public static CommonDecidionGraphic GetCommonDecidionGraphic(Dictionary<HierarhyElem_Analys, DecisionGraphic> decidions, List<Combination> combinations)
        {
            var combs = CreateGraphic(combinations);
            Dictionary<double, CommonScript> commonScripts = new Dictionary<double, CommonScript>();
            for (int i = 0; i < combs.Count(); i++)
            {
                commonScripts[combs[i].Point.X] = new CommonScript(decidions, combs[i]);
            }
            return new CommonDecidionGraphic(commonScripts);
        }

       /* public static CommonDecidionGraphic GetCommonDecidionGraphic(Dictionary<HierarhyElem_Analys, CommonDecidionGraphic> decidions, List<Combination> combinations)
        {
            var combs = CreateGraphic(combinations);
            Dictionary<double, CommonScript> commonScripts = new Dictionary<double, CommonScript>();
            for (int i = 0; i < combs.Count(); i++)
            {
                commonScripts[combs[i].Point.X] = new CommonScript(decidions, combs[i]);
            }
            return new CommonDecidionGraphic(commonScripts);
        }*/

    }
}
