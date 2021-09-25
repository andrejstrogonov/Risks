using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System.Collections.Generic;

namespace DataProcessLibrary.InitialData
{
    public static class ProductData
    {
        private static List<int> productKeys;

        private static List<Product> products;

        private static Dictionary<long, Product> productDictionary;

        private static Dictionary<Product, KeyValuePair<double, Unit>> AVCOfProducts;

        public static void Init()
        {
            products =ProductDAL.GetProductsNotEmpty();
            productDictionary = new Dictionary<long, Product>();
            foreach (var p in products)
            {
                productDictionary[p.ID] = p;
            }

            productKeys = new List<int>();
            foreach (var p in products)
            {
                productKeys.Add((int)p.ID);
                if (p.Stock > 0)
                    productKeys.Add(-(int)p.ID);
            }
        }


        public static List<Product> Products()
        {
            return products;
        }

        public static List<int> ProductKeys()
        {
            return productKeys;
        }

         public static Product GetProduct(long id)
        {
            if(productDictionary.ContainsKey(id))
            return productDictionary[id];
            return null;
        }

        public static Dictionary<Product, KeyValuePair<double, Unit>> GetAVCOfProductWithSale(Dictionary<long, double> sale)
        {
            AVCOfProducts = new Dictionary<Product, KeyValuePair<double, Unit>>();
            foreach (var p in products)
            {
                List<ResourceOfProduct> rop = ResourceOfProductData.GetResourcesOfProduct(p);
                double sum = 0;
                long unit_id = 0;
                //добавить перевод единиц измерения
                foreach (var r in rop)
                {
                    sum += (double)r.Resource.Price *(1- sale[r.ID_Resource]) *  (double)r.AmountOfResource;
                }
                unit_id = (long)p.UnitOfStockID;
                AVCOfProducts[p] = new KeyValuePair<double, Unit>(sum, UnitDAL.GetUnit(unit_id));
            }
            return AVCOfProducts;
        }

        public static void SetProduct(Product p)
        {
            for(int i = 0; i < products.Count; i++)
            {
                if (products[i].ID == p.ID)
                {
                    products[i].Minimum = p.Minimum;
                    break;
                }
            }
            productDictionary[p.ID].Minimum = p.Minimum;
            ResourceOfProductData.Init();
            RiskOfProductData.Init();
        }

    }
}
