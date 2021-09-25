using DataProcessLibrary.Data;
using DataProcessLibrary.InitialData;
using SimplexPlan;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataProcessLibrary.Analys
{
    [Serializable]
    public class SingleDecidionScript:DecidionScript
    {       
            protected new double MiddleProfit(double Risk)
            {
                return ProfitIfBest * (Risk / 100);
            }

            public Decidion decidion { get;  set; }
            public DataScript dataScript { get;  set; }
            [XmlIgnore]
            public AnalysData AnalysData { get;  set; }

        public SingleDecidionScript()
        {

        }

        public override void Deserialize()
        {
            decidion.Deserialize();
            RealAmountOfResources_ToBuy = new Dictionary<long, double>();
            foreach(var c in HelpRealAmountOfRes_Ser)
            {
                RealAmountOfResources_ToBuy[c.Key] = c.Value;
            }
            ResourcesInUse = new Dictionary<string, ResourceScript>();
            foreach(var c in HelpInUse_Ser)
            {
                ResourcesInUse[c.ResourceScript.Resource.Name] = c.ResourceScript;
            }
            ProductsProceeds = new Dictionary<string, ProductScript>();
            foreach(var pp in ProductsProseeds_Ser)
            {
                ProductsProceeds[pp.Value.Name] = pp.Value;
            }
            //SetProductsProceeds();
        }

        public class ProductsProseeds_HelpSer
        {
            public long Key { get; set; }
            public ProductScript Value { get; set; }
            public ProductsProseeds_HelpSer(long key, ProductScript value)
            {
                Key = key;
                Value = value;
            }
            public ProductsProseeds_HelpSer() { }
        }

        public List<ProductsProseeds_HelpSer> ProductsProseeds_Ser { get; set; }

        public SingleDecidionScript(DataScript ds, Decidion d, AnalysData analysData) : base()
            {
                decidion = d;
                //RoundDecidion(decidion);
                dataScript = ds;
                delta_VAT = 0;
                AnalysData = analysData;
                //Recount();
            }

        public double CountBudgetCost()
        {
            if (ResourcesInUse is null || ResourcesInUse.Count()==0)
                return 0;
            double sum = 0;
            foreach(var r in ResourcesInUse)
            {
                sum += r.Value.AmountToBuy * UnitDAL.TranslateToCommonUnit(r.Value.Resource.UnitOfPriceID, r.Value.Resource.Price);
            }
            foreach(var c in FixedCostDAL.GetFixedCosts(TypeOfFixedCosts.outcoming))
            {
                sum += UnitDAL.TranslateToCommonUnit((long)c.UnitOfPriceID, (long)c.Price);
            }
            return sum;
        }

            public double FindSuperRealRisk()
            {
                double summOfAmounts = 0;
                double summOfGarants = 0;
                foreach (var rop in dataScript.PricesOfProducts)
                {

                    double amount = 0;
                    if (decidion.Coefficients.ContainsKey((int)rop.ID_Product))
                    {
                        amount += decidion.Coefficients[(int)rop.ID_Product];
                    }
                    if (decidion.Coefficients.ContainsKey((int)-rop.ID_Product))
                    {
                        amount += decidion.Coefficients[(int)-rop.ID_Product];
                    }
                    summOfAmounts += amount;
                    summOfGarants += rop.GarantedAmount;
                }
                return Math.Round(summOfGarants / summOfAmounts * 100);
            }
        
            public double FindRealRisk(double Risk)
            {
                double summOfAmounts = 0;
                double summOfGarants = 0;
                foreach (var rop in dataScript.PricesOfProducts)
                {

                    double amount = 0;
                    if (decidion.Coefficients.ContainsKey((int)rop.ID_Product))
                    {
                        amount += decidion.Coefficients[(int)rop.ID_Product];
                    }
                    if (decidion.Coefficients.ContainsKey((int)-rop.ID_Product))
                    {
                        amount += decidion.Coefficients[(int)-rop.ID_Product];
                    }
                    if (amount < rop.GarantedAmount)
                    {
                        amount = rop.GarantedAmount;
                    }
                    if (AnalysData.ImportanceOfProducts[(int)rop.ID_Product])
                    {
                        summOfAmounts += amount;
                    }
                    else
                    {
                        var amount2 = rop.GarantedAmount / Risk;
                        if (amount > amount2)
                            amount = amount2;
                        summOfAmounts += amount;
                    }
                    summOfGarants += rop.GarantedAmount;
                }
                return Math.Round(summOfGarants / summOfAmounts * 100);
            }

            public void RoundDecidion(Decidion d)
            {
                var keys = d.Coefficients.Keys.ToArray();
                foreach (var key in keys)
                {
                    decidion.Coefficients[key] = Math.Round(decidion.Coefficients[key], 6);
                }
            }

            public void Recount()
            {
                RealAmountOfResources_ToBuy = SetRealAmountOfResources_ToBuy();
                SetRealMaxResult(AnalysData);
                SetProductsProceeds();
            }


            public void Init(double Risk)
            {
                Recount();

                ProfitIfBest = ProfitInBestSituation();
                delta_VAT = CountDelta_VAT(ProfitIfBest);
                /*switch (InitialData.InitialData.Settings.VAT_type)
                {
                    case VATType.full: delta_VAT = CountDelta_VAT_FULL(AnalysData); break;
                    case VATType.main_6: delta_VAT = CountDelta_VAT_main_6(AnalysData); break;
                case VATType.None: delta_VAT = CountDelta_VAT_None(AnalysData); break;
                default: delta_VAT = CountDelta_VAT_main_15(AnalysData); break;
                }*/

                ProfitIfBad = ProfitInBadSituation();

                ProfitWithPerishable += ProfitInBadSituationWithPerishable();
            //RealRisk = FindRealRisk(Risk);
                RealRisk = Risk*100;
                ProfitMiddle = MiddleProfit(FindSuperRealRisk());
                //ProfitWithPerishable = MiddleProfit(FindSuperRealRisk());
                dataScript.TransferFromCommon();//возврат в единицы измерения
                BudgetCost = CountBudgetCost();

            HelpRealAmountOfRes_Ser = new List<HelpRealAmountOfRes>();
            foreach(var c in RealAmountOfResources_ToBuy)
            {
                HelpRealAmountOfRes_Ser.Add(new HelpRealAmountOfRes() { Key = c.Key, Value = c.Value });
            }
            HelpInUse_Ser = new List<HelpInUse>();
            foreach(var c in ResourcesInUse)
            {
                HelpInUse_Ser.Add(new HelpInUse() { Key = c.Value.Resource.ID, ResourceScript = c.Value });
            }
            ProductsProseeds_Ser = new List<ProductsProseeds_HelpSer>();
            foreach(var pp in ProductsProceeds)
            {
                ProductsProseeds_Ser.Add(new ProductsProseeds_HelpSer(pp.Value.ID, pp.Value));
            }
            }


            public class HelpRealAmountOfRes
        {
            public long Key { get; set; }
            public double Value { get; set; }
            public HelpRealAmountOfRes() { }
        }

        public List<HelpRealAmountOfRes> HelpRealAmountOfRes_Ser { get; set; }

            private Dictionary<long, double> RealAmountOfResources_ToBuy;

        public class HelpInUse
        {
            public long Key { get; set; }
            public ResourceScript ResourceScript { get; set; }
        }

        public List<HelpInUse> HelpInUse_Ser { get; set; }
                    

            private Dictionary<long, double> SetRealAmountOfResources_ToBuy()
            {
                ProfitWithPerishable = 0;
                Dictionary<long, double> AmountOfResources = new Dictionary<long, double>();
                ResourcesInUse = new Dictionary<string, ResourceScript>();
                foreach (var r in ResourceOfProductData.ResourcesOfProducts())
                {
                    if (AmountOfResources.ContainsKey(r.ID_Resource))
                    {
                        if (decidion.Coefficients.ContainsKey((int)r.ID_Product))
                            AmountOfResources[r.ID_Resource] += decidion.Coefficients[(int)r.ID_Product] * r.AmountOfResource;
                    }
                    else
                    {
                        if (decidion.Coefficients.ContainsKey((int)r.ID_Product))
                            AmountOfResources[r.ID_Resource] = decidion.Coefficients[(int)r.ID_Product] * r.AmountOfResource;
                    }
                }


                foreach (var r in ResourceData.Resources())
                {

                    if (AmountOfResources.ContainsKey(r.ID))
                    {
                        double amount_outcom = UnitDAL.TranslateFromCommonUnit(r.UnitOfMaximumID, AmountOfResources[r.ID]);
                        double stock_incom = UnitDAL.TranslateFromCommonUnit(r.UnitOfPriceID, r.Stock);
                        ResourceScript rs = new ResourceScript();
                        rs.Resource = ResourceDAL.GetResource(r.ID);
                        rs.Unit = UnitDAL.GetUnit(r.UnitOfMaximumID);
                        rs.AmountFromStock = stock_incom;
                        rs.AmountToBuy = amount_outcom;
                        amount_outcom = Math.Max(0, amount_outcom - stock_incom);
                        
                        if (r.IsInteger)
                        {
                            if (Math.Truncate(amount_outcom) < amount_outcom)
                            {
                                if (!r.Perishable)
                                {
                                    ProfitWithPerishable += Math.Truncate(amount_outcom) + 1 - amount_outcom;
                                }
                                amount_outcom = Math.Truncate(amount_outcom) + 1;
                                //AmountOfResources[r.ID] = UnitDAL.TranslateToCommonUnit(r.UnitOfMaximumID, amount_outcom);
                        }

                        }
                        if (rs.AmountToBuy < rs.AmountFromStock)
                        {
                            rs.AmountFromStock = r.IsInteger ? (Math.Round(rs.AmountToBuy) < rs.AmountToBuy ? rs.AmountToBuy + 1 : rs.AmountToBuy) : rs.AmountToBuy;
                            rs.AmountToBuy = 0;
                        }
                        else
                        {
                            rs.AmountToBuy = amount_outcom;
                        }
                        AmountOfResources[r.ID] = UnitDAL.TranslateToCommonUnit(r.UnitOfMaximumID, rs.AmountToBuy);

                    ResourcesInUse[r.Name] = rs;
                    }
                }


            return AmountOfResources;
            }

            public double Costs { get; set; }


        private double CountDelta_VAT(double profitifbest)
        {
            double profit = Proceeds - Expenses + FixedCostData.Summ_Incoming - FixedCostData.Summ_Outcomin_Decrease - FixedCostData.Summ_Outcomin_NotDecrease;
            if(InitialData.InitialData.Settings.VAT_type==VATType.full)
            {
                profit -= FixedCostData.Summ_admort;
            }
            return profit - profitifbest;
        }


        private double CountDelta_VAT_FULL(AnalysData analysData)
            {
                double outputvat = 0;
                double inputvat = 0;
                foreach (var d in dataScript.PricesOfProducts)
                {
                    if (decidion.Coefficients.ContainsKey((int)d.ID_Product))
                        outputvat += analysData.output_VAT[d] * decidion.Coefficients[(int)d.ID_Product];
                if (decidion.Coefficients.ContainsKey((int)-d.ID_Product))
                {
                    outputvat += (analysData.output_VAT[d]) * decidion.Coefficients[(int)-d.ID_Product];
                    inputvat+= -analysData.input_VAT_of_AVC_with_sale[d.ID_Product] * decidion.Coefficients[(int)-d.ID_Product];
                }
                }
                foreach (var res in RealAmountOfResources_ToBuy)
                {
                    inputvat += res.Value * analysData.input_VAT[res.Key];
                }
                double dif = outputvat - inputvat;
                if (dif < 0)
                {
                    //decidion.MaxResult /= analysData.CoefficientOfMainFunction;
                    decidion.MaxResult += dif;
                    dif = 0;
                    //decidion.MaxResult *= analysData.CoefficientOfMainFunction;
                }
                //outputvat += decidion.MaxResult / analysData.CoefficientOfMainFunction - decidion.MaxResult;
                return Math.Round(dif, 2);
            }

            private double CountDelta_VAT_main_6(AnalysData analysData)
            {
                double outputvat = 0;
                foreach (var d in dataScript.PricesOfProducts)
                {
                    if (decidion.Coefficients.ContainsKey((int)d.ID_Product))
                        outputvat += analysData.output_VAT[d] * decidion.Coefficients[(int)d.ID_Product];
                    if (decidion.Coefficients.ContainsKey((int)-d.ID_Product))
                        outputvat += analysData.output_VAT[d] * decidion.Coefficients[(int)-d.ID_Product];
                }
                return Math.Round(outputvat, 2);
            }

        private double CountDelta_VAT_None(AnalysData analysData)
        {            
            return 0;
        }

        private double CountDelta_VAT_main_15(AnalysData analysData)
            {
                return Math.Round(decidion.MaxResult-  decidion.MaxResult * analysData.CoefficientOfMainFunction, 2);
            }

            private void SetProductsProceeds()
            {
            ProductsProceeds = new Dictionary<string, ProductScript>();
            Proceeds = 0;
            foreach (var p in dataScript.PricesOfProducts)
            {
                if (decidion.Coefficients.ContainsKey((int)p.ID_Product))
                {
                    double amount = decidion.Coefficients[(int)p.ID_Product];
                    Proceeds += amount * (p.Price);
                    string product_name= ProductData.GetProduct(p.ID_Product).Name;
                    if (ProductsProceeds.ContainsKey(product_name))
                    {
                        ProductsProceeds[product_name].Amount += amount;
                        ProductsProceeds[product_name].Proceeds += p.Price * amount;

                    }
                    else
                    {
                        ProductsProceeds[product_name] = new ProductScript(p.ID_Product, ProductDAL.GetProduct(p.ID_Product).Name, amount, p.Price * amount);
                    }
                }
                if (decidion.Coefficients.ContainsKey((int)-p.ID_Product))
                {
                    double amount=  decidion.Coefficients[(int)-p.ID_Product];
                    Proceeds += amount * (p.Price);

                    string product_name = ProductData.GetProduct(p.ID_Product).Name;
                    if (ProductsProceeds.ContainsKey(product_name))
                    {
                        ProductsProceeds[product_name].Amount += amount;
                        ProductsProceeds[product_name].Proceeds += p.Price * amount;

                    }
                    else
                    {
                        ProductsProceeds[product_name] = new ProductScript(p.ID_Product, ProductDAL.GetProduct(p.ID_Product).Name, amount, p.Price * amount);
                    }
                }
            }
        }

            private void SetRealMaxResult(AnalysData analysData)
            {
                double realMaxResult = 0;
                //Proceeds = 0;
                foreach (var p in dataScript.PricesOfProducts)
            {                     

                    if (decidion.Coefficients.ContainsKey((int)p.ID_Product))
                    {
                        realMaxResult += decidion.Coefficients[(int)p.ID_Product] * (p.Price - analysData.output_VAT[p]);
                    
                    //Proceeds+= decidion.Coefficients[(int)p.ID_Product] * (p.Price);
                    }
                    if (decidion.Coefficients.ContainsKey((int)-p.ID_Product))
                    {
                        realMaxResult += decidion.Coefficients[(int)-p.ID_Product] * (p.Price - analysData.output_VAT[p] + analysData.input_VAT_of_AVC_with_sale[p.ID_Product]);
                   //Proceeds+= decidion.Coefficients[(int)-p.ID_Product] * (p.Price);

                }
                }
            Expenses = 0;
                double priceOfResources = 0;
                foreach (var r in analysData.Resources)
                {
                    if (RealAmountOfResources_ToBuy.ContainsKey(r.ID))
                    {
                        priceOfResources += RealAmountOfResources_ToBuy[r.ID] * (r.Price - analysData.input_VAT[r.ID]);
                        delta_VAT-= RealAmountOfResources_ToBuy[r.ID] * ( analysData.input_VAT[r.ID]);
                    Expenses +=  RealAmountOfResources_ToBuy[r.ID] * r.Price;
                        Costs += RealAmountOfResources_ToBuy[r.ID] * (r.Price);
                    }
                }
            FixedCosts_Incoming= FixedCostDAL.GetFixedCosts(TypeOfFixedCosts.incoming).Sum(p=>UnitDAL.TranslateToCommonUnit((long)p.UnitOfPriceID, p.Price.HasValue?p.Price.Value:0));
            FixedCosts_Outcomung= FixedCostDAL.GetFixedCosts(TypeOfFixedCosts.outcoming).Sum(p => UnitDAL.TranslateToCommonUnit((long)p.UnitOfPriceID, p.Price.HasValue ? p.Price.Value : 0));
            //UntappedBudget = InitialData.InitialData.Settings.Budget - Costs;
            decidion.MaxResult = Math.Round((realMaxResult - priceOfResources), 2);
            }

            private double ProfitInBestSituation()
            {
                return PrepareWithCosts(decidion.MaxResult);
            }

            private double PrepareWithCosts(double result)
            {
                double res = 0;
                switch (InitialData.InitialData.Settings.VAT_type)
                {
                    case VATType.full: 
                        res = (result + FixedCostData.Summ_Incoming - FixedCostData.Summ_Outcomin_Decrease) * AnalysData.CoefficientOfMainFunction - FixedCostData.Summ_admort - FixedCostData.Summ_Outcomin_NotDecrease;
                        break;
                    case VATType.None:
                        res = result + FixedCostData.Summ_Incoming - FixedCostData.Summ_Outcomin_Decrease  - FixedCostData.Summ_Outcomin_NotDecrease;
                        break;
                    case VATType.main_6:
                        res = result + FixedCostData.Summ_Incoming*0.94 - FixedCostData.Summ_Outcomin_Decrease - FixedCostData.Summ_Outcomin_NotDecrease;
                        break;
                    case VATType.main_15:
                        res = (result + FixedCostData.Summ_Incoming - FixedCostData.Summ_Outcomin_Decrease - FixedCostData.Summ_Outcomin_NotDecrease)*0.85;
                        break;
                }
                 return Math.Round(res, 2);
            }

            private double ProfitInBadSituation()
            {
                double summ = decidion.MaxResult;
                foreach (var p in dataScript.PricesOfProducts)
                {
                    double tmp = 0;
                    if (decidion.Coefficients.ContainsKey((int)p.ID_Product))
                        tmp += decidion.Coefficients[(int)p.ID_Product];
                    if (decidion.Coefficients.ContainsKey((int)-p.ID_Product))
                        tmp += decidion.Coefficients[(int)-p.ID_Product];
                    summ -= Math.Max(0, (tmp - Math.Max(p.GarantedAmount, p.Product.Minimum)) * p.Price);
                }
                return PrepareWithCosts(summ);
            }

            private double ProfitInBadSituationWithPerishable()
            {
                double summ = decidion.MaxResult;
                foreach (var p in dataScript.PricesOfProducts)
                {
                    if (p.Product.Perishable)
                    {
                        double tmp = 0;
                        if (decidion.Coefficients.ContainsKey((int)p.ID_Product))
                            tmp += decidion.Coefficients[(int)p.ID_Product];
                        if (decidion.Coefficients.ContainsKey((int)-p.ID_Product))
                            tmp += decidion.Coefficients[(int)-p.ID_Product];
                        summ -= Math.Max(0, (tmp - Math.Max(p.GarantedAmount, p.Product.Minimum)) * p.Price);
                    }
                }
                return PrepareWithCosts(summ);
            }


        
    }
}
