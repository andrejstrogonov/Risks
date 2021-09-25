using Riscks.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using static Riscks.ViewModel.DecidionScriptViewModel;
using DataPoint = System.Windows.Controls.DataVisualization.Charting.DataPoint;


namespace Riscks.View
{
    public class ListBoxItem_WithImage:ListBoxItem
    {
        public static readonly DependencyProperty ImagePathProperty;
        public static readonly DependencyProperty TextProperty;

        static ListBoxItem_WithImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxItem_WithImage), new FrameworkPropertyMetadata(typeof(ListBoxItem_WithImage)));

            // Initialize ImagePath dependency properties
            ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(System.Windows.Media.ImageSource), typeof(ListBoxItem_WithImage), new UIPropertyMetadata(null));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ListBoxItem_WithImage), new UIPropertyMetadata(null));
        }

        public System.Windows.Media.ImageSource ImagePath
        {
            get { return (System.Windows.Media.ImageSource)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }



    public class PieDataPoint : System.Windows.Controls.DataVisualization.Charting.PieDataPoint
    {

        public static readonly DependencyProperty TextedGeometryProperty =
            DependencyProperty.Register("TextedGeometry", typeof(Geometry), typeof(PieDataPoint));

        public Geometry TextedGeometry
        {
            get { return (Geometry)GetValue(TextedGeometryProperty); }
            set { SetValue(TextedGeometryProperty, value); }
        }

        public static readonly DependencyProperty RectGeometryProperty =
            DependencyProperty.Register("RectGeometry", typeof(Geometry), typeof(PieDataPoint));

        public Geometry RectGeometry
        {
            get { return (Geometry)GetValue(RectGeometryProperty); }
            set { SetValue(RectGeometryProperty, value); }
        }

        static PieDataPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieDataPoint),
                new FrameworkPropertyMetadata(typeof(PieDataPoint)));
        }

        public PieDataPoint()
        {
            DependencyPropertyDescriptor dependencyPropertyDescriptor
                = DependencyPropertyDescriptor.FromProperty(GeometryProperty, GetType());

            dependencyPropertyDescriptor.AddValueChanged(this, OnGeometryValueChanged);
        }

        public PieDataPoint(SolidColorBrush back)
        {
            Background = back;
            DependencyPropertyDescriptor dependencyPropertyDescriptor
                = DependencyPropertyDescriptor.FromProperty(GeometryProperty, GetType());

            dependencyPropertyDescriptor.AddValueChanged(this, OnGeometryValueChanged);
        }

        private double LabelFontSize
        {
            get
            {
                FrameworkElement parentFrameworkElement = Parent as FrameworkElement;
                return Math.Max(8, Math.Min(parentFrameworkElement.ActualWidth,
                    parentFrameworkElement.ActualHeight) / 30);
            }
        }

        private void OnGeometryValueChanged(object sender, EventArgs arg)
        {
            Point point=new Point();
            double FontSize = 14;
            FormattedText formattedText;
            double rIn = 5;
            double rOut = 8;

            double coef = rIn / rOut + (rOut - rIn) / rOut / 2;

       

            CombinedGeometry combinedGeometry = new CombinedGeometry();
            combinedGeometry.GeometryCombineMode = GeometryCombineMode.Union;

            formattedText = new FormattedText(Math.Round(Ratio*100).ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Calibri"),
                15,
                new SolidColorBrush(Color.FromRgb(255,255,255)));
            //formattedText.SetFontWeight(FontWeights.SemiBold);

            if (ActualRatio == 1)
            {
                EllipseGeometry ellipseGeometry = Geometry as EllipseGeometry;
                
                point = new Point(ellipseGeometry.Center.X - formattedText.Width / 2,
                    ellipseGeometry.Center.Y + ellipseGeometry.RadiusY*coef - formattedText.Height / 2);
                double len=ellipseGeometry.RadiusY*(rOut-rIn)/rOut;
                len -= 3;
                FontSize = Math.Round(len * 0.5);
            }
            else if (ActualRatio*100<=3)
            {
                TextedGeometry = null;
                return;
            }
            else  
            {
                Point tangent;
                Point half;
                Point origin;


                PathGeometry pathGeometry = Geometry as PathGeometry;                
                pathGeometry.GetPointAtFractionLength(0.5, out half, out tangent);
                pathGeometry.GetPointAtFractionLength(0, out origin, out tangent);

                /* Point vector = new Point(half.X - origin.X, half.Y - origin.Y);
                 double d = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
                 vector.X /= d;
                 vector.Y /= d;*/

                /*point = new Point(half.X-vector.X*(20)  - formattedText.Width / 2,
                    half.Y-vector.Y*(20)- formattedText.Height / 2);
                    */
                    Point vector =new Point(half.X - origin.X, half.Y - origin.Y);
                vector.X *= (rOut-rIn)/rOut;
                vector.Y *= (rOut - rIn) / rOut;
                double len = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
                len -= 3;
                FontSize = Math.Round(len * 0.5);                
                point = new Point(origin.X + (half.X-origin.X)*coef - formattedText.Width / 2,
                origin.Y + (half.Y - origin.Y) * coef - formattedText.Height / 2);
            }


            formattedText.SetFontSize(FontSize);
            combinedGeometry.Geometry1 = Geometry;
            Rect rect = new Rect();
            rect.Width = formattedText.Width + 4;
            rect.Height = formattedText.Height + 4;
            rect.X = point.X-2;
            rect.Y = point.Y-2;
            combinedGeometry.Geometry1 = new RectangleGeometry(rect);
            combinedGeometry.Geometry2 = formattedText.BuildGeometry(point);

            RectGeometry = new RectangleGeometry(rect);
            TextedGeometry = formattedText.BuildGeometry(point);                  
        }
    }

    public class PieSeries : System.Windows.Controls.DataVisualization.Charting.PieSeries
    {
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            int cnt = 0;
            if (!(canvas is null))
                cnt = canvas.Children.Count;
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null) && cnt > 0)
            {
                var cnt2 = canvas.Children.Count;
                canvas.Children.RemoveRange(0, cnt);
            }
        }

        protected override DataPoint CreateDataPoint() {
            /*Dictionary<Color, int> colors = new Dictionary<Color, int>();
            for (int i = 0; i < Palette.Count(); i++)
            {
                colors[(Palette[i]["Background"] as SolidColorBrush).Color] = 0;
            }
            foreach(var i in ActiveDataPoints)
            {
                colors[(i.Background as SolidColorBrush).Color] += 1;
            }
            int min = colors.Values.Min();
            var color = colors.First(p => p.Value == min).Key;*/

            var lgcnt = this.LegendItems.Count();



            var colorbrush = Palette[lgcnt%Palette.Count]["Background"] as SolidColorBrush;
            if(LegendItems.Count()==ActiveDataPoints.Count() && ActiveDataPoints.Any(p=> p.Background == colorbrush))
            {
                Dictionary<Color, int> colors = new Dictionary<Color, int>();
                for (int i = 0; i < Palette.Count(); i++)
                {
                    colors[(Palette[i]["Background"] as SolidColorBrush).Color] = 0;
                }
                foreach (var i in ActiveDataPoints)
                {
                    colors[(i.Background as SolidColorBrush).Color] += 1;
                }
                int min = colors.Values.Min();
                colorbrush= new SolidColorBrush( colors.First(p => p.Value == min).Key);
            }
            if (lgcnt != ActiveDataPoints.Count() && ActiveDataPoints.Count()>0)
            {
                colorbrush = ActiveDataPoints.Last().Background as SolidColorBrush;
            }

            var point = new PieDataPoint();
            point.Background = colorbrush;
            return point;  
            //return new PieDataPoint();
        }
 
    }

    public class Prop: ViewModel.ViewModel, IComparable, IConvertible
    {
        private double Max;

        private double first;
        public double First { get
            {
                return first;
            }
            set
            {
                first = value;
                Max = Math.Max(first, second);
                OnPropertyChanged(nameof(First));
            }
        }
        private double second;
        public double Second
        {
            get
            {
                return second;
            }
            set
            {
                second = value;
                Max = Math.Max(first, second);
                OnPropertyChanged(nameof(Second));
            }
        }


        public int CompareTo(object obj)
        {
            double thismax = Math.Max(First, Second);
            double num = 0;
            if (obj is Prop)
            {
                num = Math.Max(((Prop)obj).First, ((Prop)obj).Second);              
                
            }
            else
            {
                if(obj is double)
                    num = (double)obj;
                else
                {
                    num = 0;
                }
            }
            return thismax.CompareTo(num);
        }

        public TypeCode GetTypeCode()
        {
            return Max.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)Math.Max(first,second)).ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToDecimal(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Max).ToDateTime(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Max.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Max).ToType(conversionType, provider);
        }
    }

    public class DoubleColumnDataPoint : System.Windows.Controls.DataVisualization.Charting.ColumnDataPoint
    {

        public static readonly DependencyProperty FirstHeightProperty =
            DependencyProperty.Register("FirstHeight", typeof(double), typeof(DoubleColumnDataPoint));

        public double FirstHeight
        {
            get
            {
                return (double)GetValue(FirstHeightProperty);
            }
            set
            {
                SetValue(FirstHeightProperty, value);
            }
        }

        public static readonly DependencyProperty SecondHeightProperty =
            DependencyProperty.Register("SecondHeight", typeof(double), typeof(DoubleColumnDataPoint));

        public double SecondHeight
        {
            get
            {
                return (double)GetValue(SecondHeightProperty);
            }
            set
            {
                SetValue(SecondHeightProperty, value);
            }
        }


        static DoubleColumnDataPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleColumnDataPoint),
                new FrameworkPropertyMetadata(typeof(DoubleColumnDataPoint)));
        }

        private List<RiskOfProductForDiagram> Source;
        public DoubleColumnDataPoint(List<RiskOfProductForDiagram> source)
        {
            Source = source;
            DependencyPropertyDescriptor dependencyPropertyDescriptor
                = DependencyPropertyDescriptor.FromProperty(DependentValueProperty, GetType());


            DependencyPropertyDescriptor dependencyPropertyDescriptor2
                = DependencyPropertyDescriptor.FromProperty(HeightProperty, GetType());

            dependencyPropertyDescriptor2.AddValueChanged(this, OnHeightValueChanged);
        }

        public double firstval;
        public double secondval;

        private void OnHeightValueChanged(object sender, EventArgs arg)
        {
            if (!(Source is null) && Source.Any(p => p.Name == (string)IndependentValue))
            {
                var riskOfProductForDiagram = Source.First(p => p.Name == (string)IndependentValue);
                this.firstval = riskOfProductForDiagram.Prop.First;
                this.secondval = riskOfProductForDiagram.Prop.Second;
                var max = Math.Max(firstval, secondval);
                FirstHeight = Height / max * firstval;
                SecondHeight = Height / max * secondval;
            }
        }
    }

    public class DoubleColumnSeries : System.Windows.Controls.DataVisualization.Charting.ColumnSeries
    {
        
        protected override DataPoint CreateDataPoint()
        {
            return new DoubleColumnDataPoint((List<RiskOfProductForDiagram>)this.ItemsSource);
            
        }

        protected override void RemoveDataPoint(DataPoint dataPoint)
        {
            base.RemoveDataPoint(dataPoint);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            int cnt = 0;
            if (!(canvas is null))
            {
                cnt = canvas.Children.Count;
            }
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null) && cnt > 0)
            {
                var cnt2 = canvas.Children.Count;
                canvas.Children.RemoveRange(0, cnt);
                canvas.Children.Capacity = cnt2 - cnt + 1;
            }
        }
    }

    public class HorizontalColumnDataPoint : System.Windows.Controls.DataVisualization.Charting.BarDataPoint
    {

        public static readonly DependencyProperty ColorWidthProperty =
            DependencyProperty.Register("ColorWidth", typeof(double), typeof(HorizontalColumnDataPoint));

        public double ColorWidth
        {
            get
            {
                return (double)GetValue(ColorWidthProperty);
            }
            set
            {
                SetValue(ColorWidthProperty, value);
            }
        }

        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(HorizontalColumnDataPoint));

        public double TextWidth
        {
            get
            {
                return (double)GetValue(TextWidthProperty);
            }
            set
            {
                SetValue(TextWidthProperty, value);
            }
        }


        static HorizontalColumnDataPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalColumnDataPoint),
                new FrameworkPropertyMetadata(typeof(HorizontalColumnDataPoint)));
        }
        private static double canvaswidth;


        public void HorizontalColumnSeries_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvaswidth = e.NewSize.Width;
            Width = canvaswidth;
            TextWidth = maxtextlength;
            if (maxval == 0)
            {
                ColorWidth = 0;
            }
            else
                ColorWidth = (Width - TextWidth) / maxval * (double)ActualDependentValue;
        }


        public HorizontalColumnDataPoint()
        {
            DependencyPropertyDescriptor dependencyPropertyDescriptor
                = DependencyPropertyDescriptor.FromProperty(DependentValueProperty, GetType());

            dependencyPropertyDescriptor.AddValueChanged(this, OnDependentValueChanged);

            DependencyPropertyDescriptor dependencyPropertyDescriptor2
                = DependencyPropertyDescriptor.FromProperty(WidthProperty, GetType());

            dependencyPropertyDescriptor2.AddValueChanged(this, OnWidthValueChanged);
            Width = canvaswidth;
            TextWidth = maxtextlength;
            if (maxval == 0)
            {
                ColorWidth = 0;
            }
            else
                ColorWidth = (Width - TextWidth) / maxval * (double)ActualDependentValue;
        }

        public bool widthchanged = false;


        public static double maxval=0;
        public static double maxtextlength=0;
        private void OnDependentValueChanged(object sender, EventArgs arg)
        {
            var formattedText = new FormattedText(ActualDependentValue.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Calibri"),
                15,
                new SolidColorBrush(Color.FromRgb(255, 255, 255)));
            var textwidth = formattedText.Width + 10;
            if (maxval < (double)DependentValue)
            {
                maxval = (double)DependentValue;
                //Changed();
            }
            if (textwidth > maxtextlength)
            {
                maxtextlength = textwidth;
            }
            Width = canvaswidth;
            TextWidth = maxtextlength;
            if (maxval == 0)
            {
                ColorWidth = 0;
            }
            else
                ColorWidth = (Width - TextWidth) / maxval * (double)ActualDependentValue;
        }
        private void OnWidthValueChanged(object sender, EventArgs arg)
        {            
            if (Width != canvaswidth)
            {
                Canvas.SetLeft(this, 0);
                Width = canvaswidth;
                TextWidth = maxtextlength;
                if (maxval == 0)
                {
                    ColorWidth = 0;
                }
                else
                    ColorWidth = (Width - TextWidth) / maxval * (double)ActualDependentValue;
            }
        }

    }


    public class HorizontalColumnSeries : System.Windows.Controls.DataVisualization.Charting.BarSeries
    {     
        protected override DataPoint CreateDataPoint()
        {

            var cnt = ItemsSource is null?0: ((ObservableCollection<ResourceIntake>)ItemsSource).Count();
            if (ActiveDataPointCount == 0)
            {
                HorizontalColumnDataPoint.maxval = 0;
                HorizontalColumnDataPoint.maxtextlength = 0;
            }
            var elem = new HorizontalColumnDataPoint();
            var index = Math.Abs( ActiveDataPointCount )%10;
            elem.Background = ((System.Windows.Controls.DataVisualization.Charting.Chart)this.SeriesHost).Palette[index]["Background"] as SolidColorBrush;
            var color = (elem.Background as SolidColorBrush).Color;
            elem.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)(color.R * 0.9), (byte)(color.G * 0.9),(byte)(color.B * 0.9)));
            this.SizeChanged += elem.HorizontalColumnSeries_SizeChanged;
            
            return elem;
        }

        protected override void RemoveDataPoint(DataPoint dataPoint)
        {
            this.SizeChanged -= ((HorizontalColumnDataPoint)dataPoint).HorizontalColumnSeries_SizeChanged;
            dataPoint.Visibility = Visibility.Hidden;
            base.RemoveDataPoint(dataPoint);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            int cnt = 0;
            if (!(canvas is null))
                cnt = canvas.Children.Count;
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null) && cnt > 0)
            {
                var cnt2 = canvas.Children.Count;
                canvas.Children.RemoveRange(0, cnt);
            }
        }
    }

    public class HorizontalColumnSeries_Flexibility : HorizontalColumnSeries
    {
        protected override DataPoint CreateDataPoint()
        {

            var cnt = ItemsSource is null ? 0 : ((List<DiagramItem>)ItemsSource).Count();
            if (ActiveDataPointCount == 0)
            {
                HorizontalColumnDataPoint.maxval = 0;
                HorizontalColumnDataPoint.maxtextlength = 0;
            }
            var elem = new HorizontalColumnDataPoint();
            var index = Math.Abs(ActiveDataPointCount) % 10;
            elem.Background = ((System.Windows.Controls.DataVisualization.Charting.Chart)this.SeriesHost).Palette[index]["Background"] as SolidColorBrush;
            var color = (elem.Background as SolidColorBrush).Color;
            elem.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)(color.R * 0.9), (byte)(color.G * 0.9), (byte)(color.B * 0.9)));
            this.SizeChanged += elem.HorizontalColumnSeries_SizeChanged;

            return elem;
        }

        protected override void RemoveDataPoint(DataPoint dataPoint)
        {
            this.SizeChanged -= ((HorizontalColumnDataPoint)dataPoint).HorizontalColumnSeries_SizeChanged;
            dataPoint.Visibility = Visibility.Hidden;
            base.RemoveDataPoint(dataPoint);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            int cnt = 0;
            if (!(canvas is null))
                cnt = canvas.Children.Count;
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null) && cnt > 0)
            {
                var cnt2 = canvas.Children.Count;
                canvas.Children.RemoveRange(0, cnt);
            }
        }
    }


    public class ColumnSeries_Gistogram : System.Windows.Controls.DataVisualization.Charting.ColumnSeries
    {
        protected override DataPoint CreateDataPoint()
        {           
            var elem = new ColumnDataPoint();
            int cnt = ((System.Windows.Controls.DataVisualization.Charting.Chart)this.SeriesHost).Palette.Count();
            var index = Math.Abs(this.ActiveDataPointCount) % cnt;
            elem.Background = ((System.Windows.Controls.DataVisualization.Charting.Chart)this.SeriesHost).Palette[index]["Background"] as SolidColorBrush;
            var color = (elem.Background as SolidColorBrush).Color;
            elem.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)(color.R * 1.1), (byte)(color.G * 1.1), (byte)(color.B * 1.1)));
            return elem;
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            int cnt = 0;
            if(!(canvas is null))
                cnt = canvas.Children.Count;
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null) && cnt > 0)
            {
                var cnt2 = canvas.Children.Count;
                canvas.Children.RemoveRange(0, cnt);
            }
        }
    }

    public class TripleColumnDataPoint : System.Windows.Controls.DataVisualization.Charting.ColumnDataPoint
    {

        public static readonly DependencyProperty FirstHeightProperty =
            DependencyProperty.Register("FirstHeight", typeof(double), typeof(TripleColumnDataPoint));

        public double FirstHeight
        {
            get
            {
                return (double)GetValue(FirstHeightProperty);
            }
            set
            {
                SetValue(FirstHeightProperty, value);
            }
        }

        public static readonly DependencyProperty SecondHeightProperty =
            DependencyProperty.Register("SecondHeight", typeof(double), typeof(TripleColumnDataPoint));

        public double SecondHeight
        {
            get
            {
                return (double)GetValue(SecondHeightProperty);
            }
            set
            {
                SetValue(SecondHeightProperty, value);
            }
        }


        static TripleColumnDataPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TripleColumnDataPoint),
                new FrameworkPropertyMetadata(typeof(TripleColumnDataPoint)));
        }

        public TripleColumnDataPoint()
        {
            DependencyPropertyDescriptor dependencyPropertyDescriptor
                = DependencyPropertyDescriptor.FromProperty(DependentValueProperty, GetType());

            dependencyPropertyDescriptor.AddValueChanged(this, OnDependentValueChanged);

            DependencyPropertyDescriptor dependencyPropertyDescriptor2
                = DependencyPropertyDescriptor.FromProperty(HeightProperty, GetType());

            dependencyPropertyDescriptor2.AddValueChanged(this, OnHeightValueChanged);
        }

        public double firstval;
        public double secondval;

        private void OnDependentValueChanged(object sender, EventArgs arg)
        {
            if (DependentValue is Prop)
            {
                firstval = (double)((Prop)DependentValue).First;
                secondval = (double)((Prop)DependentValue).Second;
                var val = firstval+secondval;
                DependentValue = val;
            }
        }

        private void OnHeightValueChanged(object sender, EventArgs arg)
        {
            var max = firstval+secondval;
            FirstHeight = Height / max * firstval;
            SecondHeight = Height / max * secondval;
        }
    }

    public class TripleColumnSeries : System.Windows.Controls.DataVisualization.Charting.ColumnSeries
    {

        protected override DataPoint CreateDataPoint()
        {
            return new TripleColumnDataPoint();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            int cnt = 0;
            if (!(canvas is null))
                cnt = canvas.Children.Count;
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null) && cnt > 0)
            {
                var cnt2 = canvas.Children.Count;
                canvas.Children.RemoveRange(0, cnt);
            }
        }
    }

    public class LineSeries_Custom: LineSeries
    {
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        { 
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            var cntwas = 0;
            if(!(canvas is null))
            {
                cntwas = canvas.Children.Count>0?canvas.Children.Count-1:0;
            }
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null))
            {
                var cntneed= ActiveDataPoints.Count();
                var cntnow= canvas.Children.Count > 0 ? canvas.Children.Count -1 : 0;
                if (cntneed != cntnow)
                {
                    canvas.Children.RemoveRange(1, cntwas);
                }
            }
        }        

        protected override DataPoint CreateDataPoint()
        {
            return new LineDataPoint();
        }
    }

    public class ScatterSeries_Custom: ScatterSeries
    {
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            Canvas canvas = this.GetTemplateChild("PlotArea") as Canvas;
            var cntwas = 0;
            if (!(canvas is null))
            {
                cntwas =  canvas.Children.Count;
            }
            base.OnItemsSourceChanged(oldValue, newValue);
            if (!(canvas is null))
            {
                var cntneed = ActiveDataPoints.Count();
                var cntnow =  canvas.Children.Count ;
                if (cntneed != cntnow)
                {
                    canvas.Children.RemoveRange(0, cntwas);
                }
            }
        }
    }
}
