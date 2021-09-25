using ProjectManagerLibrary.WorkWithProjects;
using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.InitialData
{
    public static class InitialData
    {
        public static Settings Settings
        {
            get {
                return SettingsDAL.GetSettings();
            }
            private set
            {
            }
        }

        public static void Init()
        {
            ProductData.Init();
            ResourceData.Init();
            ResourceOfProductData.Init();
            RiskOfProductData.Init();
            GroupsData.Init();
            FixedCostData.Init();
        }

        public static HierarhyElem_Analys HierarhyElem_Analys { get; set; }

        public static HierarhyElem_Analys LoadAnalyses(Project project)
        {
            if(HierarhyElem_Analys is null || HierarhyElem_Analys.HierarhyElem!=project)
                HierarhyElem_Analys = new HierarhyElem_Analys(project);
            HierarhyElem_Analys.LoadFromCommonAnalysSettings(project.AnalysSettingsSetuper.Get().SerializeHierarhy);
            return HierarhyElem_Analys;
        }

        public static void ReloadCommonAnalysSettings(Project project)
        {
            HierarhyElem_Analys.LoadFromCommonAnalysSettings(project.AnalysSettingsSetuper.Get().SerializeHierarhy);
        }

        public static HierarhyElem_Analys GetAnalys(HierarhyElem hierarhyElem)
        {
            return HierarhyElem_Analys.Find(hierarhyElem);
        }

    }
}
