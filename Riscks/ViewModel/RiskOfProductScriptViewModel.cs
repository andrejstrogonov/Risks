using SimplexPlan;
using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class RiskOfProductScriptViewModel:ViewModel
    {

        private Decidion decidion;
        public RiskOfProduct riskOfProduct;
        public RiskOfProductScriptViewModel(Decidion decidion, RiskOfProduct rop)
        {
            this.decidion = decidion;
            riskOfProduct = rop;

            amount = 0;
            if (decidion.Coefficients.ContainsKey((int)riskOfProduct.ID_Product))
            {
                amount += decidion.Coefficients[(int)riskOfProduct.ID_Product];
            }
            if (decidion.Coefficients.ContainsKey((int)-riskOfProduct.ID_Product))
            {
                amount += decidion.Coefficients[(int)-riskOfProduct.ID_Product];
            }

        }

        public string ProductName
        {
            get
            {
                return riskOfProduct.Product.Name;
            }
        } 

        public double Price
        {
            get
            {
                return riskOfProduct.Price;
            }
        }

        public double GarantAmount
        {
            get
            {
                return riskOfProduct.GarantedAmount;
            }
        }

        public double MaximumAmount
        {
            get
            {
                return riskOfProduct.MaximumAmount;
            }
        }

        private  double amount;

        public double Amount
        {
            get
            {
                return amount;
            }
        }

        public double Stock
        {
            get
            {
                if (decidion.Coefficients.ContainsKey((int)-riskOfProduct.ID_Product))
                {
                    return decidion.Coefficients[(int)-riskOfProduct.ID_Product];
                }
                return 0;
            }
        }

        public double Risk
        {
            get
            {
                if (amount == 0)
                    return 100;
                return Math.Min(100, Math.Round(GarantAmount / amount * 100,2));
            }
        }

    }
}
