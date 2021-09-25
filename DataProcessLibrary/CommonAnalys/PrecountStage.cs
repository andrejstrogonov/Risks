using ProjectManagerLibrary.WorkWithProjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.CommonAnalys
{
    public static class PrecountStage
    {
        public static DecisionGraphic PrecountDecidion(DecisionGraphic decidionGraphic)
        {
            if (decidionGraphic is null)
                return null;
            decidionGraphic.adviced_points = PrecountMiddlePoints(decidionGraphic.adviced_points);
            return decidionGraphic;
        }

        private static ObservableCollection<Point> PrecountMiddlePoints(ObservableCollection<Point> middlePoints)
        {
            if (middlePoints.Count == 0)
                return middlePoints;

            var list = middlePoints.ToList();
            list.Sort((Point p1, Point p2) =>
            {
                if (p1.Y > p2.Y)
                    return -1;
                if (p1.Y < p2.Y)
                    return 1;
                if (p1.X < p2.X)
                    return -1;
                return 1;
            });
            bool isOk = false;
            while (!isOk)
            {
                isOk = true;
                double last_x = list[0].X;
                int i = 1;
                for (; i < list.Count(); i++)
                {
                    if (list[i].X < last_x)
                        break;
                    last_x = list[i].X;
                }
                if (i < list.Count())
                {
                    isOk = false;
                    int cnt = 1;
                    for (; (i + cnt < list.Count()) && (list[i + cnt].X < last_x); cnt++)
                        { }
                    list.RemoveRange(i, cnt);
                }
            }
            list.Sort((Point p1, Point p2) =>
            {
                if (p1.X < p2.X)
                    return -1;
                return 1;
            });
            ObservableCollection<Point> result = new ObservableCollection<Point>();
            foreach (var p in list)
                result.Add(p);
            return result;
        }

        public static Dictionary<HierarhyElem_Analys, DecisionGraphic> PrecountAllDecidions(Dictionary<HierarhyElem_Analys, DecisionGraphic> decidions)
        {
            Dictionary<HierarhyElem_Analys, DecisionGraphic> result = new Dictionary<HierarhyElem_Analys, DecisionGraphic>();
            foreach(var d in decidions)
            {
                result[d.Key] = PrecountDecidion(d.Value);
            }
            return result;
        }


    }
}
