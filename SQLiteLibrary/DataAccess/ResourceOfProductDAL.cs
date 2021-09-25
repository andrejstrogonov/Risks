using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;

namespace SQLiteLibrary.DataAccess
{
    public static class ResourceOfProductDAL
    {
        public static List<ResourceOfProduct> GetResourcesOfProducts()
        {
            using (SampleContext db = new SampleContext())
            {
                return db.ResourcesOfProductsNullable.Select(r => new ResourceOfProduct(r)).ToList();
            }
        }

        public static void AddResourceOfProduct(ResourceOfProduct resourceOfProduct)
        {
            using (SampleContext db = new SampleContext())
            {
                
                if (db.ResourcesOfProductsNullable.Any(rp => rp.ID == resourceOfProduct.ID))
                {
                    db.ResourcesOfProductsNullable.Update(resourceOfProduct.ResourceOfProductNullable);
                }
                else
                    db.ResourcesOfProductsNullable.Add(resourceOfProduct.ResourceOfProductNullable);
                db.SaveChanges();
            }
        }

        public static void AddResourceOfProduct(ResourceOfProductNullable resourceOfProduct)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesOfProductsNullable.Any(rp => rp.ID == resourceOfProduct.ID))
                {
                    db.ResourcesOfProductsNullable.Update(resourceOfProduct);
                }
                else
                    db.ResourcesOfProductsNullable.Add(resourceOfProduct);
                db.SaveChanges();
            }
        }

        public static void DeleteResources(Product p)
        {
            using (SampleContext db = new SampleContext())
            {
                var query = db.ResourcesOfProductsNullable.Where(res => res.ID_Product == p.ID);
                while(query.Count()>0)
                {
                    var q = query.First();
                    RemoveResourceOfProduct(new ResourceOfProduct(q));
                    query = db.ResourcesOfProductsNullable.Where(res => res.ID_Product == p.ID);
                }
                db.SaveChanges();
            }
        }

        public static ObservableCollection<ResourceOfProduct> GetAllResourcesOfProduct(Product p)
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<ResourceOfProduct> result = new ObservableCollection<ResourceOfProduct>();
                var resources = db.ResourcesOfProductsNullable.Where(rp => rp.ID_Product == p.ID).Select(rp => new ResourceOfProduct(rp));               
                foreach(var r in resources)
                {
                    result.Add(r);
                }
                return result;
            }
        }

        public static ObservableCollection<ResourceOfProductNullable> GetAllResourcesOfProduct(ProductNullable p)
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<ResourceOfProductNullable> result = new ObservableCollection<ResourceOfProductNullable>();
                var resources = db.ResourcesOfProductsNullable.Where(rp => rp.ID_Product == p.ID);
                foreach (var r in resources)
                {
                    result.Add(r);
                }
                return result;
            }
        }

        public static List<ResourceOfProduct> GetAllResourcesOfProductWR(Product p)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                List<ResourceOfProduct> result = new List<ResourceOfProduct>();
                foreach (var rop in db.ResourcesOfProductsNullable.Where(rp=>rp.ID_Product==p.ID).Select(r => new ResourceOfProduct(r) {Product=p, Resource = ResourceDAL.GetResource(r.ID_Resource), UnitOfResource = UnitDAL.GetUnit((long)r.UnitOfResourceID) }))
                {
                    result.Add(rop);
                }
                //SampleContext.Mutex.ReleaseMutex();
                SampleContext.IsBusy = false;

                return result;
            }
        }

        public static List<ResourceOfProductNullable> GetAllResourcesOfProductWR(ProductNullable p)
        {
            using (SampleContext db = new SampleContext())
            {
                List<ResourceOfProductNullable> result = new List<ResourceOfProductNullable>();
                foreach (var rop in db.ResourcesOfProductsNullable.Where(rp=>rp.ID_Product==p.ID))
                {
                        rop.ProductNullable = p;
                        rop.ResourceNullable = ResourceDAL.GetResourceNullable(rop.ID_Resource);
                        rop.UnitOfResource = UnitDAL.GetUnit((long)rop.UnitOfResourceID);
                        result.Add(rop);
                }
                return result;
            }
        }

        public static List<ResourceOfProduct> GetAllProductsContainsResource(Resource r)
        {
            using (SampleContext db = new SampleContext())
            {
                List<ResourceOfProduct> result = new List<ResourceOfProduct>();
                result = db.ResourcesOfProductsNullable.Where(rp => rp.ID_Resource == r.ID).Select(rp => new ResourceOfProduct(rp)).ToList();
                return result;
            }
        }

        public static bool RemoveResourceOfProduct(ResourceOfProduct rop)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesOfProductsNullable.Any<ResourceOfProductNullable>(rp => rp.ID == rop.ID))
                {
                    foreach (var pr in db.ResourcesOfProductsNullable.Where(rp => rp.ID == rop.ID))
                        db.ResourcesOfProductsNullable.Remove(pr);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool RemoveResourceOfProduct(ResourceOfProductNullable rop)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesOfProductsNullable.Any<ResourceOfProductNullable>(p => p.ID==rop.ID))
                {
                    foreach (var pr in db.ResourcesOfProductsNullable.Where(rp => rp.ID == rop.ID))
                        db.ResourcesOfProductsNullable.Remove(pr);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static void ClearAll()
        {
            using (SampleContext db = new SampleContext())
            {
                db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable);
                db.SaveChanges();
            }
        }


        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.ResourcesOfProductsNullable.Any(p => p.UnitOfResourceID == id));
            }
        }

        public static ResourceOfProduct TransferToCommon(ResourceOfProduct resourceOfProduct)
        {
            resourceOfProduct.AmountOfResource = UnitDAL.TranslateToCommonUnit((long)resourceOfProduct.UnitOfResourceID, (double)resourceOfProduct.AmountOfResource);
            return resourceOfProduct;
        }

        public static ResourceOfProduct TransferFromCommon(ResourceOfProduct resourceOfProduct)
        {
            resourceOfProduct.AmountOfResource = UnitDAL.TranslateFromCommonUnit((long)resourceOfProduct.UnitOfResourceID, (double)resourceOfProduct.AmountOfResource);
            return resourceOfProduct;
        }

        public static ResourceOfProductNullable TransferToCommon(ResourceOfProductNullable resourceOfProduct)
        {
            resourceOfProduct.AmountOfResource = UnitDAL.TranslateToCommonUnit((long)resourceOfProduct.UnitOfResourceID, resourceOfProduct.AmountOfResource.HasValue ? resourceOfProduct.AmountOfResource.Value:0);
            return resourceOfProduct;
        }
        public static ResourceOfProductNullable TransferFromCommon(ResourceOfProductNullable resourceOfProduct)
        {
            resourceOfProduct.AmountOfResource = UnitDAL.TranslateFromCommonUnit((long)resourceOfProduct.UnitOfResourceID, (double)resourceOfProduct.AmountOfResource);
            return resourceOfProduct;
        }

        public static ResourceOfProductNullable GetResourceOfProduct(long ID)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;

            using (SampleContext db = new SampleContext())
            {
               
                    var res  = db.ResourcesOfProductsNullable.Where( rop =>rop.ID == ID).First();
                //SampleContext.Mutex.ReleaseMutex();
                SampleContext.IsBusy = false;
                return res;
            }
        }

        public static List<ResourceOfProductNullable> GetResourceOfProduct(long ID_Product, long ID_Resource)
        {
            using (SampleContext db = new SampleContext())
            {
                return db.ResourcesOfProductsNullable.Where(rop => rop.ID_Product == ID_Product && rop.ID_Resource == ID_Resource).ToList();
            }
        }

        /*public static double GetAVCOfProduct(Product p)
        {
            List<ResourceOfProduct> resources = GetAllResourcesOfProductWR(p).ToList();
            double sum = 0;
            foreach(var r in resources)
            {
                var r1 = TransferToCommon(r);
                r1.Resource = ResourceDAL.TransferToCommon(r1.Resource);
                sum += r1.AmountOfResource * r1.Resource.Price;
            }
            sum += ProductDAL.TransferToCommon(p).OtherCosts;
            return sum;
        }*/
    }
}
