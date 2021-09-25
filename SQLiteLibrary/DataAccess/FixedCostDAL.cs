using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteLibrary.DataAccess
{
    public static class FixedCostDAL
    {
        public static FixedCost GetFixedCost(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.FixedCosts.Any(p => p.ID == id)){
                    return db.FixedCosts.Where(p => p.ID == id).First();
                }
                return null;
            }
        }

        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.FixedCosts.Any(p => p.UnitOfPriceID == id));
            }
        }

        public static ObservableCollection<FixedCost> GetFixedCosts()
        {
            using (SampleContext db = new SampleContext())
            {
                var query = db.FixedCosts;
                ObservableCollection<FixedCost> fixedCosts = new ObservableCollection<FixedCost>();
                foreach (var q in query)
                    fixedCosts.Add(q);
                return fixedCosts;
            }
        }

        public static ObservableCollection<FixedCost> GetFixedCosts(TypeOfFixedCosts typeOfFixedCosts, bool withadmort=true)
        {
            using (SampleContext db = new SampleContext())
            {
                var query = db.FixedCosts.Where(f=>f.TypeOfFixedCosts==typeOfFixedCosts);
                ObservableCollection<FixedCost> fixedCosts = new ObservableCollection<FixedCost>();
                if (withadmort && typeOfFixedCosts == TypeOfFixedCosts.outcoming && SettingsDAL.GetSettings().VAT_type== VATType.full)
                {
                    FixedCost amort = null;
                    if (db.FixedCosts.Any(fc => fc.TypeOfFixedCosts == TypeOfFixedCosts.admortization))
                    {
                        amort = db.FixedCosts.Where(fc => fc.TypeOfFixedCosts == TypeOfFixedCosts.admortization).First();
                        
                    }
                    else
                    {
                        amort = new FixedCost();
                        amort.TypeOfFixedCosts = TypeOfFixedCosts.admortization;
                        amort.Name = "Амортизация";
                        amort.UnitOfPriceID = UnitDAL.GetUnitsMoney().First().ID;
                        amort.Price = 0;
                        db.FixedCosts.Add(amort);
                        db.SaveChanges();
                    }
                    if(!(amort is null))
                        fixedCosts.Add(amort);
                }
                foreach (var q in query)
                    fixedCosts.Add(q);
                
                return fixedCosts;
            }
        }

        public static double? SummOfFixedCosts()
        {
            using (SampleContext db = new SampleContext())
            {
                var query = db.FixedCosts.Sum(fc => (double)fc.Price);
                return query;
            }
        }

        public static void SetFixedCost(FixedCost fixedCost)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.FixedCosts.Any(f => f.ID == fixedCost.ID))
                {
                    db.FixedCosts.Update(fixedCost);
                }
                else
                {
                    db.FixedCosts.Add(fixedCost);
                }
                db.SaveChanges();
            }
        }

        public static void DeleteFixedCost(long ID)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.FixedCosts.Any(f => f.ID == ID))
                {
                    FixedCost fc = db.FixedCosts.Where(f => f.ID == ID).First();
                    db.FixedCosts.Remove(fc);
                    db.SaveChanges();
                }
            }
        }

    }
}
