using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.InitialData
{
    public static class RiskOfProductData
    {

        private static Dictionary<Product, List<RiskOfProduct>> pricesOfProducts;

        public static void Init()
        {
            //products
            pricesOfProducts = new Dictionary<Product, List<RiskOfProduct>>();
            foreach (var v in ProductData.Products())
            {
                pricesOfProducts[v] = new List<RiskOfProduct>();
                foreach(var r in RiskOfProductDAL.GetPricesOfProduct(v).ToList())
                {
                        r.Product = ProductData.GetProduct(v.ID);
                        pricesOfProducts[v].Add(RiskOfProductDAL.TransferToCommon(r));
                }
            }
        }

        public static Dictionary<Product, List<RiskOfProduct>> PricesOfEachProduct()
        {
            return pricesOfProducts;
        }

        public static List<RiskOfProduct> GetPricesAndAmountsOfProduct(Product p)
        {
            return pricesOfProducts[p];
        }

        public static void SetPriceOfProduct(RiskOfProduct riskOfProduct)
        {
            Product key = null;
            foreach(var k in pricesOfProducts)
            {
                if (k.Key.ID == riskOfProduct.ID_Product)
                {
                    key = k.Key;
                    break;
                }
            }
            for(int i=0;i<pricesOfProducts[key].Count();i++)
            {
                if (pricesOfProducts[key][i].Price == riskOfProduct.Price)
                {
                    pricesOfProducts[key][i].MaximumAmount = riskOfProduct.MaximumAmount;
                    pricesOfProducts[key][i].GarantedAmount = riskOfProduct.GarantedAmount;
                    break;
                }
            }
        }
    }
}
