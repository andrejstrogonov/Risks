using Riscks.Commands;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class ProductIsChecked
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
    public class GroupViewModel:ViewModel
    {
        private GroupOfInterchangableNullable group;

        public List<string> UnitsNotMoney { get; set; }
        private Dictionary<string, Unit> FromStrToUnit;

        public Dictionary<string,Product> FromStrToProduct { get; set; }

        public class ProductInDiagramm
        {
            public int Value { get; set; }
            public string Name { get; set; }
            public ProductInDiagramm()
            {
                Value = 1;
            }
        }
        public ObservableCollection<ProductInDiagramm> ProductInDiagramms { get; set; }
        public ObservableCollection<string> ProductsIn { get; set; }
        public ObservableCollection<string> ProductsOut { get; set; }

        private string selectedProductIn;

        public string SelectedProductIn
        {
            get
            {
                return selectedProductIn;
            }
            set
            {
                selectedProductIn = value;
                if(!(value is null))
                    SelectedProductOut = null;
                OnPropertyChanged(nameof(SelectedProductIn));
            }
        }

        private string selectedProductOut;

        public string SelectedProductOut
        {
            get
            {
                return selectedProductOut;
            }
            set
            {
                selectedProductOut = value;
                if (!(value is null))
                    SelectedProductIn = null;
                OnPropertyChanged(nameof(SelectedProductOut));
            }
        }

        private void Init()
        {
            UnitsNotMoney = new List<string>();
            FromStrToUnit = new Dictionary<string, Unit>();
            foreach(var u in UnitDAL.GetUnitsNotMoney())
            {
                UnitsNotMoney.Add(u.Symbol);
                FromStrToUnit[u.Symbol] = u;
            }
            ReloadProducts();
            MoveProductsCommand = new MoveProductsCommand(this);
        }
               
        public void ReloadProducts()
        {
            if(!(ProductsIn is null))
            foreach(var a in ProductsIn)
            {
                long id_product = FromStrToProduct[a].ID;
                if ((ProductDAL.GetProduct(id_product) is null))
                {
                    MemberOfGroup member = new MemberOfGroup();
                    member.ID_Product = id_product;
                    member.ID_GroupOfInterchangable = group.ID;
                    MemberOfGroupDAL.Delete(member);
                }
            }

            ProductInDiagramms = new ObservableCollection<ProductInDiagramm>();
            ProductsIn = new ObservableCollection<string>();
            ProductsOut = new ObservableCollection<string>();
            FromStrToProduct = new Dictionary<string, Product>();
            foreach (var p in ProductDAL.GetProducts())
            {
                bool isin = !(MemberOfGroupDAL.GetMemberOfGroup(p.ID, group.ID) is null);
                if (isin)
                {
                    ProductsIn.Add(p.Name);
                    ProductInDiagramms.Add(new ProductInDiagramm() { Name = p.Name });
                }
                else
                    ProductsOut.Add(p.Name);
                FromStrToProduct[p.Name] = p;
            }
            OnPropertyChanged(nameof(ProductsIn));
            OnPropertyChanged(nameof(ProductsOut));
            SelectedProductIn = null;
            SelectedProductOut = null;
        }
        public GroupViewModel(GroupOfInterchangableNullable groupOfInterchangable)
        {
            group = groupOfInterchangable;
            Init();
        }

        public GroupViewModel()
        {
            group = new GroupOfInterchangableNullable();
            group.Name = "";
            group.UnitOfMaximumID = UnitDAL.GetUnitsNotMoney().First().ID;
            group.UnitOfMinimumID = group.UnitOfMaximumID;
            Init();
        }

        public MoveProductsCommand MoveProductsCommand { get; set; } 

        public string Name
        {
            get
            {
                return group.Name;
            }
            set
            {
                group.Name = value;
                OnPropertyChanged("Name");
                GroupDAL.SetGroup(group);
            }
        }

        public long? UnitOfMaximumID
        {
            get
            {
                return group.UnitOfMaximumID;
            }
            set
            {
                group.UnitOfMaximumID = value;
                OnPropertyChanged("UnitOfMaximumID");
            }
        }

        public Unit UnitOfMaximum
        {
            get
            {
                return UnitDAL.GetUnit(group.UnitOfMaximumID);
            }
            set
            {
                UnitOfMaximumID = value.ID;
                OnPropertyChanged("UnitOfMaximum");
            }
        }

        public string UnitOfMaximumSymbol
        {
            get
            {
                return UnitOfMaximum.Symbol;
            }
            set
            {
                UnitOfMaximum = FromStrToUnit[value];
                OnPropertyChanged("UnitOfMaximumSymbol");
            }
        }

        public long? UnitOfMinimumID
        {
            get
            {
                return group.UnitOfMinimumID;
            }
            set
            {
                group.UnitOfMinimumID = value;
                OnPropertyChanged("UnitOfMinimumID");
            }
        }

        public Unit UnitOfMinimum
        {
            get
            {
                return UnitDAL.GetUnit(group.UnitOfMinimumID);
            }
            set
            {
                UnitOfMinimumID = value.ID;
                OnPropertyChanged("UnitOfMinimum");
            }
        }

        public string UnitOfMinimumSymbol
        {
            get
            {
                return UnitOfMinimum.Symbol;
            }
            set
            {
                UnitOfMinimum = FromStrToUnit[value];
                UnitOfMaximum = UnitOfMinimum;
                OnPropertyChanged("UnitOfMinimumSymbol");
                GroupDAL.SetGroup(group);
            }
        }

        public double? Minimum
        {
            get
            {
                return group.Minimum;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                group.Minimum = value;
                OnPropertyChanged("Minimum");
                GroupDAL.SetGroup(group);
            }
        }

        public double? Maximum
        {
            get
            {
                return group.Maximum;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                group.Maximum = value;
                OnPropertyChanged("Maximum");
                GroupDAL.SetGroup(group); 
            }
        }

        public void Save()
        {
            GroupDAL.SetGroup(group);
            foreach (var m in ProductsIn)
            {
                MemberOfGroup member = new MemberOfGroup();
                member.ID_Product = FromStrToProduct[m].ID;
                member.ID_GroupOfInterchangable = group.ID;
                MemberOfGroupDAL.Save(member);
            }

            foreach(var m in ProductsOut)
            {
                MemberOfGroup member = new MemberOfGroup();
                member.ID_Product = FromStrToProduct[m].ID;
                member.ID_GroupOfInterchangable = group.ID;
                MemberOfGroupDAL.Delete(member);
            }
        }

        public bool MoveIsEnable()
        {
                if (selectedProductOut is null && selectedProductIn is null)
                    return false;
                if (selectedProductOut is null)
                    return true;
                if (ProductsIn.Count() == 0)
                    return true;
                Product p = FromStrToProduct[selectedProductOut];
                var unit_p = UnitDAL.GetUnit(p.UnitOfStockID);
                var unit_need = UnitDAL.GetUnit(FromStrToProduct[ProductsIn.First()].UnitOfStockID);
                return (unit_p.UnitSequence_ID == unit_need.UnitSequence_ID);
            
        }

        public void Move()
        {
            if(selectedProductIn is null && !(selectedProductOut is null) && !ProductsIn.Any(p => p == selectedProductOut))
            {
                ProductsIn.Add(selectedProductOut);
                ProductInDiagramms.Add(new ProductInDiagramm() { Name = selectedProductOut });
                OnPropertyChanged(nameof(ProductInDiagramms));
                ProductsOut.Remove(selectedProductOut);
                SelectedProductIn = selectedProductOut;
                SelectedProductOut = null;
            }
            else
                if (selectedProductOut is null && !(selectedProductIn is null) && !ProductsOut.Any(p=>p==selectedProductIn))
                {
                    ProductsOut.Add(selectedProductIn);
                    ProductInDiagramms.Remove(ProductInDiagramms.Where(p => p.Name == selectedProductIn).First());
                    ProductsIn.Remove(selectedProductIn);
                    OnPropertyChanged(nameof(ProductInDiagramms));
                    SelectedProductOut = selectedProductIn;
                    SelectedProductIn = null;
                }
            Save();
        }

        public void Delete()
        {
            foreach (var m in ProductsIn)
            {
                MemberOfGroup member = new MemberOfGroup();
                member.ID_Product = FromStrToProduct[m].ID;
                member.ID_GroupOfInterchangable = group.ID;
                MemberOfGroupDAL.Delete(member);
            }
            GroupDAL.DeleteGroup(group);
        }

        public void ReloadUnits()
        {
            UnitsNotMoney = new List<string>();
            FromStrToUnit = new Dictionary<string, Unit>();
            foreach (var u in UnitDAL.GetUnitsNotMoney())
            {
                UnitsNotMoney.Add(u.Symbol);
                FromStrToUnit[u.Symbol] = u;
            }
            OnPropertyChanged("UnitsNotMoney");
        }
    }
}
