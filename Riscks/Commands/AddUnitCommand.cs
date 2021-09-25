using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class AddUnitCommand:ICommand
    {
        private UnitSequenceViewModel UnitSequence;

        public AddUnitCommand(UnitSequenceViewModel iuvm)
        {
            this.UnitSequence = iuvm;
        }

        public event EventHandler CanExecuteChanged;



        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            UnitSequence.AddUnit();
        }
    }
}
