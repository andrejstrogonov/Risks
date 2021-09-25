using DataProcessLibrary.Data;
using SQLiteLibrary;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Analys
{
    public static class TAXStage
    {
        public static void SetTax(ref AnalysData AnalysData)
        {
            switch (InitialData.InitialData.Settings.VAT_type)
            {
                case VATType.full: Full(ref AnalysData);return;
                case VATType.main_15: Main_15(ref AnalysData); return;
                case VATType.None: None(ref AnalysData); return;
            }
            Main_6(ref AnalysData);
        }

        private static void Main_15(ref AnalysData analysData)
        {
            analysData.CoefficientOfMainFunction = 0.85;
            foreach (var kvp in analysData.PricesOfEachProduct)
            {
                for (int i = 0; i < kvp.Value.Count(); i++)
                {
                    var elem = (analysData.PricesOfEachProduct[kvp.Key])[i];
                    analysData.output_VAT[elem] = 0;
                }
            }

            for (int i = 0; i < analysData.Resources.Count(); i++)
            {
                var res = analysData.Resources[i];
                analysData.input_VAT[res.ID] = 0;
            }
            foreach (var p in analysData.ResourcesOfEachProduct)
            {
                analysData.input_VAT_of_AVC_with_sale[p.Key.ID] = 0;
            }
        }

        private static void None(ref AnalysData analysData)
        {
            analysData.CoefficientOfMainFunction = 1;
            foreach (var kvp in analysData.PricesOfEachProduct)
            {
                for (int i = 0; i < kvp.Value.Count(); i++)
                {
                    var elem = (analysData.PricesOfEachProduct[kvp.Key])[i];
                    analysData.output_VAT[elem] = 0;
                }
            }

            for (int i = 0; i < analysData.Resources.Count(); i++)
            {
                var res = analysData.Resources[i];
                analysData.input_VAT[res.ID] = 0;
            }
            foreach (var p in analysData.ResourcesOfEachProduct)
            {
                analysData.input_VAT_of_AVC_with_sale[p.Key.ID] = 0;
            }
        }


        private static void Main_6(ref AnalysData analysData)
        {
            analysData.CoefficientOfMainFunction = 1;
            foreach (var kvp in analysData.PricesOfEachProduct)
            {
                for (int i = 0; i < kvp.Value.Count(); i++)
                {

                    var elem = (analysData.PricesOfEachProduct[kvp.Key])[i];
                    analysData.output_VAT[elem] = Math.Round(elem.Price * 0.06, 3);
                }
            }

            for (int i = 0; i < analysData.Resources.Count(); i++)
            {
                var res = analysData.Resources[i];
                analysData.input_VAT[res.ID] =0;
                
            }
            foreach (var p in analysData.ResourcesOfEachProduct)
            {
                analysData.input_VAT_of_AVC_with_sale[p.Key.ID] = 0;
            }

        }

        private static void Full(ref AnalysData analysData)
        {
            analysData.CoefficientOfMainFunction = 0.8;
            foreach (var kvp in analysData.PricesOfEachProduct)
            {
                for (int i = 0; i < kvp.Value.Count(); i++)
                {

                    var elem = (analysData.PricesOfEachProduct[kvp.Key])[i];
                    analysData.output_VAT[elem] =Math.Round( elem.Price * elem.Product.Rate / (elem.Product.Rate + 1),3);
                }
            }
            for(int i=0;i<analysData.Resources.Count();i++)
            {
                var res = analysData.Resources[i];
                analysData.input_VAT[res.ID] = Math.Round(res.Price * res.InputTax / (res.InputTax + 1),2);
                analysData.input_VAT_with_sale[res.ID] = Math.Round(analysData.ResoursePriceWithSale[res.ID] * res.InputTax / (res.InputTax + 1), 2);

            }
            foreach (var p in analysData.ResourcesOfEachProduct)
            {
                analysData.input_VAT_of_AVC_with_sale[p.Key.ID] = 0;
                foreach(var res in p.Value)
                {
                    analysData.input_VAT_of_AVC_with_sale[p.Key.ID] += Math.Round( res.AmountOfResource * analysData.input_VAT_with_sale[res.ID_Resource],2);
                }
            }
        }
    }
}
