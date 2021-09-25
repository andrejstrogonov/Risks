using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class AddUnitSeqCommand : ICommand
    {

        public IndexUnitViewModel indexUnitViewModel;
        public AddUnitSeqCommand(IndexUnitViewModel indexUnitViewModel)
        {
            this.indexUnitViewModel = indexUnitViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            indexUnitViewModel.AddUnitSeq();
        }
    }
}
