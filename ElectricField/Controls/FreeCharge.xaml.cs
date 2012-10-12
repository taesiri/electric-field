using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElectricField.Classes;
using ElectricField.Graph;
using ElectricField.SettingsPages;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for FreeCharge.xaml
    /// </summary>
    public partial class FreeCharge 
    {
        public Point StartPoint { get; set; }
        public Vector LastVector { get; set; }
        public List<PointStatistics> VisitedPoints; 

        public Charge MyCharge;
        
        private Point _previousLocation;
        private Transform _previousTransform;

        public FreeCharge()
        {
            InitializeComponent();
            VisitedPoints = new List<PointStatistics>();
        }
        public void Restart()
        {
            this.Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Window wnd = Window.GetWindow(this);
            Point currentLocation = e.MouseDevice.GetPosition(wnd);

            var move = new TranslateTransform(
                    currentLocation.X - _previousLocation.X, currentLocation.Y - _previousLocation.Y);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var group = new TransformGroup();
                if (_previousTransform != null)
                {
                    group.Children.Add(_previousTransform);
                }
                group.Children.Add(move);

                this.RenderTransform = group;


            }
            else
            {
                this.Cursor = Cursors.Hand;
            }

            _previousLocation = currentLocation;
            _previousTransform = this.RenderTransform;


            MainWindow.Instance.CalculatePositions();

            base.OnMouseMove(e);
        }

        private void FreeChargeOnSpaceMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Generate Data Chart
            var mychart = new DataChart(VisitedPoints, "Position/Time Chart", "", "Position", "Time");

            mychart.ShowDialog();

        }
        public void ShowSettings()
        {
            var settingpage = new FreeChargeSettings(this);
            settingpage.ShowDialog();
        }

    }
}
