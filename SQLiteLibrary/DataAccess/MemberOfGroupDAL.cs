using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteLibrary.DataAccess
{
    public static class MemberOfGroupDAL
    {
        

        public static MemberOfGroup GetMemberOfGroup(long ID_Product, long ID_Group)
        {
            using(SampleContext db = new SampleContext())
            {
                if(db.MemberOfGroupsNullable.Any(m => m.ID_GroupOfInterchangable == ID_Group && m.ID_Product == ID_Product))
                 return new MemberOfGroup(db.MemberOfGroupsNullable.Where(m => m.ID_GroupOfInterchangable == ID_Group && m.ID_Product == ID_Product).First());
                return null;
            }
        }

        public static ObservableCollection<MemberOfGroup> GetMembersOfGroup(long ID_Group)
        {
            using(SampleContext db = new SampleContext())
            {
                ObservableCollection<MemberOfGroup> members = new ObservableCollection<MemberOfGroup>();
                foreach(var m in db.MemberOfGroupsNullable.Where(g=> g.ID_GroupOfInterchangable == ID_Group))
                {
                    members.Add(new MemberOfGroup(m));
                }
                return members;
            }
        }

        public static ObservableCollection<MemberOfGroup> GetAllMembersOfGroups()
        {
            using (SampleContext db = new SampleContext())
            {
                ObservableCollection<MemberOfGroup> members = new ObservableCollection<MemberOfGroup>();
                foreach (var m in db.MemberOfGroupsNullable)
                {
                    members.Add(new MemberOfGroup(m));
                }
                return members;
            }
        }

        public static void Save(MemberOfGroup memberOfGroup)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(m => m.ID_GroupOfInterchangable == memberOfGroup.ID_GroupOfInterchangable && m.ID_Product == memberOfGroup.ID_Product))
                    db.MemberOfGroupsNullable.Update(memberOfGroup.MemberOfGroupNullable);
                else
                    db.MemberOfGroupsNullable.Add(memberOfGroup.MemberOfGroupNullable);
                db.SaveChanges();
            }
        }

        public static void Delete(MemberOfGroup memberOfGroup)
        {
            using (SampleContext db = new SampleContext())
            {
                if (db.MemberOfGroupsNullable.Any(m => m.ID_GroupOfInterchangable == memberOfGroup.ID_GroupOfInterchangable && m.ID_Product == memberOfGroup.ID_Product))
                {
                    db.MemberOfGroupsNullable.Remove(memberOfGroup.MemberOfGroupNullable);
                    db.SaveChanges();
                }
            }
        }
    }
}
