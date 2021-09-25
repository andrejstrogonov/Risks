using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.InitialData
{
    public static class ResourceOfProductData
    {
        private static Dictionary<Product, List<ResourceOfProduct>> resourcesOfProducts;
                
        private static List<ResourceOfProduct> resourcesOfProduct;

        private static Dictionary<int,Dictionary<int,double>> resourcesForEachProduct;

        public static void Init()
        {
            //resource, product
            resourcesOfProducts = new Dictionary<Product, List<ResourceOfProduct>>();
            foreach (var pr in ProductData.Products())
            {
                resourcesOfProducts[pr] = new List<ResourceOfProduct>();
                foreach (var v in ResourceOfProductDAL.GetAllResourcesOfProduct(pr).ToList())
                {
                    v.Product = ProductData.GetProduct(v.ID_Product);
                    v.Resource = ResourceData.GetResource(v.ID_Resource);
                    resourcesOfProducts[pr].Add(ResourceOfProductDAL.TransferToCommon(v));
                }
            }
            resourcesOfProduct = new List<ResourceOfProduct>();
            foreach(var rp in resourcesOfProducts)
            {
                foreach (var r in rp.Value)
                    resourcesOfProduct.Add(r);
            }

            InitData();

        }

        internal static void InitData()
        {
            resourcesOfProducts = new Dictionary<Product, List<ResourceOfProduct>>();
            foreach (var pr in ProductData.Products())
            {
                resourcesOfProducts[pr] = new List<ResourceOfProduct>();
                foreach (var v in ResourceOfProductDAL.GetAllResourcesOfProduct(pr).ToList())
                {
                    v.Product = ProductData.GetProduct(v.ID_Product);
                    v.Resource = ResourceData.GetResource(v.ID_Resource);
                    resourcesOfProducts[pr].Add(ResourceOfProductDAL.TransferToCommon(v));
                }
            }

            resourcesForEachProduct = new Dictionary<int, Dictionary<int, double>>();
            foreach (var rop in resourcesOfProduct)
            {
                if (!resourcesForEachProduct.ContainsKey((int)rop.ID_Resource))
                {
                    resourcesForEachProduct[(int)rop.ID_Resource] = new Dictionary<int, double>();
                }
                if (!resourcesForEachProduct[(int)rop.ID_Resource].ContainsKey((int)rop.ID_Product))
                {
                    resourcesForEachProduct[(int)rop.ID_Resource][(int)rop.ID_Product] = 0;
                }
                resourcesForEachProduct[(int)rop.ID_Resource][(int)rop.ID_Product] += rop.AmountOfResource;
            }
            foreach(var res in ResourceData.Resources())
            {
                if (!resourcesForEachProduct.ContainsKey((int)res.ID))
                    resourcesForEachProduct[(int)res.ID] = new Dictionary<int, double>();
            }
        }


        public static List<ResourceOfProduct> ResourcesOfProducts()
        {
            return resourcesOfProduct;
        }

        public static List<ResourceOfProduct> GetResourcesOfProduct(Product p)
        {
            return resourcesOfProducts[p];
        }

        public static Dictionary<int,double> ResourcesForEachProduct(Resource res)
        {
            return resourcesForEachProduct[(int)res.ID];
        }

        public static Dictionary<Product, List<ResourceOfProduct>> ResourcesOfEachProducts()
        {
            return resourcesOfProducts;
        }

        public static void SetResourceOfProduct(ResourceOfProduct resourceOfProduct)
        {
            for(int i = 0; i < resourcesOfProduct.Count; i++)
            {
                if(resourcesOfProduct[i].ID_Product==resourceOfProduct.ID_Product && resourcesOfProduct[i].ID_Resource == resourceOfProduct.ID_Resource)
                {
                    resourcesOfProduct[i].AmountOfResource = resourceOfProduct.AmountOfResource;
                    
                    break;
                }
            }
            InitData();
        }
    }
}
