using DataProcessLibrary.Data;
using DataProcessLibrary.InitialData;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DataProcessLibrary.Analys
{
    public static class GeneratorDataScriptsStage
    {
        private static List<DataScript> dataScripts;
        private static List<int> number;
        private static AnalysData AnalysData;
        private static List<Product> products;

        public static void GenerateDateScripts(ref AnalysData analysData, double Risk)
        {
            Dictionary<Product, List<RiskOfProduct>> PricesOfEachProduct = new Dictionary<Product, List<RiskOfProduct>>();
            AnalysData = analysData;
            products = ProductData.Products();
            foreach(var p in products)
            {
                PricesOfEachProduct[p] = PreCountStage.PreCount_WithRisk(analysData, analysData.PricesOfEachProduct[p], Risk, analysData.ImportanceOfProducts[(int)p.ID]);
            }
            GenerateDataScripts(ref analysData, PricesOfEachProduct);
        }

        public static  void GenerateDataScripts(ref AnalysData analysData, Dictionary<Product,List<RiskOfProduct>> PricesOfEachProduct)
        {
            var prices_2 = new Dictionary<Product, List<RiskOfProduct>>();
            foreach(var pair in PricesOfEachProduct)
            {
                //prices_2[pair.Key] = pair.Value;
                prices_2[pair.Key] = new List<RiskOfProduct>();
                if (pair.Value.Count >= 1)                
                {
                    prices_2[pair.Key].Add(pair.Value.First());
                    if (pair.Value.Count() >= 2)
                    {
                        prices_2[pair.Key].Add(pair.Value.Last());
                    }
                    if (pair.Value.Count() >= 3 && analysData.ImportanceOfProducts[(int)pair.Key.ID])
                    {
                        int middle = pair.Value.Count() / 2;
                        prices_2[pair.Key].Add(pair.Value[middle]);
                    }
                }
            }
            AnalysData = analysData;
            products = ProductData.Products();
            number = new List<int>();
            foreach (var r in prices_2)
            {
                number.Add(0);
            }       
            dataScripts = new List<DataScript>();
            DataScript.Resources = analysData.Resources;
            List<RiskOfProduct> risk = ConvertToVariable(prices_2);
            dataScripts.Add(new DataScript(risk,GetId_Combination(risk)));
            while (NextStep(prices_2))
            {
                risk = ConvertToVariable(prices_2);
                dataScripts.Add(new DataScript(risk,GetId_Combination(risk)));
            }

            analysData.dataScripts = dataScripts;
        }

        private static string GetId_Combination(List<RiskOfProduct> riskOfProducts)
        {
            string id_combination = "";
            foreach(var r in riskOfProducts)
            {
                id_combination += r.ID.ToString();
            }
            return id_combination;
        }



        private static List<RiskOfProduct> ConvertToVariable(Dictionary<Product, List<RiskOfProduct>> PricesOfEachProduct)
        {
            List<RiskOfProduct> result = new List<RiskOfProduct>();
            for (int i = 0; i < products.Count(); i++)
            {
                RiskOfProduct risk = (PricesOfEachProduct[products[i]])[(int)number[i]];
                result.Add(risk);
            }
            return result;
        }


        private static bool NextStep(Dictionary<Product,List<RiskOfProduct>> PricesOfEachProduct)
        {
            int n = products.Count()- 1;
            while (n >= 0 && number[n] == PricesOfEachProduct[products[n]].Count() - 1)
            {
                number[n] = 0;
                n--;
            }
            if (n < 0)
                return false;
            number[n]++;
            return true;
        }
    }

}
