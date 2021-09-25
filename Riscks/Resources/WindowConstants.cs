using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Riscks.Resources
{
    public class WindowConstants
    {
        public static double CommonFontSize = 14;//внутритабличный
        public static  double HeaderFontSize = 16;//
        public static  double ButtonFontSize = 16;//16
        public static double LabelLauncherFontSize = 14;
        public static double LabelFontSize = 20;//
        public static double LabelFontSize_Big = 20;//
        public static  double MultiLine_HeaderFontSize = 14;


        public static  double ActualWidth = 1018;
        public static double ActualWidth_LeftPanel = 150;
        public static  double SliderHeight = 14;
        public static  double SliderWidth = 12;
        public static  double RowHeight = 36;
        public static  double ImageHeight = 15;
        public static  double ImageWidth = 16;
        public static  double BarSeriesWidth = 200;
        public static  double FullChartWidth = 400;
        public static  double MenuBetweenWidth = 20;
        public static  double ComboBoxToggleWidth = 26;

        public static  double ProductGridHeight = 400;
        public static  double GraphicHeight = 300;
        public static  double ButtonWidth = 100;
        public static  double ButtonHeight = 30;

        public static  double ArrowsHeight = 30;
        public static  double MaxColumnWidth = 120;
        public static  double MaxColumnWidth_Commentary = 120;

        public static  Thickness CommonMargin = new Thickness(15,10,15,10);
        public static  Thickness ButtonPadding = new Thickness(15, 10, 15, 10);

        public static  Thickness ButtonMargin_First = new Thickness(0, 10, 0, 10);
        public static  Thickness ButtonMargin_Last = new Thickness(10, 10, 0, 10);
        public static  Thickness LabelMargin = new Thickness(15, 10, 15, 10);

        public static  Thickness LabelMargin1 = new Thickness(15, 10, 0, 10);

        public static  Thickness LabelMargin2 = new Thickness(0, 10, 15, 10);
        public static  Thickness ThickMargin = new Thickness(15,7,15,7);
        public static  Thickness SliderMargin = new Thickness(10, 2, 10, 2);
        public static  Thickness CellMargin = new Thickness(15, 3, 15, 3);
        public static  Thickness MenuPadding = new Thickness(10, 10, 20, 10);
        public static  Thickness MenuMargin = new Thickness(10, 10, 10, 10);
        public static  Thickness HeaderRightMargin = new Thickness(20, 4, 4, 4);
        public static  Thickness UnderComboBoxMargin = new Thickness(15, 7, 41, 7);

        public static  double WidthOfCheckBox = 14;
        public static  double WidthOfCheck = 17;
        public static double HelpWindowHeight = 500;
        public static  double HelpWindowWidth = 600;

        public static double LauncherWindowHeight = 500;
        public static double LauncherWindowWidth = 600;

        public static double DpiX;
        public static double DpiY;

        private static double coefdpi;

        public static double Margin = 30;


        public static double getPXFromDesign(float designpx)
        {
            return designpx * coefdpi;
        }

        public static double getDPI(double x, double y, double d)
        {
            double diagp = Math.Sqrt(x * x + y * y);
            return diagp/d;
        }
        

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("User32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        const int LOGPIXELSX = 117;
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        const int LOGPIXELSY = 116;

        public static double Convert(double pixel, double DPI)
        {
            return pixel / DPI * DpiY;
        }
    
        public static double width;
        public static double height;
        public static double diag;
        public static void WindowConstantsInit()
        {            
            //PointF dpi = PointF.Empty;
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            DpiX = (int)dpiXProperty.GetValue(null, null);
            DpiY = (int)dpiYProperty.GetValue(null, null);

            //IntPtr hdc = GetDC(IntPtr.Zero);
            // Graphics g = Graphics.FromHdc(IntPtr.Zero);
            //DpiX = GetDeviceCaps(hdc, LOGPIXELSX);
            //DpiY = GetDeviceCaps(hdc, LOGPIXELSY);
            //ReleaseDC(IntPtr.Zero, hdc);

            width = System.Windows.SystemParameters.WorkArea.Width;
            height = System.Windows.SystemParameters.WorkArea.Height;


            Margin = Math.Round(width * 0.023);
            if (Margin < 30)
            {
                Margin = 30;
            }
            if (Margin > 44)
            {
                Margin = 44;
            }


            double dppi = getDPI(1920, 1080, 23);
            double mppi = getDPI(width, height, diag);
            coefdpi = mppi / dppi;


            CommonFontSize = 14;//14 0,013
            HeaderFontSize = 16; //16  0,0148
            ButtonFontSize = 16;  //16 0,0167
            LabelFontSize = 18;//18 0,022
            LabelFontSize_Big = 46; //46 0,0426
            LabelLauncherFontSize = 15;//15  0,0139

            MenuBetweenWidth = Math.Round(height * 0.011, 2);
            ComboBoxToggleWidth = Math.Round(height * 0.025, 2);
            WidthOfCheckBox = Math.Round(height * 0.02, 2);
            WidthOfCheck = WidthOfCheckBox / 2;
            MultiLine_HeaderFontSize = HeaderFontSize;
            ButtonWidth = Math.Round(height * 0.14, 2);
            ButtonHeight = Math.Round(height * 0.045, 2);
            ArrowsHeight = Math.Round(height * 0.04, 2);
            MaxColumnWidth = CommonFontSize * 6;
            MaxColumnWidth_Commentary = MaxColumnWidth * 1.5;


            double marginHor = Math.Round(height * 0.0213, 2);
            double marginVerCom = Math.Round(height * 0.01, 2);
            double marginVerThink = Math.Round(height * 0.007, 2);
            CommonMargin = new Thickness(marginHor, marginVerCom, marginHor, marginVerCom);
            ThickMargin = new Thickness(marginVerThink, marginVerThink, marginVerThink, marginVerThink);
            LabelMargin = new Thickness(marginHor+4, marginVerThink, marginHor+4, marginVerThink);
            LabelMargin1 = new Thickness(marginHor + 4, 0, 0, marginVerThink);
            LabelMargin2 = new Thickness(0, 0, 0, marginVerThink);

            CellMargin = new Thickness(marginHor-1, marginVerThink, marginVerThink, marginVerThink);
            MenuPadding = new Thickness(marginHor*1.5 , marginVerThink, marginHor*1.5, marginVerThink);
            MenuMargin = new Thickness(HeaderFontSize*0.5, HeaderFontSize, HeaderFontSize*0.5, HeaderFontSize);
            HeaderRightMargin = new Thickness(marginHor, marginVerThink, marginVerThink, marginVerThink);
            UnderComboBoxMargin = new Thickness(marginVerThink, marginVerThink, marginVerThink + ComboBoxToggleWidth+3, marginVerThink);
            ButtonPadding = new Thickness(marginHor, marginVerThink, marginHor, marginVerThink);
            ButtonMargin_First = new Thickness(0, marginVerCom, 0, marginVerCom);
            ButtonMargin_Last = new Thickness(marginVerCom, marginVerCom, 0, marginVerCom);

            double sliderLR= Math.Round(height * 0.011, 2);
            double sliderTB= Math.Round(height * 0.0036, 2);
            SliderMargin = new Thickness(sliderLR, sliderTB, sliderLR, sliderTB);

            SliderHeight = Math.Round(height * 0.03, 2);
            SliderWidth = Math.Round(height * 0.022, 2);
            RowHeight = Math.Round(height * 0.042, 2);
            ImageHeight = RowHeight - 8;
            ImageWidth = ImageHeight + 4;
            ActualWidth = Math.Round(Math.Min(height*1.35, width));
            ActualWidth_LeftPanel = Math.Round(Math.Min(height * 0.4, width));
            ProductGridHeight = Math.Round(height * 0.42, 2);
            GraphicHeight = Math.Round(height * 0.517, 2);
            FullChartWidth = ActualWidth / 3;
            BarSeriesWidth = FullChartWidth / 2;

            HelpWindowHeight = height * 0.7+40;
            HelpWindowWidth = height * 0.785 + 40;

            LauncherWindowHeight = height * 0.7+40;
            LauncherWindowWidth = height*0.785 + 40;
        }
    }
}
