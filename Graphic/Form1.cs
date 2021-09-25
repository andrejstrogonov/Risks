using DataProcessLibrary;
using SimplexLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Graphic
{
    public partial class Graphic : Form
    {
        public Graphic()
        {
            InitializeComponent();
        }

        private Dictionary<double, KeyValuePair<DataScript, Decidion>> graphic;

        public Graphic(Dictionary<double,KeyValuePair<DataScript, Decidion>> _graphic)
        {
            InitializeComponent();
            graphic = _graphic;
            DrawGraphic();
        }

        private void DrawGraphic()
        {
            chrtGraphic.ChartAreas.Clear();
            chrtGraphic.ChartAreas.Add(new ChartArea("График"));
            Series series = new Series();
                       series.ChartType = SeriesChartType.Line;
            series.Color = Color.AliceBlue;
            foreach (var p in graphic) {
                series.Points.AddXY(p.Key, p.Value.Value.MaxResult);
            }
            series.MarkerSize = 6;
            series.MarkerStyle = MarkerStyle.Circle;
            chrtGraphic.Series.Add(series);
        }
    }
}
