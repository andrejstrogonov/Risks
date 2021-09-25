using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;

namespace SQLiteLibrary.DataAccess
{
    public static class UnitDAL
    {
        public static ObservableCollection<Unit> GetUnits()
        {
            //SampleContext.Mutex.WaitOne();

            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<Unit> query = new ObservableCollection<Unit>();
                List<long> isUsingSeq_ids = db.UnitSequences.Where(p => p.IsUsing == true).Select(p => p.ID).ToList();
                foreach (var a in db.Units.Where(p=>isUsingSeq_ids.Contains( p.UnitSequence_ID)))
                {
                    query.Add(a);
                }

                SampleContext.IsBusy = false;
                //SampleContext.Mutex.ReleaseMutex();
                return query;
            }
        }

        public static ObservableCollection<Unit> GetUnits(long typeOfUnit)
        {
            ObservableCollection<Unit> query = new ObservableCollection<Unit>();
            //SampleContext.Mutex.WaitOne();

            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                foreach (var a in db.Units.Where(u=>u.UnitSequence_ID==typeOfUnit))
                {
                    query.Add(a);
                }
            }

            SampleContext.IsBusy = false;
            //SampleContext.Mutex.ReleaseMutex();
            return query;
        }
        
        public static ObservableCollection<Unit> GetUnitsMoney()
        {
            //SampleContext.Mutex.WaitOne();
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<Unit> query = new ObservableCollection<Unit>();
                var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
                foreach (var a in db.Units.Where(p => p.UnitSequence_ID == money.ID))
                {
                    query.Add(a);
                }
                //SampleContext.Mutex.ReleaseMutex();
                return query;
            }
        }

        public static ObservableCollection<Unit> GetUnitsNotMoney()
        {
            //SampleContext.Mutex.WaitOne();

            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<Unit> query = new ObservableCollection<Unit>();
                var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
                var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time);
                foreach (var a in db.Units.Where(p => p.UnitSequence_ID != money.ID && p.UnitSequence_ID!=time.ID))
                {
                    query.Add(a);
                }

                SampleContext.IsBusy = false;
                //SampleContext.Mutex.ReleaseMutex();
                return query;
            }
        }

        public static bool AddUnit(Unit unit)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                if (db.Units.Any(u => u.ID == unit.ID))
                    db.Units.Update(unit);
                else
                    db.Units.Add(unit);
                db.SaveChanges();
            }
            SampleContext.IsBusy = false;
            //SampleContext.Mutex.ReleaseMutex();
            return true;
        }

        public static void AddOrChange(Unit unit)
        {
            //SampleContext.Mutex.WaitOne();
            using (var db = new SampleContext())
            {
                unit.TransferOfUnitsFrom=null;
                unit.TransferOfUnitsTo=null;
                var unitSuq = db.UnitSequences.First(p => p.Name == unit.UnitSequence.Name);
                unit.UnitSequence = null;
                unit.UnitSequence_ID = unitSuq.ID;
                if (db.Units.Any(u => u.ID == unit.ID))
                {
                    db.Units.Update(unit);
                }
                else
                {
                    db.Units.Add(unit);
                }
                db.SaveChanges();
            }
            //SampleContext.Mutex.ReleaseMutex();
        }

        public static Unit GetUnit(long? ID)
        {
            if (ID is null)
                return null;
            Unit result = null;
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                result = db.Units.First(u => u.ID == ID);                
            }
            SampleContext.IsBusy = false;
            //SampleContext.Mutex.ReleaseMutex();
            return result;
        }

        public static bool DeleteUnit(long ID)
        {
            //SampleContext.Mutex.WaitOne();
            using (SampleContext db = new SampleContext())
            {
                if (!CanDeleteUnit(db.Units.First(o=>o.ID==ID)))
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    return false;
                }
                if(db.TransfersOfUnits.Any(tu=>tu.ID_UnitFrom==ID || tu.ID_UnitTo == ID))
                {
                    db.TransfersOfUnits.RemoveRange(db.TransfersOfUnits.Where(tu => tu.ID_UnitFrom == ID || tu.ID_UnitTo == ID));
                    db.SaveChanges();
                }
                if (db.Units.Any(u => u.ID == ID))
                {
                    db.Units.Remove(db.Units.First(u => u.ID == ID));
                    db.SaveChanges();
                }
                //SampleContext.Mutex.ReleaseMutex();
                return true;
            }
        }

        public static bool CanDeleteUnit(Unit unit)
        {
            using (var db =new  SampleContext())
            {
                try
                {
                    if (!db.Units.Any(c => c.ID == unit.ID || c.Name==unit.Name && c.Symbol==unit.Symbol))
                        return true;
                    Unit p = db.Units.First(u => u.ID == unit.ID || u.Name == unit.Name && u.Symbol == unit.Symbol);
                    return FixedCostDAL.CanDeleteUnit(p.ID) &&
                            GroupDAL.CanDeleteUnit(p.ID) &&
                            ProductDAL.CanDeleteUnit(p.ID) &&
                            ResourceDAL.CanDeleteUnit(p.ID) &&
                            ResourceOfProductDAL.CanDeleteUnit(p.ID) &&
                            RiskOfProductDAL.CanDeleteUnit(p.ID) &&
                            SettingsDAL.CanDeleteUnit(p.ID);
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }

        public static double TranslateToCommonUnit(long unitID, double value)
        {            
            var transfer = TransferOfUnitsDAL.GetTransferToCommonUnit(unitID);            
            double result = value * transfer;
            return result;
        }

        public static double TranslateFromCommonUnit(long unitID, double value)
        {
            var transfer = TransferOfUnitsDAL.GetTransferFromCommonUnit(unitID);
            double result = value * transfer;
            return result;
        }

        private class UnitComparer : IEqualityComparer<Unit>
        {
            public bool Equals(Unit x, Unit y)
            {
                return x.ID==y.ID && x.UnitSequence_ID==y.UnitSequence_ID;
            }

            public int GetHashCode(Unit obj)
            {
                return (int)obj.ID;
            }
        }

        private class UnitSeqComparer : IEqualityComparer<UnitSequence>
        {
            public bool Equals(UnitSequence x, UnitSequence y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(UnitSequence obj)
            {
                return (int)obj.ID;
            }
        }

        private class UnitTranComparer : IEqualityComparer<TransferOfUnits>
        {
            public bool Equals(TransferOfUnits x, TransferOfUnits y)
            {
                return x.ID_UnitFrom == y.ID_UnitFrom && x.ID_UnitTo==y.ID_UnitTo;
            }

            public int GetHashCode(TransferOfUnits obj)
            {
                return (int)obj.ID_UnitFrom*10000+(int)obj.ID_UnitTo;
            }
        }
        public static bool UnitsWereChange(string path_from)
        {
            string thisPath = SampleContext.Path;

            if (path_from == thisPath)
                return false;
            SampleContext.Path = path_from;
            List<UnitSequence> unitsequences = new List<UnitSequence>();
            List<Unit> units = new List<Unit>();
            List<TransferOfUnits> transfers = new List<TransferOfUnits>();
            using (var db = new SampleContext())
            {
                unitsequences = db.UnitSequences.ToList();
                units = db.Units.ToList();
                transfers = db.TransfersOfUnits.ToList();
            }
            SampleContext.Path = thisPath;
            using (var db = new SampleContext())
            {
                var unisS = db.UnitSequences.ToList();
                var unit = db.Units.ToList();
                var tran = db.TransfersOfUnits.ToList();
                var unitcmp = new UnitComparer();
                var unittrancmp = new UnitTranComparer();
                var unitSeqCmp = new UnitSeqComparer();
                if (units.Union(unit, unitcmp).Except(units.Intersect(unit, unitcmp), unitcmp).Count() > 0)
                    return true;
                if (transfers.Union(tran, unittrancmp).Except(transfers.Intersect(tran, unittrancmp), unittrancmp).Count() > 0)
                    return true;
                if (unitsequences.Union(unisS, unitSeqCmp).Except(unitsequences.Intersect(unisS, unitSeqCmp), unitSeqCmp).Count() > 0)
                    return true;
            }
            return false;
        }
        public static void CopyUnitDataFrom(string path_from)
        {
            string thisPath = SampleContext.Path;

            if (path_from == thisPath)
                return;
            SampleContext.Path = path_from;
            List<UnitSequence> unitsequences = new List<UnitSequence>();
            List<Unit> units = new List<Unit>();
            List<TransferOfUnits> transfers = new List<TransferOfUnits>();
            using(var db = new SampleContext())
            {
                unitsequences = db.UnitSequences.ToList();
                units = db.Units.ToList();
                transfers = db.TransfersOfUnits.ToList();
            }
            SampleContext.Path = thisPath;
            using (var db = new SampleContext())
            {
                db.TransfersOfUnits.RemoveRange(db.TransfersOfUnits.ToList().Except(transfers, new UnitTranComparer()));
                db.SaveChanges();
                var units_to_delete = db.Units.ToList().Except(units, new UnitComparer()).ToList();
                db.Units.RemoveRange(units_to_delete);
                db.SaveChanges();
                db.UnitSequences.RemoveRange(db.UnitSequences.ToList().Except(unitsequences, new UnitSeqComparer()).ToList());
                db.SaveChanges();
            }

            foreach (var us in unitsequences)
            {
                us.Units.Clear();
                UnitSequenceDAL.SetUnitSequence(us);
            }
            foreach (var u in units)
            {
                UnitDAL.AddOrChange(u);
            }        
            foreach(var t in transfers)
            {                
                TransferOfUnitsDAL.AddOrChange(t);
            }
        }
    }
}
