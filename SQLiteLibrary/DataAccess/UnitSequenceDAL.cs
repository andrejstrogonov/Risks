using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteLibrary.DataAccess
{
    public static class UnitSequenceDAL
    {
        public static UnitSequence GetUnitSequence(long id)
        {
            using(SampleContext db = new SampleContext())
            {
                return db.UnitSequences.First(p => p.ID == id);
            }
        }

        public static void SetUnitSequence(UnitSequence unitSequence)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.UnitSequences.Any(p => p.ID == unitSequence.ID))
                {
                    db.UnitSequences.Update(unitSequence);
                }
                else
                {
                    db.UnitSequences.Add(unitSequence);
                }
                db.SaveChanges();
            }
        }

        public static List<UnitSequence> GetUnitSequences()
        {
            using (SampleContext db = new SampleContext())
            {
                return db.UnitSequences.ToList();
            }
        }

        public static UnitSequence GetUnitSequence(TypeOfUnit typeOfUnit)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                var result = db.UnitSequences.First(p=>p.TypeOfUnit==typeOfUnit);
                //SampleContext.Mutex.ReleaseMutex();
                SampleContext.IsBusy = false;
                return result;
            }
        }

        public static void DeleteUnitSequence(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.Units.Any(u => u.UnitSequence_ID == id))
                {
                    foreach(var u_id in db.Units.Where(u => u.UnitSequence_ID == id).Select(u => u.ID))
                    {
                        if (!UnitDAL.DeleteUnit(id))
                            return;
                    }
                }

                if (db.UnitSequences.Any(us => us.ID == id))
                {
                    db.UnitSequences.Remove(db.UnitSequences.First(p => p.ID == id));
                    db.SaveChanges();
                }
            }
        }
    }
}
