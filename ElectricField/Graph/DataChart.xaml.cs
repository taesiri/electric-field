using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ElectricField.Classes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace ElectricField.Graph
{
    /// <summary>
    /// Interaction logic for DataChart.xaml
    /// </summary>
    public partial class DataChart : Window
    {
        private readonly List<PointStatistics> _visitedPoints;
        public string ChartDescription = "Chart";

        public DataChart(IEnumerable<PointStatistics> data, string chartTitle, string chartDescription,
                         string verticalAxisTitle, string horizontalAxisTitle)
        {
            InitializeComponent();
            _visitedPoints = new List<PointStatistics>(data);
            ChartTitle = chartTitle;
            VerticalAxisTitle = verticalAxisTitle;
            HorizontalAxisTitle = horizontalAxisTitle;
            ChartDescription = chartDescription;
            DrawChart();
        }

        public string ChartTitle
        {
            set { header_PlotterName.Content = value; }
            get { return (string) header_PlotterName.Content; }
        }

        public string VerticalAxisTitle
        {
            set { vatName.Content = value; }
            get { return (string) vatName.Content; }
        }

        public string HorizontalAxisTitle
        {
            set { hatName.Content = value; }
            get { return (string) hatName.Content; }
        }

        private void DrawChart()
        {
            var xpoints = new int[_visitedPoints.Count];
            var ypoints = new int[_visitedPoints.Count];
            var times = new int[_visitedPoints.Count];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = i;
                xpoints[i] = (int) _visitedPoints[i].Position.X;
                ypoints[i] = (int) _visitedPoints[i].Position.Y;
            }

            var timesDataSource = new EnumerableDataSource<int>(times);
            timesDataSource.SetXMapping(x => timeAxis.ConvertToDouble(x));

            var xPointsDataSource = new EnumerableDataSource<int>(xpoints);
            xPointsDataSource.SetYMapping(y => Convert.ToInt32(y));

            var yPointsDataSource = new EnumerableDataSource<int>(ypoints);
            yPointsDataSource.SetYMapping(y => Convert.ToInt32(y));

            var compositeDataSource1 = new CompositeDataSource(xPointsDataSource, timesDataSource);
            var compositeDataSource2 = new CompositeDataSource(yPointsDataSource, timesDataSource);


            plotter.AddLineGraph(compositeDataSource1,
                                 new Pen(Brushes.GreenYellow, 2),
                                 new CirclePointMarker {Size = 10.0, Fill = Brushes.Red},
                                 new PenDescription("x/t"));
            plotter.AddLineGraph(compositeDataSource2,
                                 new Pen(Brushes.Gold, 2),
                                 new CirclePointMarker {Size = 10.0, Fill = Brushes.DodgerBlue},
                                 new PenDescription("y/t"));

            plotter.Viewport.FitToView();
        }

        private void Button1Click(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }
    }
}