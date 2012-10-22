using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ElectricField.Classes;
using ElectricField.SettingsPages;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for Surface.xaml
    /// </summary>
    public partial class Surface
    {
        public ChargeDensity MyCharge;

        private Point _previousLocation;
        private Transform _previousTransform;

        public Surface(ChargeDensity chargeDensity)
        {
            InitializeComponent();
            MyCharge = new ChargeDensity(chargeDensity);
            CheckColor();
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

        public void ShowSettings()
        {
            var setting = new SurfaceSettings(MyCharge);
            bool? result = setting.ShowDialog();
            if (result == true)
            {
                MyCharge = new ChargeDensity(setting.ChrDensity);
                MainWindow.Instance.Calculation();
                CheckColor();
            }
        }

        private void CheckColor()
        {
            if (MyCharge.Type == Charge.ChargeType.Positive)
            {
                Background = Brushes.DarkRed;
            }
            else
            {
                Background = Brushes.DodgerBlue;
            }

            if ((int) MyCharge.ElectricCharge == 0 || MyCharge.IsActive == false)
            {
                Background = Brushes.DarkGray;
            }
        }

        private void DisableCharge()
        {
            MyCharge.IsActive = false;
            //txtChargeLable.Text = ".";
            CheckColor();
            MainWindow.Instance.Calculation();
        }

        private void EnableCharge()
        {
            MyCharge.IsActive = true;
            //txtChargeLable.Text = "-";
            CheckColor();
            MainWindow.Instance.Calculation();
        }

        private void EliminateCharge()
        {
            MainWindow.Instance.RemoveExistingCharge(MyCharge, this);
        }

        private void ShowSettingsClick(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void DeleteChargeClick(object sender, RoutedEventArgs e)
        {
            EliminateCharge();
        }

        private void MnuItemDisableChecked(object sender, RoutedEventArgs e)
        {
            DisableCharge();
        }

        private void MnuItemDisableUnChecked(object sender, RoutedEventArgs e)
        {
            EnableCharge();
        }
    }
}