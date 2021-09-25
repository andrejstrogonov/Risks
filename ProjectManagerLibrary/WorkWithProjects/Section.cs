using SQLiteLibrary;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.WorkWithProjects
{
    public class Section:HierarhyElem
    {
        public Section() { }

        public Section(HierarhyElem project, string name) : base(project, name)
        {
            //Parent = project;
            Parent.AddElem(this);
            Activate();
            Settings settings = SettingsDAL.GetSettings();
            if (settings.Level != 2)
            {
                settings.Level = 2;
                SettingsDAL.SaveSettings(settings);
            }
        }

        public Section(Project project):base(project, GenerateDefaultName(project.DirectoryPath, "Раздел"))
        {
            //Parent = project;
            Parent.AddElem(this);
            //Activate();
            Settings settings = SettingsDAL.GetSettings();
            if (settings.Level != 2)
            {
                settings.Level = 2;
                SettingsDAL.SaveSettings(settings);
            }
        }
    }
}
