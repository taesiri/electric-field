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
using ElectricField.SettingsPages;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for PositiveCharge.xaml
    /// </summary>
    public partial class PositiveCharge
    {
        public Charge MyCharge;

        private Point _previousLocation;
        private Transform _previousTransform;

        public PositiveCharge()
        {
            InitializeComponent();
            MyCharge = new Charge();
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

        public void ShowSettings()
        {
            var setting = new Settings(MyCharge);
            Nullable<bool> result = setting.ShowDialog();
            if (result == true)
            {
                this.MyCharge.ElectricCharge = Convert.ToInt32(setting.integerUDCharge.Value);
                MainWindow.Instance.Calculation();
            }
        }

        private void DisableCharge()
        {
            this.MyCharge.IsActive = false;
            txtChargeLable.Text = ".";
            MainWindow.Instance.Calculation();
        }

        private void EnableCharge()
        {
            this.MyCharge.IsActive = true;
            txtChargeLable.Text = "+";
            MainWindow.Instance.Calculation();
        }

        private void EliminateCharge()
        {
            MainWindow.Instance.RemoveExistingCharge(this.MyCharge, this);
       
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
