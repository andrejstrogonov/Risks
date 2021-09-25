using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DataProcessLibrary
{

    public class Point:INotifyPropertyChanged
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Color HelpColor { get; set; }
        private bool isSelected;
        public bool IsSelected {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public string ColorName { get
            {
                return HelpColor.ToString();
            }
        }

        public bool IsRecomended { get; set; }

        public Point()
        {
            X = 0;
            Y = 0;
            IsSelected = false;
            IsRecomended = false;
        }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
            IsRecomended = false;
            IsSelected = false;
        }

        public Point(double x, double y, Color color)
        {
            X = x;
            Y = y;
            HelpColor = color;
            IsRecomended = false;
            IsSelected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

}
