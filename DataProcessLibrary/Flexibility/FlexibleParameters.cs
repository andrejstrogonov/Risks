using DataProcessLibrary.Data;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessLibrary.Analys;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary;
using System.Globalization;

namespace DataProcessLibrary.Flexibility
{
    public enum TypeOfParameter
    {
        AVCOfResources,
        ResourceOfProductConsumption,
        MaxAmountOfResource,
        GarantOfProduct,
        MaximumOfProduct,
        MinimumOfProduct,
        Budget,
        SFC,//расходы
        CFI//доходы
    }

    public interface IParameter
    {
        ProfitFlexibility FindFlexibility();
        void Init(object o);
        void Save(ProfitFlexibility profitFlexibility);
        void TransferFromCommon();
        void TransferToCommon();
        double LastMeaning { get; set; }
    }

    public abstract class Parameter:IParameter
    {
        public static double FlexProcent { get; set; }
        public static double Risk { get; set; }
        
        public TypeOfParameter TypeOfParameter { get; set; }
        public Unit UnitSymbol { get; set; }
        public string Name { get; set; }
        public double LastMeaning { get; set; }

        public double FirstMeaning { get; protected set; }
        public static double InitialDecidion { get; set; }
        public double Sign { get; set; }
        public bool IsInteger { get; set; }
        public double Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public abstract void Init(object o);

        public ProfitFlexibility FindFlexibility()
        {
            var FlexiblePair = FindFlexiblePair(LastMeaning);
            //TransferFromCommon();
            return SetFlexibility(FlexiblePair.Key, FlexiblePair.Value);
        }

        public static SingleDecidionScript FindDecidion()
        {
            Analys.Analys analys = new Analys.Analys(false);
            var error = analys.Prepare();
            if (error.Key)
            {
                var result = analys.FindDecidionForRisk(Risk);
                if (result is null)
                    return null;
                //result.Init(Risk);
                return result;
            }
            return null;
        }

        public static DecisionGraphic FindDecidionGraphic()
        {
            Analys.Analys analys = new Analys.Analys(false);
            var error = analys.Prepare();
            if (error.Key)
            {
                var result = analys.FindDecidion();
                if (result is null)
                    return null;
                //result.Init(Risk);
                return result;
            }
            return null;
        }
        
        public static double NewMeaning(double lastMeaning,double Sign,bool IsInteger)
        {
            return Math.Round(lastMeaning * (1 + Sign*FlexProcent),IsInteger?0:3);
        }

        public KeyValuePair<double, double> FindFlexiblePair(double lastMeaning)
        {
            var t = DateTime.Now;
            double newMeaning = NewMeaning(lastMeaning, Sign, IsInteger);
            double decidionNew = InitialDecidion;
            try
            {
                ChangeData(newMeaning);
                var dec = FindDecidion();
                if(!(dec is null))
                    decidionNew = dec.ProfitIfBest;
            }
            finally
            {
                ChangeData(lastMeaning);
            }
            KeyValuePair<double, double> result = new KeyValuePair<double, double>(InitialDecidion, decidionNew);
            var dif = DateTime.Now - t;
            return result;
        }
        
        public ProfitFlexibility SetFlexibility(double last, double next)
        {
            TransferFromCommon();
            ProfitFlexibility profitFlexibility = new ProfitFlexibility(this);
            profitFlexibility.Procent = FlexProcent;
            profitFlexibility.LastMeaning = LastMeaning;
            profitFlexibility.Sign = Sign;
            profitFlexibility.Parameter_Change = Name;
            profitFlexibility.TypeOfParameter = TypeOfParameter;
            profitFlexibility.Profit_Change_Absolut = Math.Round(next - last,2);
            profitFlexibility.Profit_Change_Procent = Math.Round((next - last) / last,6);
            profitFlexibility.Coefficient_Flexibility = Math.Round( profitFlexibility.Profit_Change_Procent / FlexProcent,6);
            profitFlexibility.UnitOfMeaning = UnitSymbol.Symbol;
            TransferToCommon();
            return profitFlexibility;
        }

        public abstract void ChangeData(double newMeaning);
        public abstract void Save();
        public abstract void Save(ProfitFlexibility profitFlexibility);
        public abstract void TransferFromCommon();
        public abstract void TransferToCommon();        
        public HistoryItem HistoryItem { get; protected set; }
        public HistoryItem Logger(TypeOfHistoryItem typeOfItem, long id, string Property, object Last, object New, string Title, string TitleChange)
        {
            HistoryItem historyItem = new HistoryItem();
            historyItem.TypeOfItem = typeOfItem;
            historyItem.Property = Property;
            historyItem.ID = id;
            historyItem.LastValue = Last;
            historyItem.NewValue = New;
            historyItem.DateTime = DateTime.Now;
            historyItem.Title = Title;
            var str = TitleChange.Split('\n');
            var str_res = "";
            if (str.Count() == 2)
            {
                double n1 = 0;
                if (double.TryParse(str[0], out n1))
                {
                    str_res += n1.ToString("N", CultureInfo.CurrentCulture);
                }
                else
                {
                    str_res += str[0];
                }
                str_res += "\n";

                double n2 = 0;
                if (double.TryParse(str[1], out n2))
                {
                    str_res += n2.ToString("N", CultureInfo.CurrentCulture);
                }
                else
                {
                    str_res += str[1];
                }
            }
            else
            {
                str_res = TitleChange;
            }
            historyItem.TitleChange = str_res;
            return historyItem;
        }

    }

    public abstract class GroupParameter:IParameter
    {
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public bool isInteger { get; set; }
        public double LastMeaning { get; set; }
        public  ProfitFlexibility FindFlexibility()
        {
            if (Parameters.Count == 0)
                return null;
            foreach (var p in Parameters)
            {
                p.ChangeData(Parameter.NewMeaning(p.LastMeaning,p.Sign,p.IsInteger));
            }
            var dec = Parameter.FindDecidion();
            double decidion2 = Parameter.InitialDecidion;
            if (!(dec is null))
                decidion2 = dec.ProfitIfBest;

            foreach (var p in Parameters)
            {
                p.ChangeData(p.LastMeaning);
            }
            foreach (var p in Parameters)
            {
                //if(p.Name!=first.Name)
                p.TransferFromCommon();
            }
            var first = Parameters.First();
            ProfitFlexibility profitFlexibility = first.SetFlexibility(Parameter.InitialDecidion, decidion2);
           
            profitFlexibility.Parameter = this;
            profitFlexibility.LastMeaning = first.LastMeaning;
            return profitFlexibility;
        }

        public void ChangeData(double procent)
        {
            foreach (var p in Parameters)
            {
                var newmean = Math.Round( p.LastMeaning + p.LastMeaning * procent*p.Sign,isInteger?0:2);
                p.ChangeData(newmean);
            }
        }

        public void ChangeDataBack()
        {
            foreach (var p in Parameters)
            {
                p.ChangeData(p.FirstMeaning);
            }
        }

        public void Save()
        {
            foreach (var p in Parameters)
            {
                p.Save();
            }
            
        }

        public void Save(ProfitFlexibility profitFlexibility)
        {
            if(Parameters.Count()>0)
                Parameters.First().Save(profitFlexibility);
        }

        public abstract void Init(object o);

        public void TransferFromCommon()
        {
            foreach(var p in Parameters)
            {
                p.TransferFromCommon();
            }
        }

        public  void TransferToCommon()
        {
            foreach (var p in Parameters)
            {
                p.TransferToCommon();
            }
        }
    }

    public class ParametersList
    {
        public Dictionary<Parameter,double> Parameters { get; set; }
        public Dictionary<GroupParameter,double> GroupParameters { get; set; }
        public ParametersList(Dictionary<Parameter,double> parameters, Dictionary<GroupParameter,double> groupParameters)
        {
            Parameters = parameters;
            GroupParameters = groupParameters;
        }

        public void ChangeDataTo()
        {
            foreach (var p in Parameters)
            {
                p.Key.TransferFromCommon();
                p.Key.ChangeData(p.Value);
                p.Key.TransferToCommon();
            }
            foreach (var gp in GroupParameters)
            {
                gp.Key.TransferFromCommon();
                gp.Key.ChangeData(gp.Value);
                gp.Key.TransferToCommon();
            }
        }

        public void ChangeDataBack()
        {
            foreach (var p in Parameters)
            {
                p.Key.TransferFromCommon();
                p.Key.ChangeData(p.Key.LastMeaning);
                p.Key.TransferToCommon();
            }
            foreach (var gp in GroupParameters)
            {
                gp.Key.TransferFromCommon();
                gp.Key.ChangeDataBack();
                gp.Key.TransferToCommon();
            }
        }


       


        public ResultOfChange FindResultOfChange()
        {
            ResultOfChange resultOfChange = new ResultOfChange();

            ChangeDataTo();
            var newdec = Parameter.FindDecidion();
            newdec.Init(Parameter.Risk);
            resultOfChange.LastProfit =Math.Round( Parameter.InitialDecidion,2);
            resultOfChange.NewProfit = newdec.ProfitIfBest;
            resultOfChange.AbsolutChange =Math.Round( resultOfChange.NewProfit - resultOfChange.LastProfit,2);
          
            return resultOfChange;
        }
               
        public DecisionGraphic FindDecidion_WithChanges()
        {
            ChangeDataTo();
            var result= Parameter.FindDecidionGraphic();
            ChangeDataBack();
            return result;
        }

        public DecisionGraphic FindDecidion_WithoutChanges()
        {            
            return Parameter.FindDecidionGraphic();
        }

        public Dictionary<TypeOfHistoryItem,List<HistoryItem>> SaveChanges()
        {
            Dictionary<TypeOfHistoryItem, List<HistoryItem>> result = new Dictionary<TypeOfHistoryItem, List<HistoryItem>>();
            result[TypeOfHistoryItem.Product] = new List<HistoryItem>();
            result[TypeOfHistoryItem.Resource] = new List<HistoryItem>();
            result[TypeOfHistoryItem.ResourceOfProduct] = new List<HistoryItem>();
            result[TypeOfHistoryItem.RiskOfProduct] = new List<HistoryItem>();
            foreach (var p in Parameters)
            {
                p.Key.TransferFromCommon();
                p.Key.ChangeData(p.Value);
                p.Key.Save();
                p.Key.TransferToCommon();
                result[p.Key.HistoryItem.TypeOfItem].Add(p.Key.HistoryItem);
            }
            foreach (var gp in GroupParameters)
            {
                gp.Key.TransferFromCommon();
                gp.Key.ChangeData(gp.Value);
                gp.Key.Save();
                gp.Key.TransferToCommon();
                foreach(var p in gp.Key.Parameters)
                {
                    result[p.HistoryItem.TypeOfItem].Add(p.HistoryItem);
                }
            }
            return result;
        }

        public void SaveChanges(List<ProfitFlexibility> savedfixedCosts)
        {
            foreach(var p in savedfixedCosts)
            {
                p.Save();
            }
        }
    }
}
