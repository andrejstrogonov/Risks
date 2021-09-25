using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.InitialData
{
    public static class ResourceData
    {
        private static List<Resource> resources;

        private static Dictionary<long, Resource> resourceDictionary;

        public static void Init()
        {
            resources = new List<Resource>();
            foreach (var r in
                ResourceDAL.GetResources().ToList())
                resources.Add(ResourceDAL.TransferToCommon(r));
        
            resourceDictionary = new Dictionary<long, Resource>();
            foreach (var p in resources)
            {
                resourceDictionary[p.ID] = p;
            }

        }

        public static List<Resource> Resources()
        {
            return resources;
        }

        public static Resource GetResource(long id)
        {
            return resourceDictionary[id];
        }

        public static void SetResource(Resource resource)
        {
            for(int i = 0; i < resources.Count; i++)
            {
                if(resources[i].ID==resource.ID)
                {
                    resources[i].Maximum = resource.Maximum;
                    resources[i].Price = resource.Price;
                    break;
                }
            }
            resourceDictionary[resource.ID].Maximum = resource.Maximum;
            resourceDictionary[resource.ID].Price = resource.Price;
            ResourceOfProductData.Init();
            RiskOfProductData.Init();
        }
    }
}
