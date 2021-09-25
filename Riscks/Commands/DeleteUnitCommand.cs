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
    public class DeleteUnitCommand:ICommand
    {
        
            private UnitSequenceViewModel UnitSequence;

            public DeleteUnitCommand(UnitSequenceViewModel iuvm)
            {
                this.UnitSequence = iuvm;
            }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }



        public bool CanExecute(object parameter)
            {
                return !(UnitSequence.CurrentUnit is null) && UnitSequence.CurrentUnit.IsEnabled && UnitSequence.CurrentUnit.CanDeleteUnit;
            }

            public void Execute(object parameter)
            {
                UnitSequence.DeleteUnit();
            }
        
    }
}
