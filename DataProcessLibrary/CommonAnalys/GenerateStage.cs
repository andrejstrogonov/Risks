using ProjectManagerLibrary.WorkWithProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.CommonAnalys
{
    public class Combination
    {
        public Point Point { get; set; }
        public Dictionary<HierarhyElem_Analys, Point> Points { get; set; }

        public Combination(Dictionary<HierarhyElem_Analys,Point> points)
        {
            Points = points;
            Point = GetCommonPoint();
        }

        private Point GetCommonPoint()
        {
            Point point = new Point();
            point.X = 0;
            //double risk = 100;
            foreach(var p in Points)
            {
                point.Y += p.Value.Y;
                point.X += (p.Value.X / 100) * p.Value.Y;
                //if (p.Value.X < risk)
                  //risk = p.Value.X;
                  //if(p.Value.X<point.X)
               //point.X = p.Value.X;
            }
            //point.X = risk;
            point.X /= point.Y;
            point.X = Math.Min(100, Math.Round(point.X*100));
            return point;
        }
    }
    public static class GenerateStage
    {
        public static bool NeedAllPoints { get; set; }

        public static List<Combination> GetCombinations(Dictionary<HierarhyElem_Analys, DecisionGraphic> pointDict)
        {
            int[] numbers = new int[pointDict.Count];
            int[] counts = new int[pointDict.Count];
            HierarhyElem_Analys[] sessions = new HierarhyElem_Analys[pointDict.Count];
            int i = 0;
            foreach(var p in pointDict)
            {
                sessions[i] = p.Key;
                counts[i] =NeedAllPoints? p.Value.best_points.Count():  p.Value.adviced_points.Count();
                numbers[i] = counts[i] > 0 ? 0:-1;
                i++;
            }

            List<Combination> combinations = new List<Combination>();
            combinations.Add(GetCombination(numbers, sessions, pointDict));
            while (NextStep(numbers, counts))
            {
                Combination combination = GetCombination(numbers, sessions,pointDict);
                combinations.Add(combination);
            }
            return combinations;   
        }

        public static List<Combination> GetCombinations_Common(Dictionary<HierarhyElem_Analys, CommonDecidionGraphic> pointDict)
        {
            int[] numbers = new int[pointDict.Count];
            int[] counts = new int[pointDict.Count];
            HierarhyElem_Analys[] sessions = new HierarhyElem_Analys[pointDict.Count];
            int i = 0;
            foreach (var p in pointDict)
            {
                sessions[i] = p.Key;
                counts[i] = NeedAllPoints ? p.Value.best_points.Count() : p.Value.adviced_points.Count();
                numbers[i] = counts[i] > 0 ? 0 : -1;
                i++;
            }

            List<Combination> combinations = new List<Combination>();
            combinations.Add(GetCombination(numbers, sessions, pointDict));
            while (NextStep(numbers, counts))
            {
                Combination combination = GetCombination(numbers, sessions, pointDict);
                combinations.Add(combination);
            }
            return combinations;
        }

        private static bool NextStep(int[] numbers, int[] counts)
        {
            int n = numbers.Length - 1;
            while (n >= 0 && (numbers[n] == counts[n] - 1 || counts[n]==0))
            {
                if (numbers[n] >= 0)
                {
                    numbers[n] = 0;
                }
                n--;
            }
            if (n < 0)
                return false;
            numbers[n]++;
            return true;
        }

        private static Combination GetCombination(int[] numbers, HierarhyElem_Analys[] sessions, Dictionary<HierarhyElem_Analys, DecisionGraphic> pointDict)
        {
            Dictionary<HierarhyElem_Analys, Point> points = new Dictionary<HierarhyElem_Analys, Point>();
            for(int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] >= 0)
                {
                    points[sessions[i]] = NeedAllPoints? pointDict[sessions[i]].best_points[numbers[i]] : pointDict[sessions[i]].adviced_points[numbers[i]];
                }
            }
            return new Combination(points);
        }

        private static Combination GetCombination(int[] numbers, HierarhyElem_Analys[] sessions, Dictionary<HierarhyElem_Analys, CommonDecidionGraphic> pointDict)
        {
            Dictionary<HierarhyElem_Analys, Point> points = new Dictionary<HierarhyElem_Analys, Point>();
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] >= 0)
                {
                    points[sessions[i]] = NeedAllPoints ? pointDict[sessions[i]].best_points[numbers[i]] : pointDict[sessions[i]].adviced_points[numbers[i]];
                }
            }
            return new Combination(points);
        }
    }

}
