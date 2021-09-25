using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riscks.Commands;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace Riscks.ViewModel
{
    public class IndexResourceOfProductViewModel: ViewModel
    {
        private Dispatcher Dispatcher;

        private ProductNullable Product;
        private ResourceOfProductViewModel currentResource;

        public ResourceOfProductViewModel CurrentResource
        {
            get
            {
                return currentResource;
            }
            set
            {
                currentResource = value;
                OnPropertyChanged("CurrentResource");
                ReloadAVC();
                //if (!(currentResource is null))
                  //  currentResource.Save();
           }
        }

        public ObservableCollection<ResourceOfProductViewModel> ResourcesOfProduct { get; set; }

        public IndexResourceOfProductViewModel(ProductNullable product, CommonViewModel commonViewModel):base(commonViewModel)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Product = product;
            ResourcesOfProduct = new ObservableCollection<ResourceOfProductViewModel>();
            foreach(var r in ResourceOfProductDAL.GetAllResourcesOfProduct(product))
            {
                var res = new ResourceOfProductViewModel(r,this, commonViewModel);
                //res.ResourceChanged += ReactOnChange;
                res.PropertyChanged += Resource_PropertyChanged;
                ResourcesOfProduct.Add(res);
            }
            if (ResourcesOfProduct.Count() > 0)
                currentResource = ResourcesOfProduct.First();
            DeleteResourceCommand = new DeleteResourceOfProductCommand(this);
            AddResourceCommand = new AddResourceOfProductCommand(this);
            ReloadAVC();

            PricesOfResources = new List<PriceOfResource>();
            foreach(var p in ResourcesOfProduct)
            {
                PriceOfResource por = new PriceOfResource(p);
                PricesOfResources.Add(por);
            }

        }

        private void Resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReloadAVC();
        }


        /*
public IndexResourceOfProductViewModel()
{
   CurrentResource = new ResourceOfProductViewModel();
   ResourcesOfProduct = new ObservableCollection<ResourceOfProductViewModel>();
   DeleteResourceCommand = new DeleteResourceOfProductCommand(this);

}*/


        public AddResourceOfProductCommand AddResourceCommand { get; protected set; }
        public DeleteResourceOfProductCommand DeleteResourceCommand { get; protected set; }

        private string avc;
        public string AVC
        {
            get
            {
                return avc;
            }
        }


        public void ReloadAVC()
        {
            //var a =AVC;
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            avc = Math.Round(ProductDAL.GetAVCOfProduct(Product), 2) + " " + TransferOfUnitsDAL.GetCommonUnitForType(money.ID).Symbol;
            OnPropertyChanged(nameof(AVC));
            OnPropertyChanged(nameof(PricesOfResources));
        }

        public class PriceOfResource
        {
            public double Value { get; set; }
            private string name;
            public string Name { get; set; }
            
            public ResourceOfProductViewModel ResourceOfProduct { get; set; }
            public PriceOfResource(ResourceOfProductViewModel resourceOfProduct)
            {
                
                ResourceOfProduct = resourceOfProduct;
                Name = resourceOfProduct.ResourceName;
                Value = resourceOfProduct.PriceOfResourceInCommonUnit;
                ResourceOfProduct.PropertyChanged += ResourceOfProduct_PropertyChanged;
            }

            private void ResourceOfProduct_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "ResourceName" || e.PropertyName == "AmountOfResource" || e.PropertyName == "UnitOfResourceSymbol")
                {
                    Name = ResourceOfProduct.ResourceName;
                    Value = ResourceOfProduct.PriceOfResourceInCommonUnit;
                }
            }
        }
        
        public List<PriceOfResource> PricesOfResources { get; set; }
        
        public void ReloadResources(long id)
        {
            var res = ResourceDAL.GetResource(id);
            if(res is null)
            {
                for (int i = 0; i < ResourcesOfProduct.Count(); i++)
                {
                    if (ResourcesOfProduct[i].ID_Resource == id)
                    {
                        currentResource = ResourcesOfProduct[i];
                        DeleteResource();
                    }
                }
                OnPropertyChanged("ResourcesOfProduct");
                if (ResourcesOfProduct.Count() > 0)
                    currentResource = ResourcesOfProduct.First();
                ReloadAVC();
                return;
            }
            if (ResourcesOfProduct.Any(p => p.ID_Resource == id))
            {
                for (int i = 0; i < ResourcesOfProduct.Count(); i++)
                {
                    if (ResourcesOfProduct[i].ID_Resource == id)
                    {
                        ResourcesOfProduct[i].ReloadUnits();
                        ReloadAVC();

                    }
                }
                OnPropertyChanged("ResourcesOfProduct");
                if (ResourcesOfProduct.Count() > 0)
                    currentResource = ResourcesOfProduct.First();
                return;
            }
            for (int i = 0; i < ResourcesOfProduct.Count(); i++)
            {
                ResourcesOfProduct[i].ReloadResources();
            }
            ReloadAVC();
        }

        public event ObjectChanged ResourceChanged;

        public void ReactOnChange(long id)
        {
            ResourceChanged(id);
        }

        public void DeleteResource()
        {
            if (!(currentResource is null))
            {
                PricesOfResources.Remove(PricesOfResources.First(p => p.ResourceOfProduct.ID_Resource == currentResource.ID_Resource));
                currentResource.Delete();
                ResourcesOfProduct.Remove(currentResource);
                ReloadAVC();
                OnPropertyChanged("ResourcesOfProduct");
            }
        }

        public void DeleteResource(long id)
        {
            if (PricesOfResources.Any(p => p.ResourceOfProduct.ID_Resource == id))
            {
                PricesOfResources.Remove(PricesOfResources.First(p => p.ResourceOfProduct.ID_Resource == id));
                Dispatcher.Invoke(new Action(() => {
                    ResourcesOfProduct.Remove(ResourcesOfProduct.First(p => p.ID_Resource == id));
                }
                ));
                ReloadAVC();
                OnPropertyChanged("ResourcesOfProduct");
            }
        }

        public void AddResource()
        {
            if (ResourceDAL.GetResources().Count() > 0)
            {
                ResourceOfProductViewModel resourceOfProduct = new ResourceOfProductViewModel(Product,this, CommonViewModel);
                resourceOfProduct.PropertyChanged += Resource_PropertyChanged;
                //resourceOfProduct.ResourceChanged += ReactOnChange;
                ResourcesOfProduct.Add(resourceOfProduct);
                currentResource = resourceOfProduct;
                currentResource.Save();
                ReloadAVC();
                OnPropertyChanged("ResourcesOfProduct");
                OnPropertyChanged("CurrentResource");
                PricesOfResources.Add(new PriceOfResource(resourceOfProduct));
            }
            else
            {
                MessageBox.Show("Внесите информацию о ресурсах в базу!");
            }
        }

        public void Save()
        {
            foreach (var r in ResourcesOfProduct)
                r.Save();
        }

        public void Delete()
        {
            foreach (var r in ResourcesOfProduct)
                r.Delete(false);
        }

        public void ReloadUnits()
        {           
                  foreach (var r in ResourcesOfProduct)
                      r.ReloadUnits();
        }
    }
}
