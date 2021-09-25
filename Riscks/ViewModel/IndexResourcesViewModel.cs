using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SQLiteLibrary.Model;
using System.Collections.ObjectModel;
using SQLiteLibrary.DataAccess;
using System.Windows.Input;
using Riscks.Commands;
using DataProcessLibrary.Analys;
using System.Windows;
using DataProcessLibrary.InitialData;
using System.Threading;

namespace Riscks.ViewModel
{
    public class IndexResourcesViewModel : ViewModel
    {
        public string AxisHeader
        {
            get
            {
                return "Стоимость, " + TransferOfUnitsDAL.GetCommonUnitForType(TypeOfUnit.Money).Symbol;
            }
        }
        public class ResourceAVC_Diagram:ViewModel
        {
            public ResourceViewModel ResourceViewModel { get; }
            public double Price
            {
                get
                {
                    var resource = ResourceDAL.GetResource(ResourceViewModel.Resource_ID);
                    if (resource is null)
                        return 0;
                    var value = ResourceViewModel.Price.HasValue ? ResourceViewModel.Price.Value : 0;
                    return UnitDAL.TranslateToCommonUnit(resource.UnitOfPriceID , value);
                }
            }

            public string Name
            {
                get
                {
                    return ResourceViewModel.Name;
                    
                }
            }
            public ResourceAVC_Diagram(ResourceViewModel resourceViewModel)
            {
                ResourceViewModel = resourceViewModel;
                ResourceViewModel.PropertyChanged += ResourceViewModel_PropertyChanged;
            }

            private void ResourceViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if(e.PropertyName=="Price" || e.PropertyName == "UnitOfPriceSymbol")
                {
                    OnPropertyChanged(nameof(Price));
                }
                else
                {
                    if (e.PropertyName == "Name")
                    {
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }
        }

        //public SetInfinityCommand SetInfinityCommand { get; set; }
        public ObservableCollection<ResourceAVC_Diagram> ResourceAVC_Diagrams { get; set; }

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

        private Dictionary<string, Unit> FromSymbolToUnit;

        public List<string> unitsDefaultSymbolsPrice { get; set; }

        public List<string> unitsDefaultSymbolsAmount { get; set; }


        private Unit defaultUnitPrice;

        public string UnitDefaultSymbolPrice
        {
            get
            {
                return defaultUnitPrice.Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    defaultUnitPrice = FromSymbolToUnit[value];
                    OnPropertyChanged(nameof(UnitDefaultSymbolPrice));
                }
            }
        }

        private Unit defaultUnitAmount;

        public string UnitDefaultSymbolAmount
        {
            get
            {
                return defaultUnitAmount.Symbol;
            }
            set
            {
                if (!(value is null) && FromSymbolToUnit.ContainsKey(value))
                {
                    defaultUnitAmount = FromSymbolToUnit[value];
                    OnPropertyChanged(nameof(UnitDefaultSymbolAmount));
                }
            }
        }
        
        private ResourceViewModel currentResource;

        public ResourceViewModel CurrentResource
        {
            get
            {
                return currentResource;
            }
            set
            {
                currentResource = value;
                OnPropertyChanged("CurrentResource");
                if (!(value is null))
                {
                    //currentResource.Save();
                    //OnPropertyChanged("CurrentResource");
                }
            }
        }

        private Dictionary<long, ResourceViewModel> resources_dict;
        
        public ObservableCollection<ResourceViewModel> Resources { get; set; }
        

        public IndexResourcesViewModel(CommonViewModel commonViewModel)
        {
            CommonViewModel = commonViewModel;
            Init();

        }

        public void Init()
        {
            FromSymbolToUnit = new Dictionary<string, Unit>();
            unitsDefaultSymbolsPrice = new List<string>();
            unitsDefaultSymbolsAmount = new List<string>();
            var money = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Money);
            var time = UnitSequenceDAL.GetUnitSequence(TypeOfUnit.Time);
            foreach (var u in UnitDAL.GetUnits())
            {
                if (u.UnitSequence_ID == money.ID)
                {
                    unitsDefaultSymbolsPrice.Add(u.Symbol);
                }
                else
                if (u.UnitSequence_ID != time.ID)
                {
                    unitsDefaultSymbolsAmount.Add(u.Symbol);
                }
                FromSymbolToUnit[u.Symbol] = u;
            }
            defaultUnitPrice = FromSymbolToUnit[unitsDefaultSymbolsPrice[0]];
            defaultUnitAmount = FromSymbolToUnit[unitsDefaultSymbolsAmount[0]];

            OnPropertyChanged(nameof(UnitDefaultSymbolAmount));
            OnPropertyChanged(nameof(UnitDefaultSymbolPrice));

            Resources = new ObservableCollection<ResourceViewModel>();
            resources_dict = new Dictionary<long, ResourceViewModel>();
            foreach (var r in ResourceDAL.GetResourcesNullable())
            {
                var res = new ResourceViewModel(r, CommonViewModel);
                res.ResourceChanged += ReactOnChange;
                resources_dict[r.ID] = res;
                Resources.Add(res);
            }
            currentResource = null;
            if (Resources.Count > 0)
                currentResource = Resources.First();

            AddCommand = new AddResourceCommand(this);
            DeleteCommand = new DeleteResourceCommand(this);
            //SetInfinityCommand = new SetInfinityCommand(this);
            OnPropertyChanged(nameof(Resources));
            OnPropertyChanged(nameof(CurrentResource));
            //SimplexStage.OnProgress += IncreaseProgress;

            ResourceAVC_Diagrams = new ObservableCollection<ResourceAVC_Diagram>();
            foreach (var res in Resources)
            {
                ResourceAVC_Diagrams.Add(new ResourceAVC_Diagram(res));
            }
        }

        public void Reload()
        {
            foreach(var r in Resources)
            {
                r.ResourceChanged -= ReactOnChange;
            }

            Resources.Clear();
            resources_dict.Clear();
            foreach (var r in ResourceDAL.GetResourcesNullable())
            {
                var res = new ResourceViewModel(r, CommonViewModel);
                res.ResourceChanged += ReactOnChange;
                resources_dict[r.ID] = res;
                Resources.Add(res);
            }
            currentResource = null;
            if (Resources.Count > 0)
                currentResource = Resources.First();

            OnPropertyChanged(nameof(Resources));
            OnPropertyChanged(nameof(CurrentResource));
            //SimplexStage.OnProgress += IncreaseProgress;

            ResourceAVC_Diagrams.Clear();
            foreach (var res in Resources)
            {
                ResourceAVC_Diagrams.Add(new ResourceAVC_Diagram(res));
            }
        }

        public AddResourceCommand AddCommand { get; protected set; }
        public DeleteResourceCommand DeleteCommand { get; protected set; }

        public void ReloadResourceUsing(long id)
        {
            Thread thread = new Thread(() =>
             {
                 if (resources_dict.ContainsKey(id))
                     resources_dict[id].ReloadUsing();
             });
            thread.Start();
        }

        public event ObjectChanged ResourcesChanged;

        public void ReactOnChange(long id)
        {
            if (ResourceOfProductViewModel.FromStrToResource is null)
                ResourceOfProductViewModel.FromStrToResource = new Dictionary<string, Resource>();
            if (ResourceOfProductViewModel.FromStrToResource.Any(p => p.Value.ID == id))
            {
                var last_name = ResourceOfProductViewModel.FromStrToResource.First(p => p.Value.ID == id).Key;
                ResourceOfProductViewModel.FromStrToResource.Remove(last_name);
            }
            var newres = ResourceDAL.GetResource(id);
            if (!(newres is null))
                ResourceOfProductViewModel.FromStrToResource[newres.Name] = newres;
            if (!(ResourcesChanged is null))
                ResourcesChanged(id);
        }

        private int ContainsKey(ResourceViewModel rvm)
        {
            foreach (var r in Resources)
                if (r.Equals(rvm))
                    return Resources.IndexOf(r);
            return -1;
        }

        public void AddResource()
        {
            ResourceViewModel resourceViewModel = new ResourceViewModel(defaultUnitPrice, defaultUnitAmount, CommonViewModel);
            resourceViewModel.Save();
            resourceViewModel.ResourceChanged += ReactOnChange;
            currentResource = resourceViewModel;
            resources_dict[resourceViewModel.Resource_ID] = resourceViewModel;
            Resources.Add(resourceViewModel);
            OnPropertyChanged("Resources");
            OnPropertyChanged("CurrentResource");
            ResourceAVC_Diagrams.Add(new ResourceAVC_Diagram(resourceViewModel));
        }

        public void DeleteResource()
        {
            if (!(CurrentResource is null))
            {
                ResourceAVC_Diagrams.Remove(ResourceAVC_Diagrams.Where(d => d.ResourceViewModel.Resource_ID == currentResource.Resource_ID).First());
                long id = currentResource.Resource_ID;
                currentResource.Delete();
                Resources.Remove(CurrentResource);
                OnPropertyChanged("Resources");

            }
            
        }

        public void ReloadUnits()
        {
            foreach (var r in Resources)
               r.ReloadUnits();
        }

        public void ReloadUsing()
        {
            foreach (var r in Resources)
               r.ReloadUsing();
        }

    }
}
