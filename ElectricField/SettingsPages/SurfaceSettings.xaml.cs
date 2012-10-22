using System.Windows;
using System.Windows.Controls;
using ElectricField.Classes;

namespace ElectricField.SettingsPages
{
    /// <summary>
    /// Interaction logic for SurfaceSettings.xaml
    /// </summary>
    public partial class SurfaceSettings
    {
        public readonly ChargeDensity ChrDensity;

        public SurfaceSettings(ChargeDensity chargeDensity)
        {
            InitializeComponent();
            ChrDensity = new ChargeDensity(chargeDensity);
            cmboxChargeType.SelectedIndex = chargeDensity.Type == Charge.ChargeType.Positive ? 0 : 1;

            doubleUDCharge.Value = chargeDensity.ElectricCharge;
            doubleUDDensity.Value = chargeDensity.Density;

            intUDHeight.Value = chargeDensity.BodySurface.Height;
            intUDWidth.Value = chargeDensity.BodySurface.Width;
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DoubleUdDensityValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (doubleUDDensity.Value != null)
            {
                ChrDensity.Density = (double) doubleUDDensity.Value;
                doubleUDCharge.Value = ChrDensity.ElectricCharge;
            }
        }

        private void DoubleUdChargeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (doubleUDCharge.Value != null)
            {
                ChrDensity.ElectricCharge = (double) doubleUDCharge.Value;
                doubleUDDensity.Value = ChrDensity.Density;
            }
        }

        private void CmboxChargeTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmboxChargeType.SelectedIndex == 0)
            {
                ChrDensity.Type = Charge.ChargeType.Positive;
            }
            else
            {
                ChrDensity.Type = Charge.ChargeType.Negative;
            }
        }
    }
}