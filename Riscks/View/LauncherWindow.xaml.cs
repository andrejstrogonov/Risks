using Riscks.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using ClientServerLibrary;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using ProjectManagerLibrary.WorkWithProjects;
using Riscks.ViewModel;

namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для LauncherWindow.xaml
    /// </summary>
    public partial class LauncherWindow : Window
    {
        public LauncherWindow()
        {
            WindowConstants.WindowConstantsInit();
            InitializeComponent();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(
    typeof(FrameworkElement),
    new FrameworkPropertyMetadata(
    System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)
    )
    );
            }
            catch(Exception e) { }
            ((LauncherViewModel)DataContext).LauncherWindow = this;
        }

        public System.Windows.Size GetElementPixelSize(UIElement element)
        {
            Matrix transformToDevice;
            var source = PresentationSource.FromVisual(element);
            if (source != null)
                transformToDevice = source.CompositionTarget.TransformToDevice;
            else
                using (var source1 = new HwndSource(new HwndSourceParameters()))
                    transformToDevice = source1.CompositionTarget.TransformToDevice;

            if (element.DesiredSize == new System.Windows.Size())
                element.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

            return (System.Windows.Size)transformToDevice.Transform((Vector)element.DesiredSize);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            ClosingApp_L();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState= WindowState.Minimized;
            ShowInTaskbar = true;
        }

        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.Close();
        }



        private void CreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateProjectWindow createProjectWindow = new CreateProjectWindow();
                createProjectWindow.Show();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+'\n'+ex.StackTrace);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderTransform_L();
            PresentationSource source = PresentationSource.FromVisual(this);
            double dpiX, dpiY;
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }           
        }
 
        private void RenderTransform_L()
        {
            string key = "";
            bool isnew = false;
            if (!License.ReadKeyFromFile())
            {
                isnew = true;
                if (!TransformSize_L(ref key))
                    this.Close();
            }
            else
            {
                key = License.LicenseKey;
            }
            int i = 0;
            for (i=0;i<3 && !License.ActivateLicense(key);i++)
            {
                if (!TransformSize_L(ref key))
                    this.Close();
            }
            if (i == 3)
            {
                this.Close();
            }
        }
        
        private void ClosingApp_L()
        {
            License.StopUsing();
        }

        private bool TransformSize_L(ref string key)
        {
            InputKeyWindow inputKeyWindow = new InputKeyWindow();
            if (inputKeyWindow.ShowDialog() is true)
            {
                key = inputKeyWindow.Key;
                return true;
            }
            return false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //StopUsingLicense();
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                if (!HierarhyElem.TryCopyProject(directoryInfo.FullName))
                {
                    MessageBox.Show("Данный путь не может быть открыт");
                }
                (DataContext as LauncherViewModel).ReloadProjects();
            }
        }
    }
}
