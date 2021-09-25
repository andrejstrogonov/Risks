using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using Riscks.Commands;
using DataProcessLibrary.InitialData;
using System.Windows;
using SQLiteLibrary;
using System.Threading;

namespace Riscks.ViewModel
{
    public class IndexProductViewModel: ViewModel
    {

        public Visibility IsFullVAT
        {
            get
            {
                return InitialData.Settings.VAT_type == VATType.full ? Visibility.Visible : Visibility.Collapsed;
            }
            private set
            {
            }
        }

        public void ReloadVATVisilibity()
        {
            OnPropertyChanged(nameof(IsFullVAT));
        }

        private ProductViewModel currentProduct;

        public ProductViewModel CurrentProduct
        {
            get
            {
                return currentProduct;
            }
            set
            {
                currentProduct = value;
                OnPropertyChanged(nameof(CurrentProduct));
                //if(!(currentProduct is null))
                //currentProduct.Save();
            }
        }

        public void ReloadInterface()
        {
            CurrentProduct.ReloadInterface();
        }

        public ObservableCollection<Unit> unitsNotMoney { get; private set; }

        private Dictionary<string, Unit> FromSymbolToUnit;

        public List<string> unitsDefaultSymbols { get; set; }

        public ObservableCollection<Unit> unitsMoney { get; private set; }

        private ObservableCollection<ProductViewModel> products;
        public ObservableCollection<ProductViewModel> Products {
            get
            {
                return products;
            }
            set
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public ObservableCollection<Resource> resources { get; set; }

        public void Init()
        {
            unitsNotMoney = UnitDAL.GetUnitsNotMoney();
            unitsMoney = UnitDAL.GetUnitsMoney();
            resources = ResourceDAL.GetResources();

            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsDefaultSymbols = new List<string>();
            foreach (var u in UnitDAL.GetUnitsNotMoney())
            {
                unitsDefaultSymbols.Add(u.Symbol);
                FromSymbolToUnit[u.Symbol] = u;
            }
            defaultUnit = FromSymbolToUnit.First().Value;
        }

        public IndexProductViewModel(CommonViewModel commonViewModel):base(commonViewModel)
        {
            Init();
            Products = new ObservableCollection<ProductViewModel>();
            foreach (var r in ProductDAL.GetProductsNullable())
            {
                var pvm = new ProductViewModel(r, CommonViewModel);
                pvm.IndexResourceOfProductViewModel.ResourceChanged += ReactOnChange;
                Products.Add(pvm);
            }
            if (Products.Count > 0)
                currentProduct = Products.First();
            else
                currentProduct = null;

            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(CurrentProduct));
            AddCommand = new AddProductCommand(this);
            DeleteCommand = new DeleteProductCommand(this);
        }

        public void Reload()
        {
            foreach(var p in Products)
            {
                p.IndexResourceOfProductViewModel.ResourceChanged -= ReactOnChange;
            }
            Products.Clear();
            foreach (var r in ProductDAL.GetProductsNullable())
            {
                var pvm = new ProductViewModel(r, CommonViewModel);
                pvm.IndexResourceOfProductViewModel.ResourceChanged += ReactOnChange;
                Products.Add(pvm);
            }
            if (Products.Count > 0)
                currentProduct = Products.First();
            else
                currentProduct = null;

            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(CurrentProduct));
        }

        public AddProductCommand AddCommand { get; protected set; }

        public DeleteProductCommand DeleteCommand { get; protected set; }

        public AddResourceOfProductCommand AddResourceCommand { get; protected set; }

        private int ContainsKey(ProductViewModel rvm)
        {
            foreach (var r in Products)
                if (r.product.ID==rvm.product.ID)
                    return Products.IndexOf(r);
            return -1;
        }

        public delegate void ProductsChanged();
        public event ProductsChanged OnProductsChanged;

        public void AddProduct()
        {
            ProductViewModel productViewModel = new ProductViewModel(defaultUnit, CommonViewModel);
            productViewModel.IndexResourceOfProductViewModel.ResourceChanged += ReactOnChange;
            products.Add(productViewModel);
            currentProduct = productViewModel;
            currentProduct.Save();
            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(CurrentProduct));
            Thread thread = new Thread(() =>
            {

                if (!(OnProductsChanged is null))
                    OnProductsChanged();
            });
            thread.Start();
        }

        public void DeleteProduct()
        {
            if (!(CurrentProduct is null))
            {
                int index = products.IndexOf(currentProduct);
                currentProduct.Delete();
                products.RemoveAt(index);

                OnPropertyChanged(nameof(Products));
                OnPropertyChanged(nameof(CurrentProduct));
                //IndexGroupViewModel.ReloadProducts();
                Thread thread = new Thread(() =>
                {

                    if (!(OnProductsChanged is null))
                        OnProductsChanged();
                });
                thread.Start();
            }

        }

        public event ObjectChanged ResourceChanged;

        public void ReactOnChange(long id)
        {
            Thread thread = new Thread(() =>
              {
                  foreach (var p in products)
                  {
                      p.IndexResourceOfProductViewModel.ReloadResources(id);
                  }
              });
            thread.Start();
        }

        /*
        public void AddResource() {
            currentProduct.Save();
            currentProduct.IndexResourceOfProductViewModel.AddResource();
        }*/

        private Unit defaultUnit;

        public string UnitDefaultSymbol
        {
            get
            {
                return defaultUnit.Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    defaultUnit = FromSymbolToUnit[value];
                    OnPropertyChanged(nameof(UnitDefaultSymbol));
                }
            }
        }

        public void ReloadResources(object sender, PropertyChangedEventArgs e)
        {
            /*if(e.PropertyName == "CurrentResource")
            {
                for (int i = 0; i < Products.Count(); i++)
                {
                    Products[i].IndexResourceOfProductViewModel.ReloadAVC();
                }
            }
            else
            if(e.PropertyName=="Resources")
            for (int i=0; i < Products.Count(); i++){
                Products[i].IndexResourceOfProductViewModel.ReloadResources();
            }*/
        }

        public void ReloadResources(long id)
        {
            Thread thread = new Thread(() =>
            {
                for (int i = 0; i < Products.Count(); i++)
                {
                    Products[i].IndexResourceOfProductViewModel.ReloadResources(id);
                }

                OnPropertyChanged("Products");
            });
            thread.Start();
        }

        public void ReloadUnits()
        {
            Thread thread = new Thread(() =>
              {
                  foreach (var p in Products)
                      p.ReloadUnits();
              });
            thread.Start();
        }

    }
}
