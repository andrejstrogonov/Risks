using DataProcessLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riscks.ViewModel
{
    public class ResourceScriptViewModel : ViewModel
    {
        public ResourceScript resourceScript;
        public ResourceScriptViewModel(ResourceScript resourceScript)
        {
            this.resourceScript = resourceScript;
        }

        public string ResourceName
        {
            get
            {
                return resourceScript.Resource.Name;
            }
        }

        public string AmountToBuy
        {
            get
            {
                return resourceScript.AmountToBuy.ToString("N", CultureInfo.CurrentCulture) +" "+resourceScript.Unit.Symbol;
            }
        }

        public string AmountFromStock
        {
            get
            {
                return resourceScript.AmountFromStock.ToString("N", CultureInfo.CurrentCulture) + " " + resourceScript.Unit.Symbol;
            }
        }

        public string FullAmount
        {
            get
            {
                return (resourceScript.AmountFromStock + resourceScript.AmountToBuy).ToString("N", CultureInfo.CurrentCulture) + " " + resourceScript.Unit.Symbol;
            }
        }
    }
}
