using DataProcessLibrary.Flexibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class ProfitFlexibilityViewModel:ViewModel
    {
        public double NewMeaning
        {
            get
            {
                return ProfitFlexibility.NewMeaning;
            }
            set
            {
                ProfitFlexibility.NewMeaning = value;
                double change = ProfitFlexibility.Sign*( ProfitFlexibility.NewMeaning - ProfitFlexibility.LastMeaning) ;
                double factProcentChange = change / ProfitFlexibility.LastMeaning;
                double procentchangeofprofit = ProfitFlexibility.Coefficient_Flexibility * factProcentChange;
                double absolutchangeofprofit = ProfitFlexibility.Profit_Change_Absolut / ProfitFlexibility.Profit_Change_Procent * procentchangeofprofit;
                profitFromChange = Math.Round( absolutchangeofprofit,2);
                Result =Math.Round( profitFromChange,2);
                Posibility = true;
                OnPropertyChanged(nameof(ProfitFromChange));
                OnPropertyChanged(nameof(NewMeaning));
            }
        }

        private string comment;
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        private double profitFromChange;
        public double ProfitFromChange
        {
            get
            {
                return profitFromChange;
            }
            private set { }
        }

        public ProfitFlexibility ProfitFlexibility { get; private set; }

        public ProfitFlexibilityViewModel(ProfitFlexibility profitFlexibility)
        {
            ProfitFlexibility = profitFlexibility;
            ProfitFlexibility.NewMeaning =ProfitFlexibility.LastMeaning+ Math.Round(profitFlexibility.LastMeaning * profitFlexibility.Procent *ProfitFlexibility.Sign,3);
            profitFromChange = profitFlexibility.Profit_Change_Absolut;
            
        }

        public string Changing
        {
            get
            {
                return (ProfitFlexibility.Sign * ProfitFlexibility.Procent * 100).ToString();
            }
            private set { }
        }

        public string TypeOfParameter {
            get
            {
                switch (ProfitFlexibility.TypeOfParameter)
                {
                    case DataProcessLibrary.Flexibility.TypeOfParameter.AVCOfResources:return "Себестоимость ресурса";
                    case DataProcessLibrary.Flexibility.TypeOfParameter.GarantOfProduct:return "Гарантированное количество товара";
                    case DataProcessLibrary.Flexibility.TypeOfParameter.MaximumOfProduct:return "Максимум товара";
                    case DataProcessLibrary.Flexibility.TypeOfParameter.MinimumOfProduct:return "Минимум товара";
                    case DataProcessLibrary.Flexibility.TypeOfParameter.MaxAmountOfResource:return "Максимум ресурса";
                    case DataProcessLibrary.Flexibility.TypeOfParameter.ResourceOfProductConsumption: return "Количество ресурса в товаре";
                }
                return "другое";
            } 
            private set { }
        }
        public string Parameter_Change {
            get
            {
               return ProfitFlexibility.Parameter_Change;
            }
            private set { }
        }

        public string LastMeaning
        {
            get
            {
                return ProfitFlexibility.LastMeaning + " " + ProfitFlexibility.UnitOfMeaning;
            }
            private set { }
        }

        public string Profit_Change_Procent
        {
            get
            {
                return (ProfitFlexibility.Profit_Change_Procent*100).ToString();
            }
            private set { }
        }

        public double Coefficient_Flexibility {
            get
            {
                return ProfitFlexibility.Coefficient_Flexibility;
            }
            private set
            {
            }
        }
        public double Profit_Change_Absolut
        {
            get
            {
                return ProfitFlexibility.Profit_Change_Absolut;
            }
            private set
            {

            }
        }
        public bool? Posibility {
            get
            {
                return ProfitFlexibility.Posibility;
            }
            set
            {
                if (value==true)
                {
                    Result = ProfitFromChange - (ProfitFlexibility.PriceOfChange.HasValue ? ProfitFlexibility.PriceOfChange.Value : 0);
                }
                else
                    Result = 0;
                ProfitFlexibility.Posibility = value;
                OnPropertyChanged("Posibility");
            }
        }
        public double? PriceOfChange {
            get
            {
                return ProfitFlexibility.PriceOfChange;
            }
            set
            {
                ProfitFlexibility.PriceOfChange = value;
                Result = - (ProfitFlexibility.PriceOfChange.HasValue?ProfitFlexibility.PriceOfChange.Value:0) + ProfitFromChange;
                Posibility = true;
                OnPropertyChanged("PriceOfChange");
            }
        }

        


        public double Result {
            get
            {
                return ProfitFlexibility.Result;
            }
            private set
            {
                ProfitFlexibility.Result = value;
                OnPropertyChanged("Result");
            }
        }

    }
}
