using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;

namespace DataProcessLibrary.InitialData
{
    public static class GroupsData
    {

        public static Dictionary<long, List<Product>> GroupsOfProducts { get;  set; }
        public static List<GroupOfInterchangable> Groups { get; set; }
        
        public static void Init()
        {
            Groups = new List<GroupOfInterchangable>();
            foreach(var g in GroupDAL.GetGroups())
            {
                if(!(g.Name is null || g.Minimum is null || g.Maximum is null || g.UnitOfMinimumID is null || g.UnitOfMaximumID is null))
                {
                    Groups.Add(new GroupOfInterchangable(g));
                }
            }

            GroupsOfProducts = new Dictionary<long, List<Product>>();
            foreach (var m in MemberOfGroupDAL.GetAllMembersOfGroups())
            {
                Product product = ProductData.GetProduct(m.ID_Product);
                GroupOfInterchangable group = GroupsData.GetGroup(m.ID_GroupOfInterchangable);
                if (!(product is null || group is null))
                {
                    if (!GroupsOfProducts.ContainsKey(m.ID_GroupOfInterchangable))
                    {

                        GroupsOfProducts[m.ID_GroupOfInterchangable] = new List<Product>();
                    }

                    GroupsOfProducts[m.ID_GroupOfInterchangable].Add(product);
                }
            }
        }

        public static GroupOfInterchangable GetGroup(long id)
        {
            if (Groups.Any(g => g.ID == id))
            {
                return Groups.Where(g => g.ID == id).First();
            }
            return null;
        }
    }
}
