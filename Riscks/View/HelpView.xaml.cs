using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

using Microsoft.Win32;


namespace Riscks.View
{
    /// <summary>
    /// Логика взаимодействия для HelpView.xaml
    /// </summary>
    public partial class HelpView : UserControl
    {
        private Dictionary<int, int> PageToSection;
        private Dictionary<int, int> SectionToPage;
        public HelpView()
        {
            InitializeComponent();

            PageToSection = new Dictionary<int, int>();
            SectionToPage = new Dictionary<int, int>();
            //Создание проекта в лаунчере программы
            PageToSection.Add(3, 0);
            SectionToPage.Add(0, 3);
            //Работа в менеджере проекта - создание структуры
            SectionToPage.Add(1, 3);
            //Ввод информации о номенклатурах предпрития
            /*PageToSection.Add(3, 0);
            SectionToPage.Add(0, 3);

            PageToSection.Add(3, 0);
            SectionToPage.Add(0, 3);

            PageToSection.Add(3, 0);
            SectionToPage.Add(0, 3);

            PageToSection.Add(3, 0);
            SectionToPage.Add(0, 3);*/

            var dir = @"Page_2";
            var files = Directory.GetFiles(dir);
            var path1 = @"help.mht";
            if (files.Count()>0)
            {
                path1 = files.First();
            }
            FileInfo fileInfo1 = new FileInfo(path1);
            path = fileInfo1.FullName;
            this.web_browser.Navigate(new Uri( path));
        }

        string path;
        private void DocumentViewer_PageViewsChanged(object sender, EventArgs e)
        {
            //var viewport_width = documentViewer.ViewportWidth;
            //var extent_width = documentViewer.ExtentWidth;
        }

        private void documentViewer_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void studySetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = studySetList.SelectedIndex+1;

            if(!(path is null))
            this.web_browser.Navigate(new Uri(path+"#"+index.ToString()));

        }

        private void web_browser_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void web_browser_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            
        }
    }
}
