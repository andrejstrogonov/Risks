using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using SQLiteLibrary;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;

namespace ProjectManagerLibrary.WorkWithProjects
{
    public class Project:HierarhyElem
    {
        public Project(string Name):base(commonProjectsDirectory,Name)
        {
            //Parent = null;
            //this.parentDirectory = commonProjectsDirectory;
          
            //CheckAndFix();

            //LoadElems();

            Activate();
            Settings settings = SettingsDAL.GetSettings();
            if (settings.Level != 1)
            {
                settings.Level = 1;
                SettingsDAL.SaveSettings(settings);
            }
        }

        public Project():base()
        {

        }


        /*
         * 0-пустой проект
         * 1 - проект-расчет
         * 2 - проект-раздел-расчет
         */
        public void CreateHierarhy(int code)
        {
            switch (code)
            {
                case 1:
                    {
                        new Session(this);
                        break;
                    }
                case 2:
                    {
                        Section section = new Section(this);
                        new Session(section);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public Project(string nameOfProject,bool needload=true):base(commonProjectsDirectory,nameOfProject,needload)
        {
            Activate();
            Settings settings = SettingsDAL.GetSettings();
            if (settings.Level != 1)
            {
                settings.Level = 1;
                SettingsDAL.SaveSettings(settings);
            }
        }
        
    }
}
