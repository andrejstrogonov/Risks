using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SQLiteLibrary.Model;
using SQLiteLibrary;
using SQLiteLibrary.DataAccess;

namespace ProjectManagerLibrary.WorkWithProjects
{
    public class Session:HierarhyElem
    {

        public Session() { }

        public Session(HierarhyElem owner, string nameOfSession):base(owner,nameOfSession)
        {
            //this.Parent = owner;
            Parent.AddElem(this);
            Activate();
            Settings settings = SettingsDAL.GetSettings();
            if (settings.Level != 3)
            {
                settings.Level = 3;
                SettingsDAL.SaveSettings(settings);
            }
        }

        public Session(HierarhyElem owner):base(owner, GenerateDefaultName(owner.DirectoryPath,"Расчет"))
        {
            //this.Parent = owner;
            Parent.AddElem(this);
            Activate();
            Settings settings = SettingsDAL.GetSettings();
            if (settings.Level != 3)
            {
                settings.Level = 3;
                SettingsDAL.SaveSettings(settings);
            }
        }

         /*
         0 - resources names
         1 - resources parameters
         2 - products names
         3 - products parameters
         4 - products composition
         5 - products risks
         */

        public Session SaveAs(string new_name, bool[] parameters, Settings settings, HierarhyElem Owner)
        {
            if (Directory.Exists(Path.Combine(Owner.DirectoryPath, new_name)))
                return null;
            Session session = new Session(Owner, new_name);
            File.Copy(this.DBPath, session.DBPath, true);
            session.Activate();
            SettingsDAL.SaveSettings(settings);
            if (!parameters[0] && !parameters[1] && !parameters[4])
                ResourceDAL.ClearAll();
            if (parameters[0] && !parameters[1])
                ResourceDAL.ClearData();
            if (!parameters[2] && !parameters[3] && !parameters[4] && !parameters[5])
                ProductDAL.ClearAll();
            if (parameters[2] && !parameters[3])
                ProductDAL.ClearData();
            if (!parameters[4])
                ResourceOfProductDAL.ClearAll();
            if (!parameters[5])
                RiskOfProductDAL.ClearAll();
            return session;
        }


    }

    
}
