using Riscks.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riscks.Commands
{
    public class DeleteUnitSeqCommand : ICommand
    {
        private IndexUnitViewModel indexUnitViewModel;

        public DeleteUnitSeqCommand(IndexUnitViewModel indexUnitViewModel)
        {
            this.indexUnitViewModel = indexUnitViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return !(indexUnitViewModel.CurrentSeq is null) && (indexUnitViewModel.CurrentSeq.CanDeleteSequence());
        }

        public void Execute(object parameter)
        {
            indexUnitViewModel.DeleteUnitSeq();
        }
    }
}
