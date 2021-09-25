using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для InputKeyWindow.xaml
    /// </summary>
    public partial class InputKeyWindow : Window
    {
        public string Key { get; set; }
        public InputKeyWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Key = textBox.Text;
            if (Key.Length < 15)
            {
                MessageBox.Show("Длина ключа должна быть больше!");
                textBox.Focus();

            }
            else
            {
                DialogResult = true;
                this.Close();
            }
        }
    }
}
