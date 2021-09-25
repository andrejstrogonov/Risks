using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;

namespace SQLiteLibrary.DataAccess
{
    public static class ProductDAL
    {
        public static List<Product> GetProducts()
        {
            using (SampleContext db = new SampleContext())
            {
                var query = from p in db.ProductsNullable select new Product(p);
                return query.ToList();
            }
        }

        public static List<ProductNullable> GetProductsNullable()
        {
            using (SampleContext db = new SampleContext())
            {
                var query = from p in db.ProductsNullable select p;
                return query.ToList();
            }
        }

        public static void AddProduct(Product product)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ProductsNullable.Any<ProductNullable>(p => p.ID == product.ID))
                   db.ProductsNullable.Update(product.ProductNullable);
                else
                    db.ProductsNullable.Add(product.ProductNullable);
                db.SaveChanges();
            }
        }

        public static void AddProduct(ProductNullable product)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ProductsNullable.Any<ProductNullable>(p => p.ID == product.ID))
                    db.ProductsNullable.Update(product);
                else
                    db.ProductsNullable.Add(product);
                db.SaveChanges();
            }
        }

        public static bool DeleteProduct(long ID)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(p => p.ID_Product == ID))
                {
                    db.MemberOfGroupsNullable.RemoveRange(db.MemberOfGroupsNullable.Where(p => p.ID_Product == ID));
                    db.SaveChanges();
                }
                if (db.RiskOfProductsNullable.Any(p => p.ID_Product == ID))
                {
                    db.RiskOfProductsNullable.RemoveRange(db.RiskOfProductsNullable.Where(p => p.ID_Product == ID));
                    db.SaveChanges();
                }
                if (db.ResourcesOfProductsNullable.Any(p => p.ID_Product == ID))
                {
                    db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable.Where(p => p.ID_Product == ID));
                    db.SaveChanges();
                }
                if (db.ProductsNullable.Any<ProductNullable>(p => p.ID == ID))
                {
                    var pr = (from p in db.ProductsNullable where p.ID == ID select p).First();
                    ResourceOfProductDAL.DeleteResources(new Product(pr));
                    db.ProductsNullable.Remove(pr);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool DeleteProduct(Product product)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(p => p.ID_Product == product.ID))
                {
                    db.MemberOfGroupsNullable.RemoveRange(db.MemberOfGroupsNullable.Where(p => p.ID_Product == product.ID));
                    db.SaveChanges();
                }
                if (db.RiskOfProductsNullable.Any(p => p.ID_Product == product.ID))
                {
                    db.RiskOfProductsNullable.RemoveRange(db.RiskOfProductsNullable.Where(p => p.ID_Product == product.ID));
                    db.SaveChanges();
                }
                if (db.ResourcesOfProductsNullable.Any(p => p.ID_Product == product.ID))
                {
                    db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable.Where(p => p.ID_Product == product.ID));
                    db.SaveChanges();
                }
                if(db.ProductsNullable.Any(p=>p.ID==product.ID))
                { db.ProductsNullable.Remove(product.ProductNullable);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool DeleteProduct(ProductNullable product)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(p => p.ID_Product == product.ID))
                {
                    db.MemberOfGroupsNullable.RemoveRange(db.MemberOfGroupsNullable.Where(p => p.ID_Product == product.ID));
                    db.SaveChanges();
                }
                if (db.RiskOfProductsNullable.Any(p => p.ID_Product == product.ID))
                {
                    db.RiskOfProductsNullable.RemoveRange(db.RiskOfProductsNullable.Where(p => p.ID_Product == product.ID));
                    db.SaveChanges();
                }
                if (db.ResourcesOfProductsNullable.Any(p => p.ID_Product == product.ID))
                {
                    db.ResourcesOfProductsNullable.RemoveRange(db.ResourcesOfProductsNullable.Where(p => p.ID_Product == product.ID));
                    db.SaveChanges();
                }
                if (db.ProductsNullable.Any(p => p.ID == product.ID))
                {
                    db.ProductsNullable.Remove(product);
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
                db.ProductsNullable.RemoveRange(db.ProductsNullable);
                db.SaveChanges();
            }
        }


        public static void ClearData()
        {
            using (SampleContext db = new SampleContext())
            {
                List<ProductNullable> products = new List<ProductNullable>();
                foreach (var prod in db.ProductsNullable)
                {
                    prod.IsInteger = null;
                    prod.Minimum = null;
                    prod.Perishable = null;
                    prod.Rate = null;
                    prod.Stock = null;
                    products.Add(prod);
                }
                foreach (var prod in products)
                {
                    db.ProductsNullable.Update(prod);
                }
                db.SaveChanges();
            }
        }


        public static Product GetProduct(long ID)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            Product product = null;
            using (SampleContext db = new SampleContext())
            {
                if (db.ProductsNullable.Any<ProductNullable>(p => p.ID == ID))
                {
                    product= new Product(db.ProductsNullable.Where(p => p.ID == ID).First());
                }

            }
            //SampleContext.Mutex.ReleaseMutex();
            SampleContext.IsBusy = false;
            return product;
        }

        public static ProductNullable GetProductNullable(long ID)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.ProductsNullable.Any<ProductNullable>(p => p.ID == ID))
                {
                    return db.ProductsNullable.Where(p => p.ID == ID).First();
                }
                return null;
            }

        }

        public static List<int> ProductsID()
        {
            List<int> res = new List<int>();
            using (SampleContext db = new SampleContext())
            {
                var query = from pr in db.ProductsNullable
                            select pr;
                foreach (var p in query)
                {
                    res.Add((int)p.ID);
                }
                res.Sort();
                return res;
            }
        }

        public static List<Product> GetProductsNotEmpty()
        {
            using (SampleContext db = new SampleContext())
            {
                var query = db.ProductsNullable.Where(p=> db.RiskOfProductsNullable.Any(rp=>rp.ID_Product==p.ID)).Select(p=>new Product(p));
                return query.ToList();
            }
        }

        public static Product TransferToCommon(Product product)
        {
            if (product.OtherCosts > 0)
                product.OtherCosts = UnitDAL.TranslateToCommonUnit((long)product.UnitOfOtherID, (double)product.OtherCosts);
            return product;
        }

        public static Product TransferFromCommon(Product product)
        {
             if (product.OtherCosts > 0)
                product.OtherCosts = UnitDAL.TranslateFromCommonUnit((long)product.UnitOfOtherID, (double)product.OtherCosts);
            return product;
        }

        public static ProductNullable TransferToCommon(ProductNullable product)
        {
            if (product.OtherCosts > 0)
                product.OtherCosts = UnitDAL.TranslateToCommonUnit((long)product.UnitOfOtherID, (double)product.OtherCosts);
            return product;
        }

        public static ProductNullable TransferFromCommon(ProductNullable product)
        {
            if (product.OtherCosts > 0)
                product.OtherCosts = UnitDAL.TranslateFromCommonUnit((long)product.UnitOfOtherID, (double)product.OtherCosts);
            return product;
        }


        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.ProductsNullable.Any(p => p.UnitOfMaximumID == id || p.UnitOfMimimumID==id || p.UnitOfOtherID==id || p.UnitOfStockID==id));
            }
        }

        public static double GetAVCOfProduct(Product p)
        {
            List<ResourceOfProduct> resources = ResourceOfProductDAL.GetAllResourcesOfProductWR(p).ToList();
            double sum = 0;
            foreach (var r in resources)
            {
                var r1 = ResourceOfProductDAL.TransferToCommon(r);
                r1.Resource = ResourceDAL.TransferToCommon(r1.Resource);
                sum += (double)r1.AmountOfResource * (double)r1.Resource.Price;
            }
            sum += (double)ProductDAL.TransferToCommon(p).OtherCosts;
            return sum;
        }

        public static double GetAVCOfProduct(ProductNullable pn)
        {
            Product p = new Product(pn);
            List<ResourceOfProduct> resources = ResourceOfProductDAL.GetAllResourcesOfProductWR(p).ToList();
            double sum = 0;
            foreach (var r in resources)
            {
                var r1 = ResourceOfProductDAL.TransferToCommon(r);
                r1.Resource = ResourceDAL.TransferToCommon(r1.Resource);
                sum += (double)r1.AmountOfResource * (double)r1.Resource.Price;
            }
            sum += (double)ProductDAL.TransferToCommon(p).OtherCosts;
            return sum;
        }

        public static Dictionary<Resource, double> GetPrices(ProductNullable pn)
        {
            Dictionary<Resource, double> result = new Dictionary<Resource, double>();
            Product p = new Product(pn);
            List<ResourceOfProduct> resources = ResourceOfProductDAL.GetAllResourcesOfProductWR(p).ToList();
            foreach (var r in resources)
            {
                var r1 = ResourceOfProductDAL.TransferToCommon(r);
                r1.Resource = ResourceDAL.TransferToCommon(r1.Resource);
                result[r1.Resource]= (double)r1.AmountOfResource * (double)r1.Resource.Price;
            }
            return result;
        }

        /*public static double GetAVCOfProduct(ProductNullable p)
        {
            List<ResourceOfProductNullable> resources = ResourceOfProductDAL.GetAllResourcesOfProductWR(p).ToList();
            double sum = 0;
            foreach (var r in resources)
            {
                var r1 = ResourceOfProductDAL.TransferToCommon(r);
                r1.ResourceNullable = ResourceDAL.TransferToCommon(r1.ResourceNullable);
                sum += (double)r1.AmountOfResource * (double)r1.ResourceNullable.Price;
            }
            sum += (double)ProductDAL.TransferToCommon(p).OtherCosts;
            return sum;
        }
        /*

               public static Product TransferToCommon(Product product)
               {
                  if (product.Minimum > 0)
                       product.Minimum = UnitDAL.TranslateToCommonUnit(product.UnitOfMimimumID, product.Minimum);
                   if (product.Stock > 0)
                       product.Stock = UnitDAL.TranslateToCommonUnit(product.UnitOfStockID, product.Stock);
                   return product;
               }

               public static Product TransferFromCommon(Product product)
               {
                   if (product.Minimum > 0)
                       product.Minimum = UnitDAL.TranslateFromCommonUnit(product.UnitOfMimimumID, product.Minimum);
                   if (product.Stock > 0)
                       product.Stock = UnitDAL.TranslateFromCommonUnit(product.UnitOfStockID, product.Stock);
                   return product;
               }*/
    }
}
