using DataProcessLibrary.InitialData;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Flexibility
{
    public class FlexibilityAnalys
    {
        public static double Risk = 0.7;
        public static double FlexProcent = 0.05;
        public double InitialDecidion { get; set; }
        private Dictionary<TypeOfParameter, List<IParameter>> typesOfParameters { get; set; }
        public FlexibilityAnalys()
        {
            StopFromInterface = false;
            InitialData.InitialData.Init();
            Parameter.Risk = Risk;
            Parameter.FlexProcent = FlexProcent;
            typesOfParameters = new Dictionary<TypeOfParameter, List<IParameter>>();
            typesOfParameters[TypeOfParameter.AVCOfResources] = Parameters<Resource, AVCOfResourceFlexibility_Parameter>(ResourceData.Resources().ToList());
            typesOfParameters[TypeOfParameter.MaxAmountOfResource] = Parameters<Resource, MaxAmountOfResourceFlexibility_Parameter>(ResourceData.Resources().ToList());
            typesOfParameters[TypeOfParameter.ResourceOfProductConsumption] = Parameters<ResourceOfProduct, ResourceOfProductConsumpton_Parameter>(ResourceOfProductData.ResourcesOfProducts());
            typesOfParameters[TypeOfParameter.MinimumOfProduct] = Parameters<Product, MinimumOfProductFlexibility_Parameter>(ProductData.Products());
            typesOfParameters[TypeOfParameter.MaximumOfProduct] = Parameters<Product, MaximumOfProductFlexibility_GroupParameter>(ProductData.Products());
            typesOfParameters[TypeOfParameter.GarantOfProduct] = Parameters<Product, GarantAmountOfProductFlexibility_GroupParameter>(ProductData.Products());

            int amount = typesOfParameters[TypeOfParameter.AVCOfResources].Count() +
                         typesOfParameters[TypeOfParameter.MaxAmountOfResource].Count() +
                         typesOfParameters[TypeOfParameter.ResourceOfProductConsumption].Count() +
                         typesOfParameters[TypeOfParameter.MinimumOfProduct].Count() +
                         typesOfParameters[TypeOfParameter.MaximumOfProduct].Count() + 
                         typesOfParameters[TypeOfParameter.GarantOfProduct].Count();
            if(!(OnSetMaximum is null))
            {
                OnSetMaximum(amount + 1);
            }



            var decidion = Parameter.FindDecidion();
            if (!(OnSetProgress is null))
            {
                OnSetProgress();
            }
            if (!StopFromInterface)
            {
                if (!(decidion is null))
                    InitialDecidion = decidion.ProfitIfBest;
                else
                    InitialDecidion = 0;
                Parameter.InitialDecidion = InitialDecidion;

                ProfitFlexibilities = FindFlexibilities();
            }
        }

        public delegate void SetMaximum(int maximum);
        public static SetMaximum OnSetMaximum;

        public delegate void SetCurrentProgress();
        public static SetCurrentProgress OnSetProgress;

        public static bool StopFromInterface { get; set; }
        public List<ProfitFlexibility> ProfitFlexibilities { get; set; }

        private List<IParameter> Parameters<T, S>(List<T> elems) where S:IParameter,new()
        {
            List<IParameter> parameters = new List<IParameter>();
            foreach (var elem in elems)
            {
                var param = new S();
                param.Init(elem);
                parameters.Add(param);
            }
            return parameters;
        }

        private List<ProfitFlexibility> FindFlexibilities()
        {
            List<ProfitFlexibility> flexibilities = new List<ProfitFlexibility>();
            foreach(var top in typesOfParameters)
            {
                foreach(var par in top.Value)
                {
                    if (StopFromInterface)
                    {
                        return flexibilities;
                    }
                    var res = par.FindFlexibility();
                    if (!(OnSetProgress is null))
                    {
                        OnSetProgress();
                    }
                    if (!(res is null) && res.Coefficient_Flexibility>0.0001)
                        flexibilities.Add(res);
                }
            }
            return flexibilities;
        }

    }
}
