using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class MemberOfGroupViewModel:ViewModel
    {
        private MemberOfGroup memberOfGroup;


        public List<string> Products { get; set; }
        private Dictionary<string, Product> FromStrToProduct;

        private void Init()
        {
            Products = new List<string>();
            FromStrToProduct = new Dictionary<string, Product>();
            foreach(var p in ProductDAL.GetProducts())
            {
                Products.Add(p.Name);
                FromStrToProduct[p.Name] = p;
            }
        }

        public MemberOfGroupViewModel(MemberOfGroup memberOfGroup)
        {
            Init();
            this.memberOfGroup = memberOfGroup;
        }

        public MemberOfGroupViewModel()
        {
            Init();
            memberOfGroup = new MemberOfGroup();
        }

        public long ID_Product
        {
            get
            {
                return memberOfGroup.ID_Product;
            }
            set
            {
                memberOfGroup.ID_Product = value;
                OnPropertyChanged("ID_Product");
            }
        }

        public Product Product
        {
            get
            {
                return ProductDAL.GetProduct(memberOfGroup.ID_Product);
            }
            set
            {
                ID_Product = value.ID;
                OnPropertyChanged("Product");
            }
        }

        public string ProductName
        {
            get
            {
                return Product.Name;
            }
            set
            {
                Product = FromStrToProduct[value];
                OnPropertyChanged("ProductName");
            }
        }

        public long ID_Group
        {
            get
            {
                return memberOfGroup.ID_GroupOfInterchangable;
            }
            set
            {
                memberOfGroup.ID_GroupOfInterchangable = value;
                OnPropertyChanged("ID_Group");
            }
        }

        public void Save()
        {
            MemberOfGroupDAL.Save(memberOfGroup);
        }

        public void Delete()
        {
            MemberOfGroupDAL.Delete(memberOfGroup);
        }
    }
}
