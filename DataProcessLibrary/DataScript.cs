using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using System.Data;
using DataProcessLibrary.InitialData;
using DataProcessLibrary.Data;
using System.Windows.Media;
using SimplexPlan;

namespace DataProcessLibrary
{
    public class DataScript
    {

        public List<RiskOfProduct> PricesOfProducts { get; set; }
        public static List<Resource> Resources { get; set; }
        public double GarantProfit { get; set; }
        public string combination_id { get; set; }


        public DataScript(List<RiskOfProduct> prices, string combination_id="")
        {
            PricesOfProducts = prices;
            this.combination_id = combination_id;
        }

        public DataScript() { }

        private static int N;

        public static SystemOfLimitations CreateSystemTemplate()
        {
            N = 0;
            SystemOfLimitations systemOfLimitations = new SystemOfLimitations(InitialData.ProductData.ProductKeys());
            foreach(var res in Resources)
            {
                var variables = ResourceOfProductData.ResourcesForEachProduct(res);
                Limitation limitation = new Limitation(variables,res.Maximum, false);
                systemOfLimitations.AddLimitation(limitation);
                N++;
            }

            foreach (var p in ProductData.Products())
            {
                if (p.Stock > 0) {
                    systemOfLimitations.SetStockLimitationMoreThan((int)p.ID, -(int)p.ID,  p.Minimum);
                    systemOfLimitations.SetPersonalLimitationLessThan((int)-p.ID, p.Stock);
                }
                else
                    systemOfLimitations.SetPersonalLimitationMoreThan((int)p.ID, p.Minimum);
                if (p.IsInteger)
                { systemOfLimitations.SetVariableIsInteger((int)p.ID);
                    if (p.Stock>0)
                        systemOfLimitations.SetVariableIsInteger(-(int)p.ID);
                }
            }

            foreach(var g in GroupsData.GroupsOfProducts)
            {
                Dictionary<int, double> vars = new Dictionary<int, double>();
                foreach(var p in g.Value)
                {
                    vars[(int)p.ID] = 1;
                    if (p.Stock > 0)
                        vars[-(int)p.ID] = 1;
                }
                systemOfLimitations.AddLimitation(vars, GroupDAL.GetGroup(g.Key).Minimum, true);
                systemOfLimitations.AddLimitation(vars, GroupDAL.GetGroup(g.Key).Maximum, false);
            }

            return systemOfLimitations;
        }

        private void SetPersonalLimitations(ref SystemOfLimitations systemOfLimitations, double Risk, AnalysData analysData)
        {
            int amountOfNotImport=analysData.ImportanceOfProducts.Where(p=>p.Value==false).Count();
            double coefOfRisk = (1 - Risk) / Risk;
            foreach (var p in ProductData.Products())
            {
                var r = GetRiskOfProductByProductID(p.ID);
                var imp = analysData.ImportanceOfProducts[(int)r.ID_Product];
                double max = 0;
                
                if (imp)
                {
                    max = Math.Min(r.GarantedAmount / Risk,r.MaximumAmount);
                }
                else
                {
                    max = Math.Min(r.GarantedAmount / Risk * (r.Price/ analysData.AVCOfProductsWithSale[p.ID]), r.MaximumAmount);
                }
                if (max <= p.Minimum)
                {
                    max = p.Minimum;
                }
                if (p.IsInteger)
                    max = HelpUtils.RoundDouble(max);
                if (p.Stock > 0)
                {
                    systemOfLimitations.SetStockLimitationLessThan((int)p.ID, (int)-p.ID,max);
                    if (p.Stock < max)
                    {
                        systemOfLimitations.SetPersonalLimitationLessThan((int)p.ID,  max -  p.Stock);
                    }
                    else
                    {
                        systemOfLimitations.SetPersonalLimitationLessThan((int)p.ID, 0);
                    }
                }
                else
                    systemOfLimitations.SetPersonalLimitationLessThan((int)p.ID, max);
            }
        }

        private Dictionary<int, double> GetMainFunction(AnalysData analysData)
        {
            double sum = 0;
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (var pop in PricesOfProducts)
            {
                double amount = pop.GarantedAmount;
                result[(int)pop.ID_Product] = pop.Price - analysData.AVCOfProductsWithSale[pop.ID_Product];
                if (InitialData.InitialData.Settings.VAT_type == VATType.full)
                {
                    result[(int)pop.ID_Product] -=analysData.output_VAT[pop]-analysData.input_VAT_of_AVC_with_sale[pop.ID_Product] ;
                }
                if (pop.Product.Stock > 0)
                {
                    result[(int)-pop.ID_Product] = pop.Price;
                    if (InitialData.InitialData.Settings.VAT_type == VATType.full)
                    {
                        result[(int)-pop.ID_Product] -= analysData.output_VAT[pop] - analysData.input_VAT_of_AVC_with_sale[pop.ID_Product];
                    }
                    if (pop.Product.Stock > amount)
                    {
                        sum += pop.Price * amount;
                        amount = 0;
                    }
                    else
                    {
                        sum += pop.Product.Stock * pop.Price;
                        amount-= pop.Product.Stock;
                    }
                }
                sum += result[(int)pop.ID_Product] * amount;
            }
            GarantProfit = Math.Round(sum)+1;
            return result;
        }

        private Dictionary<int,double> GetBudgetFunction(AnalysData analysData)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (var pop in PricesOfProducts)
            {
                result[(int)pop.ID_Product] = analysData.AVCOfProductsWithSale[pop.ID_Product];
            }
            return result;
        }
        
        private Dictionary<int,double> GetRiskFunction(AnalysData analysData)
        {
            double sum = 0;
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (var pop in PricesOfProducts)
            {
                double amount = pop.GarantedAmount;
                result[(int)pop.ID_Product] = pop.Price;
                if (pop.Product.Stock > 0)
                {
                    result[(int)-pop.ID_Product] = pop.Price;
                    if (pop.Product.Stock > amount)
                    {
                        sum += pop.Price * amount;
                        amount = 0;
                    }
                    else
                    {
                        sum += pop.Product.Stock * pop.Price;
                        amount -= pop.Product.Stock;
                    }
                }
                sum += result[(int)pop.ID_Product] * amount;
            }
            GarantProfit = sum;
            return result;
        }

        public void UpdateSystem(ref SystemOfLimitations system,AnalysData analysData, double enableRisk)
        {
            SetPersonalLimitations(ref system, enableRisk, analysData);
            system.AddMainFunction(GetMainFunction(analysData));
            double limit = InitialData.InitialData.Settings.Budget;
            limit -= FixedCostData.Summ_admort;
            limit -= FixedCostData.Summ_Outcomin_Decrease;
            limit -= FixedCostData.Summ_Outcomin_NotDecrease;
            Limitation limitBudget = new Limitation(GetBudgetFunction(analysData), limit, false);
            system.SetBudgetLimitation(limitBudget);
            Limitation limitRisk = new Limitation(GetRiskFunction(analysData), GarantProfit/enableRisk);
            system.SetRiskLimitation(limitRisk);
        }

        private RiskOfProduct GetRiskOfProductByProductID(long id)
        {
            foreach(var p in PricesOfProducts)
            {
                if (p.ID_Product == id)
                    return p;
            }
            return null;
        }

        public void TransferFromCommon()
        {
            for (int i = 0; i < PricesOfProducts.Count(); i++)
                PricesOfProducts[i] = RiskOfProductDAL.TransferFromCommon(PricesOfProducts[i]);
        }
        
    }
}
