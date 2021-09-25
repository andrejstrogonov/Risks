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
    public class OpenProjectCommand:ICommand
    {
        private CommonViewModel CommonViewModel;

        public OpenProjectCommand(CommonViewModel ipvm)
        {
            this.CommonViewModel = ipvm;
            this.CommonViewModel.PropertyChanged += new PropertyChangedEventHandler(product_PropertyChanged);
        }

        public event EventHandler CanExecuteChanged;

        private void product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            /*if (!CommonViewModel.ProjectViewModel.OpenProject())
            {
                System.Windows.MessageBox.Show("Выбранная папка не является корневой для проекта");
            }*/
        }
    }
}
