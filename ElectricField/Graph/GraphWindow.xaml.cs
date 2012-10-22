using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace ElectricField.Graph
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow
    {
        public GraphWindow(string verctName, string horizName)
        {
            InitializeComponent();
            verticalAxisName.Content = verctName;
            HorizontalAxisName.Content = horizName;
        }

        public void DrawThis(IEnumerable<Point> values, Brush penBrush, Brush circlePoints, string chartDescription)
        {
            if (values != null)
            {
                var time = new int[values.Count()];
                var value = new int[values.Count()];

                for (int i = 0; i < values.Count(); i++)
                {
                    if (values != null)
                    {
                        time[i] = (int) values.ToArray()[i].X;
                        value[i] = (int) values.ToArray()[i].Y;
                    }
                }

                var timesDataSource = new EnumerableDataSource<int>(time);
                timesDataSource.SetXMapping(x => timeAxis.ConvertToDouble(x));

                var xPointsDataSource = new EnumerableDataSource<int>(value);
                xPointsDataSource.SetYMapping(y => Convert.ToInt32(y));

                var compositeDataSource = new CompositeDataSource(xPointsDataSource, timesDataSource);

                plotter.AddLineGraph(compositeDataSource,
                                     new Pen(penBrush, 2),
                                     new CirclePointMarker {Size = 8.0, Fill = circlePoints},
                                     new PenDescription(chartDescription));

                plotter.Viewport.FitToView();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }


        public void DrawThis(IEnumerable<int> value, IEnumerable<int> time, Brush penBrush, Brush circlePoints)
        {
            var timesDataSource = new EnumerableDataSource<int>(value);
            timesDataSource.SetXMapping(x => timeAxis.ConvertToDouble(x));

            var xPointsDataSource = new EnumerableDataSource<int>(time);
            xPointsDataSource.SetYMapping(y => Convert.ToInt32(y));

            var compositeDataSource = new CompositeDataSource(xPointsDataSource, timesDataSource);

            plotter.AddLineGraph(compositeDataSource,
                                 new Pen(penBrush, 2),
                                 new CirclePointMarker {Size = 8.0, Fill = circlePoints},
                                 new PenDescription(""));

            plotter.Viewport.FitToView();
        }

        public void DrawThis(IEnumerable<int> value, IEnumerable<int> time)
        {
            var timesDataSource = new EnumerableDataSource<int>(value);
            timesDataSource.SetXMapping(x => timeAxis.ConvertToDouble(x));

            var xPointsDataSource = new EnumerableDataSource<int>(time);
            xPointsDataSource.SetYMapping(y => Convert.ToInt32(y));

            var compositeDataSource = new CompositeDataSource(xPointsDataSource, timesDataSource);

            plotter.AddLineGraph(compositeDataSource,
                                 new Pen(Brushes.GreenYellow, 2),
                                 new CirclePointMarker {Size = 8.0, Fill = Brushes.IndianRed},
                                 new PenDescription(""));

            plotter.Viewport.FitToView();
        }
    }
}