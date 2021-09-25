using DataProcessLibrary.Data;
using DataProcessLibrary.InitialData;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Analys
{
    public static class StoreStage
    {
        public static void ProcessWithStock(ref AnalysData analysData)
        {
            InitMaximums(analysData);
            
            AVCOfProductsWithSaleFromResourcesStock(ref analysData);
        }

        private static Dictionary<int, double> Maximums;

        private static void InitMaximums(AnalysData analysData)
        {
            Maximums = new Dictionary<int, double>();
            foreach (var p in analysData.PricesOfEachProduct)
            {
                if (analysData.PricesOfEachProduct.ContainsKey(p.Key) && analysData.PricesOfEachProduct[p.Key].Count > 0)
                    Maximums[(int)p.Key.ID] = analysData.ImportanceOfProducts[(int)p.Key.ID] ? analysData.PricesOfEachProduct[p.Key][0].GarantedAmount : analysData.PricesOfEachProduct[p.Key][0].MaximumAmount;
                else
                    Maximums[(int)p.Key.ID] = -1;
            }
        }

        private static Dictionary<int, double> Maximums_Of_Resources()
        {
            var res = new Dictionary<int, double>();
            foreach (var p in ResourceOfProductData.ResourcesOfProducts())
            {
                if (!res.ContainsKey((int)p.ID_Resource))
                    res[(int)p.ID_Resource] = Maximums[(int)p.ID_Product] * p.AmountOfResource;
                else
                    res[(int)p.ID_Resource] += Maximums[(int)p.ID_Product] * p.AmountOfResource;
            }
            return res;
        }

        private static void SetResourcePricesWithSale(ref AnalysData analysData)
        {
            Dictionary<int, double> MaximumsOfResources = Maximums_Of_Resources();
            foreach (var res in analysData.Resources)
            {
                if (MaximumsOfResources.ContainsKey((int)res.ID))
                    analysData.ResoursePriceWithSale[res.ID] = res.Price - res.Price * res.Stock / MaximumsOfResources[(int)res.ID];
                else
                    analysData.ResoursePriceWithSale[res.ID] = res.Price;
            }
        }

        private static void AVCOfProductsWithSaleFromResourcesStock(ref AnalysData analysData)
        {
            SetResourcePricesWithSale(ref analysData);   
            foreach (var p in ProductData.Products())
            {
                List<ResourceOfProduct> rop = ResourceOfProductData.GetResourcesOfProduct(p);
                double sum = 0;
                //добавить перевод единиц измерения
                foreach (var r in rop)
                {
                    sum += analysData.ResoursePriceWithSale[r.ID_Resource] * r.AmountOfResource;
                }
                analysData.AVCOfProductsWithSale[p.ID] = Math.Round( sum,4);
            }

        }

    }
}
