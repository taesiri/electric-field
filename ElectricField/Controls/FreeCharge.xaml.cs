using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        public Charge MyCharge;
        public List<PointStatistics> VisitedPoints;

        private Point _previousLocation;
        private Transform _previousTransform;

        public FreeCharge()
        {
            InitializeComponent();
            VisitedPoints = new List<PointStatistics>();
        }

        public Point StartPoint { get; set; }
        public Vector LastVector { get; set; }

        public void Restart()
        {
            Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0);
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

                RenderTransform = group;
            }
            else
            {
                Cursor = Cursors.Hand;
            }

            _previousLocation = currentLocation;
            _previousTransform = RenderTransform;


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