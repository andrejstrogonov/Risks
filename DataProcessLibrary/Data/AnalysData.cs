using DataProcessLibrary.Analys;
using DataProcessLibrary.InitialData;
using SQLiteLibrary.DataAccess;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary.Data
{
    public class AnalysData
    {
        public Dictionary<Product, List<RiskOfProduct>> PricesOfEachProduct { get; private set; }
        public List<Resource> Resources { get; set; }
        public Dictionary<Product, List<ResourceOfProduct>> ResourcesOfEachProduct { get; private set; }
        public Dictionary<RiskOfProduct,double> output_VAT { get; set; }
        public Dictionary<long, double> input_VAT { get; set; }

        public Dictionary<long,double> input_VAT_with_sale { get; set; }
        public Dictionary<long, double> input_VAT_of_AVC_with_sale { get; set; }
        public Dictionary<long,double> ResoursePriceWithSale { get; set; }

        public Dictionary<int, bool> ImportanceOfProducts { get; set; }
        public Dictionary<Product,double> AVCOfProducts { get; set; }

        public double CoefficientOfMainFunction { get; set; }

        public Dictionary<long, double> AVCOfProductsWithSale { get; set; }//для ресурсов из запасов
        
        public List<DataScript> dataScripts { get; set; }

        public AnalysData() { }//для сериализации не удалять

        public AnalysData(AnalysData analysData)
        {
            Resources = analysData.Resources;

            DataScript.Resources = analysData.Resources;
            PricesOfEachProduct = analysData.PricesOfEachProduct;
            ResourcesOfEachProduct = analysData.ResourcesOfEachProduct;
            AVCOfProducts = analysData.AVCOfProducts ;
            CoefficientOfMainFunction = 1;
            ImportanceOfProducts = analysData.ImportanceOfProducts;
            dataScripts = analysData.dataScripts;
            AVCOfProductsWithSale = analysData.AVCOfProductsWithSale;
            output_VAT = analysData.output_VAT;
            input_VAT = analysData.input_VAT;
            input_VAT_with_sale = analysData.input_VAT_with_sale;
            input_VAT_of_AVC_with_sale = analysData.input_VAT_of_AVC_with_sale;
            ResoursePriceWithSale = analysData.ResoursePriceWithSale;

            ImportanceOfProducts = analysData.ImportanceOfProducts;
            dataScripts = analysData.dataScripts;
        }

        public double GetAVCOfProduct(long id)
        {
            Product p = ResourcesOfEachProduct.Keys.First(p1 => p1.ID == id);
            List<ResourceOfProduct> resources = ResourcesOfEachProduct[p].ToList();
            double sum = 0;
            foreach (var r in resources)
            {
               // var r1 = ResourceOfProductDAL.TransferToCommon(r);
               // r1.Resource = ResourceDAL.TransferToCommon(r1.Resource);
                sum += (double)r.AmountOfResource * (double)r.Resource.Price;
            }
            sum += (double)ProductDAL.TransferToCommon(p).OtherCosts;
            return sum;
        }


        public AnalysData(bool needInit=true)
        {
            if (needInit)
                InitialData.InitialData.Init();

            
            ReloadChangableData();
            CoefficientOfMainFunction = 1;
            ImportanceOfProducts = new Dictionary<int, bool>();
            dataScripts = new List<DataScript>();
            AVCOfProductsWithSale = new Dictionary<long, double>();
            output_VAT = new Dictionary<RiskOfProduct, double>();
            input_VAT = new Dictionary<long, double>();
            input_VAT_with_sale = new Dictionary<long, double>();
            input_VAT_of_AVC_with_sale = new Dictionary<long, double>();
            ResoursePriceWithSale = new Dictionary<long, double>();

        }

        public void ReloadChangableData()
        {
            Resources = ResourceData.Resources();
            DataScript.Resources = Resources;
            PricesOfEachProduct = RiskOfProductData.PricesOfEachProduct();
            ResourcesOfEachProduct = ResourceOfProductData.ResourcesOfEachProducts();
            AVCOfProducts = CountAVCOfEachProduct();

        }

        private Dictionary<Product, double> CountAVCOfEachProduct()
        {
            Dictionary<Product, double> avc = new Dictionary<Product, double>();
            foreach(var p in ProductData.Products())
            {
                avc[p] = GetAVCOfProduct(p.ID);
            }
            return avc;
        }


    }
}
