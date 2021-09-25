using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SQLiteLibrary.Model;

namespace SQLiteLibrary.DataAccess
{
    public static class ResourceDAL
    {

        public static ObservableCollection<Resource> GetResources()
        {
            ObservableCollection<Resource> result = new ObservableCollection<Resource>();
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                foreach (var p in db.ResourcesNullable)
                {
                    result.Add(new Resource(p));
                }

            }
            //SampleContext.Mutex.ReleaseMutex();
            SampleContext.IsBusy = false;
            return result;
        }

        public static ObservableCollection<ResourceNullable> GetResourcesNullable()
        {
            using (SampleContext db = new SampleContext())
            {
                var query = from pr in db.ResourcesNullable
                            select pr;
                ObservableCollection<ResourceNullable> result = new ObservableCollection<ResourceNullable>();
                foreach (var p in query)
                {
                    result.Add(p);
                }
                return result;
            }
        }

        public static bool SetResource(Resource resource)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesNullable.Any(r => r.ID == resource.ID))
                   db.ResourcesNullable.Update(resource.ResourceNullable);
                else
                    db.ResourcesNullable.Add(resource.ResourceNullable);
                db.SaveChanges();
            }
            //SampleContext.Mutex.ReleaseMutex();
            SampleContext.IsBusy = false;
            return true;
        }

        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.ResourcesNullable.Any(p => p.UnitOfPriceID == id || p.UnitOfStockID==id || p.UnitOfMaximumID==id));
            }
        }

        public static bool SetResource(ResourceNullable resource)
        {            
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesNullable.Any(r => r.ID == resource.ID))
                    db.ResourcesNullable.Update(resource);
                else
                    db.ResourcesNullable.Add(resource);
                db.SaveChanges();
            }
            SampleContext.IsBusy = false;
            //SampleContext.Mutex.ReleaseMutex();
            return true;
        }

        public static bool DeleteResource(long ID)
        {
            using (SampleContext db = new SampleContext())
            {
                
                if (db.ResourcesOfProductsNullable.Any(p => p.ID_Resource == ID))
                {
                    db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable.Where(r => r.ID_Resource == ID));
                    db.SaveChanges();
                }

                if (db.ResourcesNullable.Any<ResourceNullable>(p => p.ID == ID))
                {
                    foreach (var pr in from p in db.ResourcesNullable where p.ID == ID select p)
                        db.ResourcesNullable.Remove(pr);
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
                db.ResourcesNullable.RemoveRange(db.ResourcesNullable);
                db.SaveChanges();
            }
        }


        public static void ClearData()
        {
            using (SampleContext db = new SampleContext())
            {
                List<ResourceNullable> resources = new List<ResourceNullable>();
                foreach(var res in db.ResourcesNullable)
                {
                    res.InputTax = null;
                    res.IsInteger = null;
                    res.Maximum = null;
                    res.Perishable = null;
                    res.Price = null;
                    res.Stock = null;
                    resources.Add(res);
                }
                foreach(var res in resources)
                {
                    db.ResourcesNullable.Update(res);
                }
                db.SaveChanges();
            }
        }

        public static bool DeleteResource(Resource resource)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesOfProductsNullable.Any(p => p.ID_Resource == resource.ID))
                {
                    db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable.Where(r => r.ID_Resource == resource.ID));
                    db.SaveChanges();
                }
                if (db.ResourcesNullable.Any(r=>r.ID==resource.ID))
                {
                    db.ResourcesNullable.Remove(resource.ResourceNullable);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        public static bool DeleteResource(ResourceNullable resource)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesOfProductsNullable.Any(p => p.ID_Resource == resource.ID))
                {
                    db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable.Where(r => r.ID_Resource == resource.ID));
                    db.SaveChanges();
                }
                if (db.ResourcesNullable.Any(r=>r.ID==resource.ID))
                {
                    db.ResourcesNullable.Remove(resource);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static Resource GetResource(long ID)
        {
            Resource result = null;
            //SampleContext.Mutex.WaitOne();
            using (SampleContext db = new SampleContext())
            {
                if (db.ResourcesNullable.Any(p => p.ID == ID))
                {
                    var res_n = db.ResourcesNullable.First(p => p.ID == ID);
                    if (!(res_n is null))
                    {
                        result = new Resource(res_n);
                    }
                }
            }
            return result;
        }

        public static ResourceNullable GetResourceNullable(long ID)
        {
            //SampleContext.Mutex.WaitOne();
            using (SampleContext db = new SampleContext())
            {
                    var result = db.ResourcesNullable.First(p=>p.ID==ID);

                //SampleContext.Mutex.ReleaseMutex();
                return result;
            }
        }

        public static Dictionary<Resource, double> GetResourcesWithAmounts(long ID_Product)
        {
            using (SampleContext db = new SampleContext())
            {
                Dictionary<Resource, double> result = new Dictionary<Resource, double>();
                var query = from rp in db.ResourcesOfProductsNullable where rp.ID_Product == ID_Product select rp;
                foreach(var elem in query)
                {
                    result[new Resource(elem.ResourceNullable)] = (double)elem.AmountOfResource;
                }
                return result;
            }
        }

        public static Resource TransferToCommon(Resource resource)
        {
            if (resource.Maximum>0)
                resource.Maximum = UnitDAL.TranslateToCommonUnit((long)resource.UnitOfMaximumID, (double)resource.Maximum);
            if (resource.Stock > 0)
                resource.Stock = UnitDAL.TranslateToCommonUnit((long)resource.UnitOfPriceID, (double)resource.Stock);
            resource.Price = UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfMaximumID, UnitDAL.TranslateToCommonUnit((long)resource.UnitOfPriceID, (double)resource.Price));
            return resource;
        }

        public static Resource TransferFromCommon(Resource resource)
        {
            if (resource.Maximum > 0)
                resource.Maximum = UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfMaximumID, (double)resource.Maximum);
            if (resource.Stock > 0)
                resource.Stock = UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfPriceID, (double)resource.Stock);
            resource.Price = UnitDAL.TranslateToCommonUnit((long)resource.UnitOfMaximumID, UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfPriceID, (double)resource.Price));
            return resource;
        }

        public static ResourceNullable TransferToCommon(ResourceNullable resource)
        {
            if (resource.Maximum > 0)
                resource.Maximum = UnitDAL.TranslateToCommonUnit((long)resource.UnitOfMaximumID, (double)resource.Maximum);
            if (resource.Stock > 0)
                resource.Stock = UnitDAL.TranslateToCommonUnit((long)resource.UnitOfPriceID, (double)resource.Stock);
            resource.Price = UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfMaximumID, UnitDAL.TranslateToCommonUnit((long)resource.UnitOfPriceID, (double)resource.Price));
            return resource;
        }

        public static ResourceNullable TransferFromCommon(ResourceNullable resource)
        {
            if (resource.Maximum > 0)
                resource.Maximum = UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfMaximumID, (double)resource.Maximum);
            if (resource.Stock > 0)
                resource.Stock = UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfPriceID, (double)resource.Stock);
            resource.Price = UnitDAL.TranslateToCommonUnit((long)resource.UnitOfMaximumID, UnitDAL.TranslateFromCommonUnit((long)resource.UnitOfPriceID, (double)resource.Price));
            return resource;
        }
    }
}
