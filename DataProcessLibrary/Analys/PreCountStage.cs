using DataProcessLibrary.Data;
using DataProcessLibrary.InitialData;
using SimplexPlan;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Analys
{
    public static class PreCountStage
    {
        public static double ImportanceCoeff = 0.15;

        public static Dictionary<int, double> ImportanceOfProducts { get; private set; }

        private static Dictionary<Product, double> test_minimum=new Dictionary<Product, double>();
        private static Dictionary<Product, double> test_maximum=new Dictionary<Product, double>();

        public static void PreCountData(ref AnalysData analysData)
        {
            FirstPreCount(ref analysData);
           // FindGoodRisksOfProduct(ref analysData);



            

        }

        public static int diff(int[,] matr)
        {
            int min = matr[0, 0];
            int max = matr[0, 0];
            for (int i = 0; i < matr.GetLongLength(0); i++)
            {
                for (int j = 0; j < matr.GetLongLength(1); j++)
                {
                    if (matr[i, j] > max)
                    {
                        max = matr[i, j];
                    }
                    if (matr[i, j] < min)
                    {
                        min = matr[i, j];
                    }
                }
            }

            
            return Math.Abs(max - min);
        }
        public static KeyValuePair<bool,string> IsPossible(AnalysData analysData)
        {
            if (ProductData.Products().Count() == 0 ||  RiskOfProductData.PricesOfEachProduct().Count() == 0)
            {
                return new KeyValuePair<bool, string>(false, "Недостаточно информации для анализа!");
            }

            Dictionary<long, double> AmountOfResources = new Dictionary<long, double>();
            foreach (var r in ResourceOfProductData.ResourcesOfProducts())
            {
                if (!AmountOfResources.ContainsKey(r.ID_Resource))
                {
                    AmountOfResources[r.ID_Resource] = 0;
                }
                AmountOfResources[r.ID_Resource] += (r.AmountOfResource)*ProductData.GetProduct(r.ID_Product).Minimum;
            }
            double summ = 0;
            foreach(var r in AmountOfResources)
            {
                summ += r.Value * (ResourceData.GetResource(r.Key).Price);
                if (ResourceData.GetResource(r.Key).Maximum < r.Value)
                    return new KeyValuePair<bool, string>(false,"Недостаточно ресурса "+ResourceData.GetResource(r.Key).Name);
            }
            if (summ > InitialData.InitialData.Settings.Budget)
            {
                return new KeyValuePair<bool, string>(false, "Недостаточно бюджета, нужно минимум - " + summ);
            }

            return new KeyValuePair<bool, string>(true,"");
        }

        private static void PreCount(AnalysData analysData, List<RiskOfProduct> list, bool important = true, double realMax=double.PositiveInfinity)
        {
            double avc = analysData.AVCOfProducts[ProductData.GetProduct(list[0].ID_Product)];
            list.Sort((RiskOfProduct r1, RiskOfProduct r2) =>
            {
                double max1;
                double max2;
                if (important)
                {
                    max1 = Math.Min(r1.GarantedAmount, realMax);
                    max2 = Math.Min(r2.GarantedAmount, realMax);

                }
                else
                {
                    max1 =Math.Min( Math.Min(r1.MaximumAmount, realMax),r1.GarantedAmount*r1.Price/avc);
                    max2 =Math.Min( Math.Min(r2.MaximumAmount, realMax), r2.GarantedAmount*r2.Price/avc);
                }
                if (max1 < max2)
                    return 1;
                if (max1> max2)
                    return -1;
                if (r1.Price < r2.Price)
                    return -1;
                return 1;
            });

            int index = 0;
            
            double maxres = double.NegativeInfinity;
            for (int i = 0; i < list.Count(); i++)
            {
                double max = important ?  Math.Min(list[i].GarantedAmount,realMax) : Math.Min(Math.Min(list[i].MaximumAmount, realMax), list[i].GarantedAmount * list[i].Price / avc);
                if (max * (list[i].Price-avc) > maxres)
                {
                    index = i;
                    maxres = max * (list[i].Price-avc);
                }
            }
            if (index >= 0)
                list.RemoveRange(0, index);
        }

        public static List<RiskOfProduct> PreCount_WithRisk(AnalysData analysData, List<RiskOfProduct> list, double Risk, bool important)
        {
            //if (list.Count() <= 1)
                return list;
            List<RiskOfProduct> result = new List<RiskOfProduct>();
            Product product = ProductData.GetProduct(list[0].ID_Product);
            double avc = analysData.AVCOfProducts[product];
            list.Sort((RiskOfProduct r1, RiskOfProduct r2) =>
            {
                double max1;
                double max2;
                if (important)
                {
                    max1 = r1.GarantedAmount/Risk;
                    max2 = r2.GarantedAmount/Risk;

                }
                else
                {
                    max1 = Math.Min(r1.MaximumAmount,  r1.GarantedAmount * r1.Price / avc/Risk);
                    max2 = Math.Min(r2.MaximumAmount,  r2.GarantedAmount * r2.Price / avc/Risk);
                }
                if (max1 < max2)
                    return 1;
                if (max1 > max2)
                    return -1;
                if (r1.Price < r2.Price)
                    return -1;
                return 1;
            });

            double test_max = test_maximum[product];
            double test_min = test_minimum[product];
            double maxNext = 0;
            for(int i = 0; i < list.Count() - 1; i++)
            {
                double maxCur = important ? list[i].GarantedAmount / Risk : Math.Min(list[i].MaximumAmount, list[i].GarantedAmount * list[i].Price / avc / Risk);
                maxNext= important ? list[i+1].GarantedAmount / Risk : Math.Min(list[i+1].MaximumAmount, list[i+1].GarantedAmount * list[i+1].Price / avc / Risk);
                double minCur = maxNext * (list[i + 1].Price-avc) / (list[i].Price-avc);
                //if (!(minCur-test_max>0.05*minCur || test_min-maxCur>0.05*maxCur))
                if(minCur<maxCur)
                    result.Add(list[i]);
            }
            //if(maxNext>test_min || test_min-maxNext<maxNext*0.05)
                result.Add(list[list.Count() - 1]);


            //return list;
            return result;
        }



        private static void ImproveList(Product p,ref AnalysData analysData, double realMax=double.PositiveInfinity)
        {
            for (int i = 0; i < analysData.PricesOfEachProduct[p].Count(); i++)
            {
                if (analysData.PricesOfEachProduct[p][i].MaximumAmount < analysData.PricesOfEachProduct[p][i].GarantedAmount)
                {
                    analysData.PricesOfEachProduct[p][i].MaximumAmount = analysData.PricesOfEachProduct[p][i].GarantedAmount;
                }
                if (analysData.PricesOfEachProduct[p][i].MaximumAmount < analysData.PricesOfEachProduct[p][i].Product.Minimum)
                {
                    analysData.PricesOfEachProduct[p][i].MaximumAmount = analysData.PricesOfEachProduct[p][i].Product.Minimum;
                }

            }
        }


        private static void ImproveAllLists(ref AnalysData analysData, Dictionary<Product,double> realMaxes=null)
        {
            if(realMaxes is null)
            {
                foreach (var p in analysData.PricesOfEachProduct)
                {
                      ImproveList(p.Key, ref analysData);
                }
                return;
            }
            foreach (var p in analysData.PricesOfEachProduct)
            {
                ImproveList(p.Key,ref analysData, realMaxes[p.Key]);
            }
        }

        private static double SummOfMiddleImportance;

        private static double CountImportance(Product p, AnalysData analysData, double realMax=double.PositiveInfinity)
        {
            double midprice = 0;
            double midmaxam = 0;
            foreach (var pr in analysData.PricesOfEachProduct[p])
            {
                midprice += pr.Price;
                midmaxam +=Math.Min( pr.MaximumAmount,realMax);
            }
            midprice /= analysData.PricesOfEachProduct[p].Count();
            midmaxam /= analysData.PricesOfEachProduct[p].Count();
            return midprice * midmaxam;
        }

        private static double CountSumOfMidImportance(AnalysData analysData, Dictionary<Product,double> realMaxes=null)
        {
            ImportanceOfProducts = new Dictionary<int, double>();
            double res = 0;
            foreach (var p in analysData.PricesOfEachProduct)
            {
                if(!(realMaxes is null))
                    ImportanceOfProducts[(int)p.Key.ID]= CountImportance(p.Key, analysData, realMaxes[p.Key]);
                else

                    ImportanceOfProducts[(int)p.Key.ID] = CountImportance(p.Key, analysData);
                res += ImportanceOfProducts[(int)p.Key.ID];
            }
            return res;
        }
        
        private static List<DataScript> GetTestDataScripts(AnalysData analysData, Product product, int type=2)
        {
            List<DataScript> result = new List<DataScript>();
            List<RiskOfProduct> risks = new List<RiskOfProduct>();
            //DataScript dataScripx;
            Random random = new Random();
            
            int indexproduct = 0;
            foreach (var r in analysData.PricesOfEachProduct)
            {
                int index = 0;
                if (r.Key.ID != product.ID)
                {
                    index = random.Next(0,r.Value.Count());
                }
                    
                else
                {
                    indexproduct = risks.Count();
                }
                risks.Add(r.Value[index]);
            }
            DataScript dataScript = new DataScript(risks);
            result.Add(dataScript);
            for (int ind = 1; ind < analysData.PricesOfEachProduct[product].Count(); ind++)
            {
                List<RiskOfProduct> r2 = new List<RiskOfProduct>(risks);
                r2[indexproduct] = analysData.PricesOfEachProduct[product][ind];
                dataScript = new DataScript(r2);
                result.Add(dataScript);
            }

            for (int ind = 0; ind < analysData.PricesOfEachProduct[product].Count(); ind++)
            {
                risks = new List<RiskOfProduct>();
                foreach (var r in analysData.PricesOfEachProduct)
                {
                    int index = ind;
                    if (r.Key.ID != product.ID)
                        index = 0;
                    if (index >= 0)
                        risks.Add(r.Value[index]);
                }
                dataScript = new DataScript(risks);
                result.Add(dataScript);
            }
            for (int ind = 0; ind < analysData.PricesOfEachProduct[product].Count(); ind++)
            {
                risks = new List<RiskOfProduct>();
                foreach (var r in analysData.PricesOfEachProduct)
                {
                    int index = ind;
                    if (r.Key.ID != product.ID)
                        index = r.Value.Count() - 1;
                    if (index >= 0)
                        risks.Add(r.Value[index]);
                }
                dataScript = new DataScript(risks);
                result.Add(dataScript);
            }

            return result;
        }

        private static void Test_FindBest(ref AnalysData analysData,  Product product)
        {
            var datascripts = GetTestDataScripts(analysData, product);
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            foreach (var d in datascripts)
            {
                var dec = MiddleDecidion(ref analysData, d, SimplexStage.StartRisk);
                if (!(dec is null))
                {
                    var res = dec.Coefficients.ContainsKey((int)product.ID) ? dec.Coefficients[(int)product.ID] : 0;
                    res += dec.Coefficients.ContainsKey((int)-product.ID) ? dec.Coefficients[(int)-product.ID] : 0;
                    if (res > max)
                    {
                        max = res;
                    }
                    if (res < min)
                    {
                        min = res;
                    }
                }
            }
            foreach (var d in datascripts)
            {
                var dec = MiddleDecidion(ref analysData, d, SimplexStage.FinishRisk);
                if (!(dec is null))
                {
                    var res = dec.Coefficients.ContainsKey((int)product.ID) ? dec.Coefficients[(int)product.ID] : 0;
                    res += dec.Coefficients.ContainsKey((int)-product.ID) ? dec.Coefficients[(int)-product.ID] : 0;
                    if (res > max)
                    {
                        res = max;
                    }
                    if (res < min)
                    {
                        min = res;
                    }
                }
            }
            test_maximum[product] = max;
            test_minimum[product] = min;
            /*HashSet<RiskOfProduct> best_risks = new HashSet<RiskOfProduct>();
            double start = SimplexStage.StartRisk;
            double finish = SimplexStage.FinishRisk;
            double step = 0.05;
            var datascripts = GetTestDataScripts(analysData, product);
            while (start <= finish)
            {
                KeyValuePair<double, RiskOfProduct> bestRes = new KeyValuePair<double, RiskOfProduct>();
                foreach(var d in datascripts)
                {
                    var dec = MiddleDecidion(ref analysData, d, start);
                    var riskofproduct = d.PricesOfProducts.Where(p => p.ID_Product == product.ID).First();
                    var maxRes = dec.MaxResult;
                    if (maxRes > bestRes.Key)
                    {
                        bestRes = new KeyValuePair<double, RiskOfProduct>(maxRes, riskofproduct);
                    }
                }
                best_risks.Add(bestRes.Value);
                if (start < finish && start + step > finish)
                    start = finish;
                else
                    start += step;
               
            }*/
            //return best_risks.ToList<RiskOfProduct>();
        }

        private static Decidion MiddleDecidion(ref AnalysData analysData, DataScript dataScript, double risk)
        {
            StoreStage.ProcessWithStock(ref analysData);
            TAXStage.SetTax(ref analysData);
            DataScript.Resources = analysData.Resources;

            analysData.dataScripts = new List<DataScript>();
            analysData.dataScripts.Add(dataScript);

            var dec = SimplexStage.FindDecidionForRisk(risk, analysData);
            if (!(dec is null))
                return dec.decidion;
            return null;
        }

        private static double PosibleMaximumOfProduct(Product product)
        {
            double min = -1;
            foreach(var r in ResourceOfProductData.GetResourcesOfProduct(product))
            {
                double posibleAmountOfProduct = ResourceData.GetResource(r.ID_Resource).Maximum / (r.AmountOfResource);
                if (min == -1 || posibleAmountOfProduct < min)
                    min = posibleAmountOfProduct;
            }
            return min;
        } 

        private static void FirstPreCount(ref AnalysData analysData)
        {
            DataScript.Resources = analysData.Resources;
            foreach (var product in ProductData.Products())
            {
                double newMax = PosibleMaximumOfProduct(product);
                double avc = analysData.AVCOfProducts[product];
                for (int i = 0; i < analysData.PricesOfEachProduct[product].Count(); i++)
                {
                    
                    analysData.PricesOfEachProduct[product][i].MaximumAmount = Math.Min(analysData.PricesOfEachProduct[product][i].MaximumAmount, newMax);
                }
            }
            ImproveAllLists(ref analysData);
            SummOfMiddleImportance = CountSumOfMidImportance(analysData);
            foreach (var product in ProductData.Products())
            {
                double imp = ImportanceOfProducts[(int)product.ID];
                analysData.ImportanceOfProducts[(int)product.ID] = (imp / SummOfMiddleImportance) > ImportanceCoeff;
                PreCount(analysData,analysData.PricesOfEachProduct[product], analysData.ImportanceOfProducts[(int)product.ID]);
            }
        }

        private static void FindGoodRisksOfProduct(ref AnalysData analysData)
        {
            foreach(var product in ProductData.Products())
            {
                Test_FindBest(ref analysData, product);
                //analysData.PricesOfEachProduct[product] = risks;
            }
        }

    }
}
