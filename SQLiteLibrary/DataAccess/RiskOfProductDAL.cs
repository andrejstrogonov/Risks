using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;

namespace SQLiteLibrary.DataAccess
{
    public static class RiskOfProductDAL
    { 
        public static ObservableCollection<RiskOfProduct> GetPricesOfProduct(Product product)
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<RiskOfProduct> result = new ObservableCollection<RiskOfProduct>();
                foreach (var p in db.RiskOfProductsNullable.Where(r => r.ID_Product == product.ID))
                {
                    //p.Product = product;
                    result.Add(new RiskOfProduct(p));
                }
                return result;
            }
        }

        public static ObservableCollection<RiskOfProductNullable> GetPricesOfProduct(ProductNullable product)
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<RiskOfProductNullable> result = new ObservableCollection<RiskOfProductNullable>();
                foreach (var p in db.RiskOfProductsNullable.Where(r => r.ID_Product == product.ID))
                {
                    //p.Product = product;
                    result.Add(p);
                }
                return result;
            }
        }
        public static List<double> GetPricesOfProduct(long Product_ID){
            using (SampleContext db = new SampleContext())
            {
                List<double> result = (from pr in db.RiskOfProductsNullable where pr.ID_Product==Product_ID group pr by pr.Price into g orderby g.Key select (double)g.Key).ToList<double>();
                return result;
            }
        }

        public static RiskOfProduct GetRiskofProduct(long product_id, double price)
        {
            using (SampleContext db = new SampleContext())
            {
                return new RiskOfProduct( db.RiskOfProductsNullable.Where(p => p.Price == price && p.ID_Product == product_id).Select(s => s).First());
            }
        }

        public static RiskOfProduct GetRiskOfProduct(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return new RiskOfProduct(db.RiskOfProductsNullable.Where(p => p.ID==id).Select(s => s).First());
            }
        }

        public static void AddRiskOfProduct(RiskOfProduct rop)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.RiskOfProductsNullable.Any(p => p.ID==rop.ID))
                {
                    db.RiskOfProductsNullable.Update(rop.RiskOfProductNullable);
                }
                else
                    db.RiskOfProductsNullable.Add(rop.RiskOfProductNullable);
                db.SaveChanges();
            }
        }

        public static void AddRiskOfProduct(RiskOfProductNullable rop)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.RiskOfProductsNullable.Any(p => p.ID == rop.ID))
                {
                    db.RiskOfProductsNullable.Update(rop);
                }
                else
                    db.RiskOfProductsNullable.Add(rop);
                db.SaveChanges();
            }
        }

        public static bool DeleteRiskOfProduct(RiskOfProduct rop)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.RiskOfProductsNullable.Any<RiskOfProductNullable>(p => p.ID == rop.ID))
                {
                    foreach (var pr in from p in db.RiskOfProductsNullable where p.ID==rop.ID select p)
                        db.RiskOfProductsNullable.Remove(pr);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool DeleteRiskOfProduct(RiskOfProductNullable rop)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.RiskOfProductsNullable.Any<RiskOfProductNullable>(p => p.ID == rop.ID))
                {
                    foreach (var pr in from p in db.RiskOfProductsNullable where p.ID == rop.ID select p)
                        db.RiskOfProductsNullable.Remove(pr);
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
                db.RiskOfProductsNullable.RemoveRange(db.RiskOfProductsNullable);
                db.SaveChanges();
            }
        }

        public static RiskOfProduct TransferToCommon(RiskOfProduct rop)
        {
            rop.MaximumAmount =TransferOfUnitsDAL.GetTransferOfUnits(rop.UnitOfAmountID, rop.Product.UnitOfStockID)*  rop.MaximumAmount;
            rop.Price = UnitDAL.TranslateToCommonUnit((long)rop.UnitOfPriceID, (double)rop.Price);
            rop.GarantedAmount = TransferOfUnitsDAL.GetTransferOfUnits(rop.UnitOfAmountID, rop.Product.UnitOfStockID) * rop.GarantedAmount;
            return rop;
        }

        public static RiskOfProduct TransferFromCommon(RiskOfProduct rop)
        {
            rop.MaximumAmount = TransferOfUnitsDAL.GetTransferOfUnits(rop.Product.UnitOfStockID, rop.UnitOfAmountID ) * rop.MaximumAmount;
            rop.Price = UnitDAL.TranslateFromCommonUnit((long)rop.UnitOfPriceID, (double)rop.Price);
            rop.GarantedAmount = TransferOfUnitsDAL.GetTransferOfUnits(rop.Product.UnitOfStockID, rop.UnitOfAmountID) * rop.GarantedAmount;
            return rop;
        }

        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.RiskOfProductsNullable.Any(p => p.UnitOfPriceID == id || p.UnitOfAmountID==id));
            }
        }

    }
}
