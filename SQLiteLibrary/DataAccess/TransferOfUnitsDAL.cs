using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteLibrary.DataAccess
{
    public static class TransferOfUnitsDAL
    {
        public static ObservableCollection<TransferOfUnits> GetTransfersOfUnits()
        {
            using(SampleContext db = new SampleContext())
            {
                ObservableCollection<TransferOfUnits> result = new ObservableCollection<TransferOfUnits>();
                foreach(var tu in db.TransfersOfUnits)
                {
                    result.Add(tu);
                }
                return result;
            }
        }

        public static ObservableCollection<TransferOfUnits> GetTransfersOfUnits(long typeOfUnit)
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<TransferOfUnits> result = new ObservableCollection<TransferOfUnits>();
                foreach (var tu in db.TransfersOfUnits.Where(tou => UnitDAL.GetUnit(tou.ID_UnitFrom).UnitSequence_ID==typeOfUnit))
                {
                    result.Add(tu);
                }
                return result;
            }
        }

        public static ObservableCollection<TransferOfUnits> GetTransfersOfUnitFrom(Unit unit)
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<TransferOfUnits> result = new ObservableCollection<TransferOfUnits>();
                foreach (var tu in db.TransfersOfUnits.Where(ui=>ui.ID_UnitFrom==unit.ID))
                {
                    result.Add(tu);
                }
                return result;
            }
        }

        private static void addTransferOfUnits(TransferOfUnits transferOfUnits)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.TransfersOfUnits.Any(tu => tu.ID_UnitFrom == transferOfUnits.ID_UnitFrom && tu.ID_UnitTo == transferOfUnits.ID_UnitTo))
                    db.TransfersOfUnits.Update(transferOfUnits);
                else
                    db.TransfersOfUnits.Add(transferOfUnits);
                db.SaveChanges();
            }
        }

        public static void AddTransferOfUnits(TransferOfUnits transferOfUnits)
        {
            addTransferOfUnits(transferOfUnits);
        }

        public static void AddOrChange(TransferOfUnits t)
        {
            var name_from = t.UnitFrom.Name;
            var name_to = t.UnitTo.Name;
            t.UnitFrom = null;
            t.UnitTo = null;
            using (SampleContext db = new SampleContext())
            {
                t.ID_UnitFrom = db.Units.First(p => p.Name == name_from).ID;
                t.ID_UnitTo = db.Units.First(p => p.Name == name_to).ID;
                if (db.TransfersOfUnits.Any(tu=> tu.ID_UnitFrom == t.ID_UnitFrom && tu.ID_UnitTo == t.ID_UnitTo))
                    db.TransfersOfUnits.Update(t);
                else
                    db.TransfersOfUnits.Add(t);
                db.SaveChanges();
            }
        }

        public static bool DeleteTransferOfUnits(TransferOfUnits transferOfUnits)
        {
            using (SampleContext db = new SampleContext())
            {
                foreach (var ui in db.TransfersOfUnits.Where(tu => tu.ID_UnitFrom == transferOfUnits.ID_UnitFrom && tu.ID_UnitTo == transferOfUnits.ID_UnitTo))
                {
                    db.Remove(ui);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static void DeleteAllTransfersOfUnit(Unit unit)
        {
            using (SampleContext db = new SampleContext())
            {
               db.TransfersOfUnits.RemoveRange(db.TransfersOfUnits.Where(tu => tu.ID_UnitFrom == unit.ID || tu.ID_UnitTo == unit.ID));
                db.SaveChanges();
            }
        }

        public static void AddAllTransfersOfUnit(ObservableCollection<TransferOfUnits> transfers)
        {
            foreach (var t in transfers)
                AddTransferOfUnits(t);
        }

        public static ObservableCollection<TransferOfUnits> GetTemplateForUnit(Unit unit)
        {
            ObservableCollection<TransferOfUnits> result = GetTransfersOfUnitFrom(unit);
            using (SampleContext db = new SampleContext())
            {
                foreach(var u in db.Units.Where(u=> u.UnitSequence_ID==unit.UnitSequence_ID && u.ID != unit.ID))
                {
                    if(!result.Any(tu=>tu.ID_UnitFrom==unit.ID && tu.ID_UnitTo == u.ID))
                    {
                        TransferOfUnits transferOfUnits = new TransferOfUnits();
                        transferOfUnits.ID_UnitFrom = unit.ID;
                        transferOfUnits.ID_UnitTo = u.ID;
                        transferOfUnits.Coefficient = 1;
                        result.Add(transferOfUnits);
                    }
                }
            }
            //сортировка
            return result;
        }

        public static double GetTransferOfUnits(Unit from, Unit to)
        {
            using (SampleContext db = new SampleContext())
            {
                if (from.ID == to.ID)
                    return 1;
                return db.TransfersOfUnits.Where(t => t.ID_UnitFrom == from.ID && t.ID_UnitTo == to.ID).First().Coefficient;
            }
        }

        public static double GetTransferToCommonUnit(long id_from)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                var common = GetCommonUnitForType(UnitDAL.GetUnit(id_from).UnitSequence_ID);
                if (id_from == common.ID)
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    SampleContext.IsBusy = false;
                    return 1;
                }
                var transfer = db.TransfersOfUnits.First(t => t.ID_UnitFrom == id_from && t.ID_UnitTo == common.ID);
                if (transfer is null)
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    SampleContext.IsBusy = false;
                    return 0;
                }
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    SampleContext.IsBusy = false;
                    return transfer.Coefficient;
                }
            }
        }

        public static double GetTransferFromCommonUnit(long id_from)
        {
            
                //SampleContext.Mutex.WaitOne();
                SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                var common = GetCommonUnitForType(UnitDAL.GetUnit(id_from).UnitSequence_ID);
                if (id_from == common.ID)
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    SampleContext.IsBusy = false;
                    return 1;
                }

                
                var res = 1 / db.TransfersOfUnits.Where(t => t.ID_UnitFrom == id_from && t.ID_UnitTo == common.ID).First().Coefficient;
                //SampleContext.Mutex.ReleaseMutex();
                SampleContext.IsBusy = false;
                return res;
            }
        }

        public static double GetTransferOfUnits(long from, long to)
        {
            //SampleContext.Mutex.WaitOne();
            SampleContext.IsBusy = true;
            using (SampleContext db = new SampleContext())
            {
                if (from == to)
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    SampleContext.IsBusy = false;
                    return 1;
                }
                if (db.TransfersOfUnits.Any(t => t.ID_UnitFrom == from && t.ID_UnitTo == to))
                {
                    var res = db.TransfersOfUnits.First(t => t.ID_UnitFrom == from && t.ID_UnitTo == to).Coefficient;
                    {
                        //SampleContext.Mutex.ReleaseMutex();
                        SampleContext.IsBusy = false;
                        return res;
                    }
                }
                var res1= UnitDAL.TranslateFromCommonUnit(to, UnitDAL.TranslateToCommonUnit(from, 1));
                {
                    //SampleContext.Mutex.ReleaseMutex();
                    SampleContext.IsBusy = false;
                    return res1;
                }
            }
        }

        public static Unit GetCommonUnitForType(long unitsequence_id)
        {            
            return UnitDAL.GetUnits(unitsequence_id).First(p=>p.IsBasis);
        }

        public static Unit GetCommonUnitForType(TypeOfUnit typeOfUnit)
        {
            return UnitDAL.GetUnits(UnitSequenceDAL.GetUnitSequence(typeOfUnit).ID).First(p => p.IsBasis);
        }

        public static Unit GetCommonUnitForUnit(long unit_id)
        {
            var unitseqid = UnitDAL.GetUnit(unit_id).UnitSequence_ID;
            return UnitDAL.GetUnits(unitseqid).First(p => p.IsBasis);
        }

    }
}
