using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteLibrary.DataAccess
{
    public static class SettingsDAL
    {
        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.Settings.Any(p => p.BudgetUnitID == id || p.PeriodUnitID == id));
            }
        }
        public static Settings GetSettings()
        {
            using(SampleContext db =new SampleContext())
            {
                if (db.Settings.Count() == 0)
                {
                    Settings settings = new Settings();
                    settings.Budget = double.PositiveInfinity;
                    settings.BudgetUnitID = UnitDAL.GetUnitsMoney().First().ID;
                    settings.Level = 3;
                    settings.Period = 1;
                    var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time).ID;
                    settings.PeriodUnitID = UnitDAL.GetUnits(time).First().ID;
                    settings.DateOfCreation = DateTime.Now;
                    db.Settings.Add(settings);
                    db.SaveChanges();
                    return settings;
                }
                return db.Settings.First();
            }
        }

        public static void SaveSettings(Settings settings)
        {
            using(SampleContext db = new SampleContext())
            {
                db.Settings.Update(settings);
                db.SaveChanges();
            }
        }
    }
}
