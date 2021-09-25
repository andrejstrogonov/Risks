using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataProcessLibrary.CommonAnalys;
using System.IO;
using DataProcessLibrary.Analys;
using System.Xml.Serialization;

namespace DataProcessLibrary
{
    [Serializable]
    [XmlInclude(typeof(SingleDecidionGraphic))]
    [XmlInclude (typeof(CommonDecidionGraphic))]
    public class DecisionGraphic
    {
        public enum TypeOfGraphic
        {
            best,
            bad,
            perishable,
            middle
        }

        public class HelpPairDouble_DecScr
        {
            public double risk { get; set; }
            public DecidionScript DecidionScript { get; set; }
            public HelpPairDouble_DecScr() { }
        }

        public List<HelpPairDouble_DecScr> decidionGraphic_Serialize { get; set; }

        [XmlIgnore]
        public Dictionary<double, DecidionScript> decidionGraphic { get; set; }

        //public Dictionary<double, CommonDecidionScript> commonDecidionGraphic;

        public ObservableCollection<Point> best_points { get;  set; }

        public ObservableCollection<Point> bad_points { get;  set; }

        public ObservableCollection<Point> perish_points { get;  set; }

        public ObservableCollection<Point> adviced_points { get; set; }

        public void Deserialize()
        {            
            SetRecomended();
            decidionGraphic = new Dictionary<double, DecidionScript>();
            foreach(var d in decidionGraphic_Serialize)
            {
                d.DecidionScript.Deserialize();
                decidionGraphic[Math.Round(d.risk)] = d.DecidionScript;
            }
        }

        public DecisionGraphic()
        {

        } 

       /* public DecidionGraphic(Dictionary<double,CommonDecidionScript> decidionGraphic)
        {
            this.commonDecidionGraphic = decidionGraphic;
            best_points = ToPoints(TypeOfGraphic.best, commonDecidionGraphic);
            bad_points = ToPoints(TypeOfGraphic.bad,commonDecidionGraphic);
            perish_points = ToPoints(TypeOfGraphic.perishable,commonDecidionGraphic);
            middle_points = ToPoints(TypeOfGraphic.middle, commonDecidionGraphic);
        }
        */
       protected ObservableCollection<Point> ToPoints(TypeOfGraphic type)
       {
            ObservableCollection<Point> points = new ObservableCollection<Point>();
            foreach (var p in decidionGraphic)
            {
                double Y = 0;
                switch (type)
                {
                    case TypeOfGraphic.bad: Y = p.Value.ProfitIfBad;break;
                    case TypeOfGraphic.best: Y = p.Value.ProfitIfBest;break;
                    case TypeOfGraphic.perishable:Y = p.Value.ProfitWithPerishable;break;
                    case TypeOfGraphic.middle:Y = p.Value.ProfitMiddle;break;
                }
                points.Add(new Point(p.Key, Math.Round(Y, 2)));
            }
            return points;
        }
       
        public ObservableCollection<Point> GetAdvicesPoints()
        {
            ObservableCollection<Point> result = new ObservableCollection<Point>();

            double startRisk = decidionGraphic.Min(p => p.Value.RealRisk);
            double finishRisk = decidionGraphic.Max(p => p.Value.RealRisk);
            double realamount = Math.Min(3, decidionGraphic.Count());
            long lenOfInterval = (long)((finishRisk - startRisk + 1) / realamount);
            for (int i = 0; i < realamount-1; i++)
            {
                double min = double.NegativeInfinity;
                Point point = new Point();
                for (long risk = (long)(startRisk) + lenOfInterval * i; risk < (long)(startRisk) + lenOfInterval * (i + 1); risk++)
                {
                    if (decidionGraphic.ContainsKey(risk) && decidionGraphic[risk].ProfitMiddle >= min)
                    {
                        min = decidionGraphic[risk].ProfitMiddle;
                        point.X = risk;
                        point.Y = decidionGraphic[risk].ProfitIfBest;
                    }
                }
                if (min > double.NegativeInfinity)
                    result.Add(point);

            }
            double min1 = double.NegativeInfinity;
            Point point1 = new Point();
            for (long risk = (long)(startRisk) + lenOfInterval * (long)(realamount-1); risk <= (long)(finishRisk); risk++)
            {
                if (decidionGraphic.ContainsKey(risk) && decidionGraphic[risk].ProfitMiddle >= min1)
                {
                    min1 = decidionGraphic[risk].ProfitMiddle;
                    point1.X = risk;
                    point1.Y = decidionGraphic[risk].ProfitIfBest;
                }
            }
            if(min1>double.NegativeInfinity)
                result.Add(point1);
            
            return result;
        }

        public void SetRecomended()
        {
            for (int i = 0; i < best_points.Count(); i++)
            {
                if (adviced_points.Any(p => p.X == best_points[i].X))
                {
                    best_points[i].IsRecomended = true;
                    bad_points[i].IsRecomended = true;
                    perish_points[i].IsRecomended = true;
                }
            }
        }

       /* private ObservableCollection<Point> ToPoints(TypeOfGraphic type, Dictionary<double,CommonDecidionScript> decidionGraph)
        {
            ObservableCollection<Point> points = new ObservableCollection<Point>();
            foreach (var p in decidionGraph)
            {
                double Y = 0;
                switch (type)
                {
                    case TypeOfGraphic.bad: Y = p.Value.ProfitIfBad; break;
                    case TypeOfGraphic.best: Y = p.Value.ProfitIfBest; break;
                    case TypeOfGraphic.perishable: Y = p.Value.ProfitWithPerishable; break;
                    case TypeOfGraphic.middle: Y = p.Value.ProfitMiddle; break;
                }
                points.Add(new Point(p.Key, Math.Round(Y, 2)));
            }
            return points;
        }
        */

    }
}
