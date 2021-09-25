using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.InitialData
{
    public static class FixedCostData
    {
        public static List<FixedCost> FixedCosts_Incoming { get; private set; }

        public static List<FixedCost> FixedCosts_Outcoming { get; private set; }

        public static List<FixedCost> FixedCosts_Admort { get; private set; }

        public static double Summ_Incoming { get; private set; }

        public static double Summ_Outcomin_Decrease { get; private set; }


        public static double Summ_admort { get; private set; }

        public static double Summ_Outcomin_NotDecrease { get; private set; }

        public static void Init()
        {
            var vattype = SettingsDAL.GetSettings().VAT_type;
            FixedCosts_Incoming = FixedCostDAL.GetFixedCosts(TypeOfFixedCosts.incoming).ToList();
            FixedCosts_Outcoming = FixedCostDAL.GetFixedCosts(TypeOfFixedCosts.outcoming,false).ToList();
            FixedCosts_Admort = FixedCostDAL.GetFixedCosts(TypeOfFixedCosts.admortization).ToList();

            Summ_admort = 0;
            foreach(var fc in FixedCosts_Admort)
            {
                if(!(fc.Price is null))
                Summ_admort += UnitDAL.TranslateToCommonUnit((long)fc.UnitOfPriceID, (double)fc.Price);
            }
            Summ_Incoming = 0;
            foreach(var fc in FixedCosts_Incoming)
            {
                if (!(fc.Price is null))
                    Summ_Incoming += UnitDAL.TranslateToCommonUnit((long)fc.UnitOfPriceID, (double)fc.Price);
            }

            Summ_Outcomin_Decrease = 0;
            foreach(var fc in FixedCosts_Outcoming.Where(f=> f.IsDecrease == true && vattype== VATType.full))
            {
                if (!(fc.Price is null))
                    Summ_Outcomin_Decrease += UnitDAL.TranslateToCommonUnit((long)fc.UnitOfPriceID, (double)fc.Price);
            }

            Summ_Outcomin_NotDecrease = 0;
            foreach (var fc in FixedCosts_Outcoming.Where(f => f.IsDecrease == false || f.IsDecrease is null || vattype!=VATType.full))
            {
                if (!(fc.Price is null))
                    Summ_Outcomin_NotDecrease += UnitDAL.TranslateToCommonUnit((long)fc.UnitOfPriceID, (double)fc.Price);
            }
        }


    }
}
