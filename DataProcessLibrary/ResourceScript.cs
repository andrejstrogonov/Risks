using SQLiteLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessLibrary
{
    public class ResourceScript
    {
        public Resource Resource { get;  set; }

        public double AmountToBuy { get;  set; }

        public double AmountFromStock { get;  set; }

        public Unit Unit { get; set; }

        public ResourceScript(Resource resource, double amount, double stock, Unit unit)
        {
            Resource = resource;
            AmountToBuy = amount;
            AmountFromStock = stock;
            Unit = unit;
        }
        public ResourceScript()
        {

        }
    }

    public class ProductScript
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double Proceeds { get; set; }
        public double Exceeds { get; set; }

        public ProductScript(long ID, string Name, double Amount, double Proceeds)
        {
            this.ID = ID;
            this.Name = Name;
            this.Amount = Amount;
            this.Proceeds = Proceeds;
        }
        public ProductScript() { }
    }
}
