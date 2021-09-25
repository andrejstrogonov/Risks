using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SQLiteLibrary.Model;
using SQLiteLibrary.DataAccess;
using Riscks.Commands;
using DataProcessLibrary;

namespace Riscks.ViewModel
{
    public delegate void CurrentPointChanged(Point newPoint);

    public class GraphicViewModel : ViewModel
    {
        
        public List<Point> Points { get; set; }
        public Point currentPoint;
        public Point CurrentPoint
        {
            get
            {
                return currentPoint;
            }
            set
            {
                if(!(currentPoint is null))
                    currentPoint.IsSelected = false;
                OnPropertyChanged("CurrentPoint");
                currentPoint = value;
                if (!(currentPoint is null))
                    currentPoint.IsSelected = true;
                OnPropertyChanged("CurrentPoint");
                if(!(OnChanged is null))
                    OnChanged(value);
            }
        }

      

        public GraphicViewModel(List<Point> points)
        {
            Points = points;
            if (points.Count() > 0)
                currentPoint = points.First();
            //OnPropertyChanged(nameof(Points));
            //if (Points.Count > 0)
            //  currentPoint = points.First();
            //else
            //currentPoint = new Point();
        }

        public void Reload(ObservableCollection<Point> points)
        {
            Points.Clear();
            foreach(var p in points)
            {
                Points.Add(p);
            }
            if (points.Count() > 0)
                currentPoint = points.First();
            OnPropertyChanged(nameof(Points));
        }

        public event CurrentPointChanged OnChanged;

        public void ChangePoint(Point newPoint)
        {
            if (!(newPoint is null) && ((currentPoint is null) || currentPoint.X!=newPoint.X))
            {
                if (!(currentPoint is null))
                    currentPoint.IsSelected = false;
                OnPropertyChanged(nameof(CurrentPoint));
                currentPoint = Points.Where(p => p.X == newPoint.X).FirstOrDefault();
                if(!(currentPoint is null))
                    currentPoint.IsSelected = true;
                OnPropertyChanged("CurrentPoint");
            }
        }

        public double GetY(double X)
        {
            foreach (var p in Points)
                if (p.X == X)
                    return p.Y;
            return 0;
        }

        public void Clear()
        {
            Points.Clear();
            if (!(CurrentPoint is null))
            {
                CurrentPoint.IsSelected = false;
            }
            currentPoint = null;
            OnPropertyChanged(nameof(Points));
        }

    }
}
