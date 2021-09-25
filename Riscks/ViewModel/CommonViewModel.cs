using DataProcessLibrary;
using DataProcessLibrary.InitialData;
using ProjectManagerLibrary.WorkWithProjects;
using Riscks.Commands;
using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerLibrary;
using License = ClientServerLibrary.License;
using System.Windows;

namespace Riscks.ViewModel
{
    public class CommonViewModel:ViewModel
    {
        public HierarhyElem HierarhyElem { get; set; }

        public HierarhyElem_Analys HierarhyElem_Analys { get; set; }

        public HistoryViewModel HistoryViewModel_Products { get; set; }
        public HistoryViewModel HistoryViewModel_Resources { get; set; }
        public HistoryViewModel HistoryViewModel_FixedCosts { get; set; }
        public HistoryViewModel HistoryViewModel_Settings { get; set; }
        public IndexResourcesViewModel IndexResourcesViewModel { get; set; }
        public IndexProductViewModel IndexProductViewModel { get; set; }
        public IndexGroupViewModel IndexGroupViewModel { get; set; }
        public AnalysViewModel AnalysViewModel { get; set; }
        public FlexibilityAnalysViewModel FlexibilityAnalysViewModel { get; set; }
        public BothFixedCostsViewModel BothFixedCostsViewModel { get; set; }

        public CommonViewModel(HierarhyElem hierarhyElem):base(null)
        {
            HierarhyElem = hierarhyElem;
            Init(hierarhyElem);
        }

        public void ReloadUnits(string from)
        {
            try
            {
                HierarhyElem.CopyUnitsFrom(from);
                if (HierarhyElem is Session)
                {
                    IndexResourcesViewModel.ReloadUnits();
                    IndexProductViewModel.ReloadUnits();
                    IndexGroupViewModel.ReloadUnits();
                    BothFixedCostsViewModel.ReloadUnits();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        public bool Init(HierarhyElem hierarhyElem)
        {            
            AnalysViewModel = new AnalysViewModel(this);
            
            //if (HierarhyElem is Session)
            {
                HistoryViewModel_Products = new HistoryViewModel(TypeOfHistoryItem.Product, hierarhyElem);
                HistoryViewModel_Resources = new HistoryViewModel(TypeOfHistoryItem.Resource, hierarhyElem);
                HistoryViewModel_FixedCosts = new HistoryViewModel(TypeOfHistoryItem.FixedCost, hierarhyElem);
                HistoryViewModel_Settings = new HistoryViewModel(TypeOfHistoryItem.Settings, hierarhyElem);
          
                IndexResourcesViewModel = new IndexResourcesViewModel(this);
                IndexProductViewModel = new IndexProductViewModel(this);
                IndexGroupViewModel = new IndexGroupViewModel(this);
                BothFixedCostsViewModel = new BothFixedCostsViewModel(this);
                FlexibilityAnalysViewModel = new FlexibilityAnalysViewModel(this);

                IndexProductViewModel.OnProductsChanged += IndexResourcesViewModel.ReloadUsing;
            }
            /*else
            {
                HistoryViewModel_Products = null;
                HistoryViewModel_Resources = null;
                HistoryViewModel_FixedCosts = null;
                HistoryViewModel_Settings = null;

                IndexResourcesViewModel = null;
                IndexProductViewModel = null;
                IndexGroupViewModel = null;
                BothFixedCostsViewModel = null;
                FlexibilityAnalysViewModel = null;
            }*/
            return true;
        }

        public void Unselect()
        {
            if (HierarhyElem is Session)
            {
                IndexProductViewModel.CurrentProduct = null;
                IndexResourcesViewModel.CurrentResource = null;
            }

        }   

        public void ReloadFixedCosts()
        {
            BothFixedCostsViewModel.IndexFixedCostViewModel_Out.Reload();
            IndexProductViewModel.ReloadVATVisilibity();
            IndexResourcesViewModel.ReloadVATVisilibity();
            ReloadInterface();
        }

        public void ReloadInterface()
        { 
            OnPropertyChanged(nameof(IndexResourcesViewModel));
            OnPropertyChanged(nameof(IndexProductViewModel));
            //IndexProductViewModel.ReloadInterface();
            OnPropertyChanged(nameof(BothFixedCostsViewModel));
            AnalysViewModel.ReloadInterface();
            OnPropertyChanged(nameof(AnalysViewModel));
        }

        
    }

    public class IndexCommonViewModel : ViewModel
    {       
        public static bool IsFullVAT { get; private set; }
        public Dictionary<HierarhyElem, CommonViewModel> Elems { get; set; }

        private CommonViewModel currentViewModel;
        public CommonViewModel CurrentElement
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                currentViewModel = value;
                OnPropertyChanged(nameof(CurrentElement));
            }
        }

        public bool CanDeleteUnit(Unit unit)
        {
            return directoryTree.CanDeleteUnit(unit);
        }

        public IndexUnitViewModel IndexUnitViewModel { get; set; }
        public ProjectViewModel ProjectViewModel { get; set; }

        public int SelectedLevel { get; set; }
        public int SelectedLevel_HierarhyParent { get; set; }

        public bool ProductOpened = true;
        //public HierarhyElem_Analys HierarhyElem_Analys { get; set; }

        public DirectoryTree directoryTree { get; private set; }
        public bool Init(DirectoryTree directoryTree)
        {
            this.directoryTree = directoryTree;
            InitialData.LoadAnalyses(directoryTree.SelectedProject.HierarhyElem as Project);
            IndexUnitViewModel = new IndexUnitViewModel(this);
            Elems = new Dictionary<HierarhyElem, CommonViewModel>();
            /*foreach (var p1 in directoryTree.NodesTree)
            {
                Elems[p1.HierarhyElem] = new CommonViewModel(p1.HierarhyElem);
                Elems[p1.HierarhyElem].FollowIndex(this);
                if (!(p1.Nodes is null))
                    foreach (var p2 in p1.Nodes)
                    {
                        Elems[p2.HierarhyElem] = new CommonViewModel(p2.HierarhyElem);
                        Elems[p2.HierarhyElem].FollowIndex(this);
                        if (!(p2.Nodes is null))
                            foreach (var p3 in p2.Nodes)
                            {
                                Elems[p3.HierarhyElem] = new CommonViewModel(p3.HierarhyElem);
                                Elems[p3.HierarhyElem].FollowIndex(this);
                            }
                    }
            }*/
            ProjectViewModel = new ProjectViewModel(directoryTree);
            ProjectViewModel.OnChangeLevel += ChangeLevel;
            ProjectViewModel.PropertyChanged += ProjectViewModel_PropertyChanged;
            if (!ProjectViewModel.Init())
                return false;

            
            //CurrentElement =Elems[ directoryTree.SelectedProject.CurrentNode.HierarhyElem];
            //OnPropertyChanged(nameof(CurrentElement));
            IsFullVAT = SettingsDAL.GetSettings().VAT_type == VATType.full;     

            return true;
        }

        private void ProjectViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentNode")
            {
                //if (!(CurrentElement is null))
                //    SaveChanges();
                if(!Elems.ContainsKey(ProjectViewModel.CurrentNode.HierarhyElem))
                {
                    var cvm= new CommonViewModel(ProjectViewModel.CurrentNode.HierarhyElem);
                    if (ProjectViewModel.CurrentNode.Property is NodeProperty_Session)
                        (ProjectViewModel.CurrentNode.Property as NodeProperty_Session).OnVATTypeChanged += cvm.ReloadFixedCosts;
                    Elems[ProjectViewModel.CurrentNode.HierarhyElem] = cvm;
                }
                CurrentElement = Elems[ProjectViewModel.CurrentNode.HierarhyElem];
                CurrentElement.HierarhyElem.Activate();
                var projectDBString = Elems.First(p => p.Key is Project).Value.HierarhyElem.DBString;
                if (Elems.Count()>0 && UnitDAL.UnitsWereChange(projectDBString))
                {
                    CurrentElement.ReloadUnits(projectDBString);
                }
                //OnPropertyChanged(nameof(CurrentElement));
                //currentViewModel.AnalysViewModel.ReloadInterface();
                CurrentElement.ReloadInterface();
            }
        }

        public void SaveChanges()
        {
            CurrentElement.HierarhyElem.SaveChanges();
            //ProjectViewModel.DirectoryTree.SelectedProject.HierarhyElem.SaveChanges();
        }

        public void ChangeLevel(int level)
        {
            SelectedLevel = level;
            SelectedLevel_HierarhyParent = SelectedLevel == 2 ? 1 : 0;
            OnPropertyChanged(nameof(SelectedLevel_HierarhyParent));
            OnPropertyChanged(nameof(SelectedLevel));
        }

        private bool wasinited = false;
        public IndexCommonViewModel(DirectoryTree directoryTree):base(null)
        {
            if(License.LicenseKey=="" || License.LicenseKey is null || !License.ActivateLicense(License.LicenseKey))
            {
                throw new ApplicationException("Лицензия отсутствует");
            }
            Init(directoryTree);
        }

        private bool unitschanged = false;
        public void ReloadUnits()
        {
            string from = SampleContext.Path;
            foreach(var c in Elems)
            {
                try
                {
                    c.Value.ReloadUnits(from);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    currentViewModel.HierarhyElem.Activate();
                    return;
                }
            }
            currentViewModel.HierarhyElem.Activate();
            unitschanged = true;
        }
    }
}
