using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using System.Collections.ObjectModel;
using System.Threading;

namespace Riscks.ViewModel
{
    public class ResourceOfProductViewModel:ViewModel
    {
        private Resource resource;

        private void ResourceViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="Price" || e.PropertyName=="UnitOfPriceSymbol")
                OnPropertyChanged("PriceInCommonUnit");
        }

        private ResourceOfProductNullable resourceOfProduct;
        
        public List<string> unitsNotMoney { get; set; }

        private Dictionary<string, Unit> FromSymbolToUnit;

        public List<string> resources { get; set; }

        public static Dictionary<string,Resource> FromStrToResource { get; set; }

        private void Init()
        {           
            resources = new List<string>();
            FromStrToResource = new Dictionary<string, Resource>();
            foreach (var r in ResourceDAL.GetResources())
            {
                resources.Add(r.Name);
                FromStrToResource[r.Name] = r;
            }
            ReloadUnits();
            if (FromStrToResource.Any(p => p.Value.ID == resourceOfProduct.ID_Resource))
            {
                resource= FromStrToResource.First(p => p.Value.ID == resourceOfProduct.ID_Resource).Value;
                OnPropertyChanged(nameof(ResourceName));
            }
        }


        private void Init(long id)
        {
            /*resources = new List<string>();
            FromStrToResource = new Dictionary<string, Resource>();
            foreach (var r in ResourceDAL.GetResources())
            {
                resources.Add(r.Name);
                FromStrToResource[r.Name] = r;
            }*/
            
            resource = null;
            resources = FromStrToResource.Keys.ToList();
            OnPropertyChanged(nameof(resources));
            if (FromStrToResource.Any(p => p.Value.ID == resourceOfProduct.ID_Resource))
            {
                resource = FromStrToResource.First(p => p.Value.ID == resourceOfProduct.ID_Resource).Value;
                OnPropertyChanged(nameof(ResourceName));
            }
            if (!(resource is null))
                ReloadUnits();
           
        }

        private void IndexResourcesViewModel_ResourcesChanged(long id)
        {
            if(resource.ID==id && ResourceDAL.GetResource(id) is null)
            {
                CommonViewModel.IndexResourcesViewModel.ResourcesChanged -= IndexResourcesViewModel_ResourcesChanged;
                IndexResourceOfProductViewModel.DeleteResource(id);
                DeleteWithoutReload();
            }
            else
                Init(id);
        }

        public void ReloadUnits()
        {            
            
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsNotMoney = new List<string>();
            var unitid = ResourceDAL.GetResource(resource.ID).UnitOfMaximumID;
            long typeOfUnit = UnitDAL.GetUnit(unitid).UnitSequence_ID;
            if (!(resource is null))
            {
                foreach (var u in UnitDAL.GetUnits(typeOfUnit))
                {
                    FromSymbolToUnit[u.Symbol] = u;
                    unitsNotMoney.Add(u.Symbol);
                }
                if (resourceOfProduct.UnitOfResourceID is null || !FromSymbolToUnit.ContainsKey(UnitDAL.GetUnit(resourceOfProduct.UnitOfResourceID).Symbol))
                    resourceOfProduct.UnitOfResourceID = FromSymbolToUnit[unitsNotMoney.First()].ID;
            }
            OnPropertyChanged(nameof(unitsNotMoney));
        }
        private ResourceViewModel ResourceViewModel {
            get
            {
                return CommonViewModel.IndexResourcesViewModel.Resources.First(p => p.Resource_ID == resource.ID);
            }
        }
        private IndexResourceOfProductViewModel IndexResourceOfProductViewModel;
        public ResourceOfProductViewModel(ProductNullable p,IndexResourceOfProductViewModel indexrop, CommonViewModel commonViewModel):base(commonViewModel)
        {
            IndexResourceOfProductViewModel = indexrop;
            commonViewModel.IndexResourcesViewModel.ResourcesChanged += IndexResourcesViewModel_ResourcesChanged;
            
            resourceOfProduct = new ResourceOfProductNullable();
            if (ResourceDAL.GetResources().Count > 0)
            {
                resource = ResourceDAL.GetResources().First();
                resourceOfProduct.ID_Resource = resource.ID;
            }
            resourceOfProduct.ID_Product = p.ID;
            Init();
        }

        public ResourceOfProductViewModel(ResourceOfProductNullable resourceOfProduct, IndexResourceOfProductViewModel indexrop, CommonViewModel commonViewModel):base(commonViewModel)
        {
            IndexResourceOfProductViewModel = indexrop;
            commonViewModel.IndexResourcesViewModel.ResourcesChanged += IndexResourcesViewModel_ResourcesChanged;

            this.resourceOfProduct = resourceOfProduct;
            resource = ResourceDAL.GetResource(resourceOfProduct.ID_Resource);
            Init();
        }

        public ResourceOfProductViewModel(IndexResourceOfProductViewModel indexrop, CommonViewModel commonViewModel):base(commonViewModel)
        {
            IndexResourceOfProductViewModel = indexrop;
            commonViewModel.IndexResourcesViewModel.ResourcesChanged += IndexResourcesViewModel_ResourcesChanged;

            resourceOfProduct = new ResourceOfProductNullable();
            if (ResourceDAL.GetResources().Count > 0)
                resource = ResourceDAL.GetResources().First();            
            resourceOfProduct.ID_Resource = resource.ID;
            resourceOfProduct.UnitOfResourceID = ResourceDAL.GetResource(resourceOfProduct.ID_Resource).UnitOfMaximumID;
            Init();
        }

        public Product Product
        {
            get
            {
                return ProductDAL.GetProduct(resourceOfProduct.ID_Product);
            }
            set
            {
                resourceOfProduct.ID_Product = value.ID;
                OnPropertyChanged("Product");
                Save();
            }
        }

        //private Resource resource;

        //public event ObjectChanged ResourceChanged;

        public string ResourceName
        {
            get
            {
                if (!(resource is null))
                    return resource.Name;
                else
                    return "";
            }
            set
            {

                if (!(value is null) && FromStrToResource.ContainsKey(value))
                {
                    var lastval = resource.ID;
                    var last = ResourceName;
                    if (last == value)
                        return;
                    resource = FromStrToResource[value];
                    resourceOfProduct.ID_Resource = resource.ID;
                    var nextval = resource.ID;
                    var next = ResourceName;
                    Logger(SQLiteLibrary.TypeOfHistoryItem.ResourceOfProduct, resourceOfProduct.ID, nameof(resourceOfProduct.ID_Resource), lastval, nextval,
                      Product.Name + '\n' + last, last + '\n' + next);

                    ReloadUnits();
                    OnPropertyChanged("ID_Resource");
                    OnPropertyChanged("ResourceName");
                    OnPropertyChanged("Resource");
                    OnPropertyChanged("unitsNotMoney");
                    OnPropertyChanged("UnitOfResourceSymbol");
                    Save(true);
                    //ResourceChanged(lastval);
                    CommonViewModel.IndexResourcesViewModel.Resources.First(p => p.Resource_ID == lastval).ReloadUsing();
                    CommonViewModel.IndexResourcesViewModel.Resources.First(p => p.Resource_ID == nextval).ReloadUsing();
                }
            }
        }

        public double PriceOfResourceInCommonUnit
        {
            get
            {
                var r1 = ResourceOfProductDAL.TransferToCommon(ResourceOfProductDAL.GetResourceOfProduct(resourceOfProduct.ID));
                r1.ResourceNullable = ResourceDAL.TransferToCommon(ResourceDAL.GetResource( r1.ID_Resource)).ResourceNullable;
                return (double)r1.AmountOfResource * (double)r1.ResourceNullable.Price;
            }
        }

        public long ID_Resource
        {
            get
            {
                return resourceOfProduct.ID_Resource;
            }
            set
            {

                resourceOfProduct.ID_Resource = value;
                OnPropertyChanged("ID_Resource");
            }
        }

        public double? AmountOfResource
        {
            get
            {
                return resourceOfProduct.AmountOfResource;
            }
            set
            {
                if (value.HasValue)
                    value = Math.Round(value.Value, 3);
                var last = resourceOfProduct.AmountOfResource;
                if (last == value)
                    return;
                resourceOfProduct.AmountOfResource = value;
                Logger(SQLiteLibrary.TypeOfHistoryItem.ResourceOfProduct, resourceOfProduct.ID, nameof(resourceOfProduct.AmountOfResource), last, resourceOfProduct.AmountOfResource,
                       Product.Name + '\n' + resource.Name, last.ToString() + '\n' + resourceOfProduct.AmountOfResource);
                Save(true);
                OnPropertyChanged("AmountOfResource");
                ResourceViewModel.ReloadUsing();
            }
        }

        public string UnitOfResourceSymbol
        {
            get
            {
                if (UnitDAL.GetUnit(resourceOfProduct.UnitOfResourceID) is null)
                    return "";
                return UnitDAL.GetUnit(resourceOfProduct.UnitOfResourceID).Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    var last = UnitOfResourceSymbol;
                    if (last == value)
                        return;
                    var lastval = resourceOfProduct.UnitOfResourceID;
                    resourceOfProduct.UnitOfResourceID = FromSymbolToUnit[value].ID;
                    var next = UnitOfResourceSymbol;
                    var nextval = resourceOfProduct.UnitOfResourceID;
                    Logger(SQLiteLibrary.TypeOfHistoryItem.ResourceOfProduct, resourceOfProduct.ID, nameof(resourceOfProduct.UnitOfResourceID), lastval, nextval,
                      Product.Name + '\n' + resource.Name, last + '\n' + next);
                    Save(true);
                    OnPropertyChanged("UnitOfResourceSymbol");
                    ResourceViewModel.ReloadUsing();
                }
            }
        }

        public void Save(bool needreload=false)
        {
            if (!(ProductDAL.GetProductNullable(resourceOfProduct.ID_Product) is null))
            {
                ResourceOfProductDAL.AddResourceOfProduct(resourceOfProduct);
                /*if(needreload && !(ResourceChanged is null))
                    ResourceChanged(resourceOfProduct.ID_Resource);*/
            }
        }

        public void Delete(bool needlog=true)
        {
            if (needlog)
            {
                Logger(SQLiteLibrary.TypeOfHistoryItem.ResourceOfProduct, resourceOfProduct.ID, "", resourceOfProduct.ID, null,
                     Product.Name + '\n' + resource.Name, "Удален");
            }
                ResourceOfProductDAL.RemoveResourceOfProduct(resourceOfProduct);
            /*if (!(ResourceChanged is null))
                ResourceChanged(resourceOfProduct.ID_Resource);*/
            Thread thread = new Thread(() =>
            {
                ResourceViewModel.ReloadUsing();
            });
            thread.Start();       
            CommonViewModel.IndexResourcesViewModel.ResourcesChanged -= IndexResourcesViewModel_ResourcesChanged;
        }

        public void DeleteWithoutReload()
        {
            ResourceOfProductDAL.RemoveResourceOfProduct(resourceOfProduct);
            CommonViewModel.IndexResourcesViewModel.ResourcesChanged -= IndexResourcesViewModel_ResourcesChanged;

        }

        public void ReloadResources()
        {
            resources = new List<string>();
            FromStrToResource = new Dictionary<string, Resource>();
            foreach (var r in ResourceDAL.GetResources())
            {
                resources.Add(r.Name);
                FromStrToResource[r.Name] = r;
            }
            OnPropertyChanged("resources");
        }

     }
}
