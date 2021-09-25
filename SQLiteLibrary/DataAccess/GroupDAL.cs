using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;

namespace SQLiteLibrary.DataAccess
{
    public static class GroupDAL
    {
        public static bool CanDeleteUnit(long id)
        {
            using (SampleContext db = new SampleContext())
            {
                return !(db.GroupOfInterchangablesNullable.Any(p => p.UnitOfMaximumID == id || p.UnitOfMinimumID==id));
            }
        }


        public static GroupOfInterchangable GetGroup(long ID)
        {
            using(SampleContext db = new SampleContext())
            {
                return new GroupOfInterchangable(db.GroupOfInterchangablesNullable.Where(g => g.ID == ID).First());
            }
        }

        public static void SetGroup(GroupOfInterchangable group)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.GroupOfInterchangablesNullable.Any(g => g.ID == group.ID))
                    db.GroupOfInterchangablesNullable.Update(group.GroupOfInterchangableNullable);
                else
                    db.GroupOfInterchangablesNullable.Add(group.GroupOfInterchangableNullable);
                db.SaveChanges();
            }
        }

        public static void SetGroup(GroupOfInterchangableNullable group)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.GroupOfInterchangablesNullable.Any(g => g.ID == group.ID))
                    db.GroupOfInterchangablesNullable.Update(group);
                else
                    db.GroupOfInterchangablesNullable.Add(group);
                db.SaveChanges();
            }
        }

        public static void DeleteGroup(GroupOfInterchangable group)
        {
            using(SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(m => m.ID_GroupOfInterchangable == group.ID))
                {
                    db.MemberOfGroupsNullable.RemoveRange(db.MemberOfGroupsNullable.Where(m => m.ID_GroupOfInterchangable == group.ID));
                    db.SaveChanges();
                }
                if (db.GroupOfInterchangablesNullable.Any(g => g.ID == group.ID))
                    db.GroupOfInterchangablesNullable.Remove(group.GroupOfInterchangableNullable);
                db.SaveChanges();
            }
        }

        public static void DeleteGroup(GroupOfInterchangableNullable group)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(m => m.ID_GroupOfInterchangable == group.ID))
                {
                    db.MemberOfGroupsNullable.RemoveRange(db.MemberOfGroupsNullable.Where(m => m.ID_GroupOfInterchangable == group.ID));
                    db.SaveChanges();
                }
                if (db.GroupOfInterchangablesNullable.Any(g => g.ID == group.ID))
                    db.GroupOfInterchangablesNullable.Remove(group);
                db.SaveChanges();
            }
        }

        public static ObservableCollection<GroupOfInterchangableNullable> GetGroups()
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<GroupOfInterchangableNullable> groups = new ObservableCollection<GroupOfInterchangableNullable>();
                foreach(var g in db.GroupOfInterchangablesNullable)
                {
                    groups.Add(g);
                }
                return groups;
            }
        }


    }
}
