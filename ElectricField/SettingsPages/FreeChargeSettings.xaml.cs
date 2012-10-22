using ElectricField.Controls;

namespace ElectricField.SettingsPages
{
    /// <summary>
    /// Interaction logic for FreeChargeSettings.xaml
    /// </summary>
    public partial class FreeChargeSettings 
    {
        public FreeChargeSettings(FreeCharge charge)
        {
            InitializeComponent();

            lblLocation.Content = "Location : (" + charge.RenderTransform.Value.OffsetX + "," +
                                  charge.RenderTransform.Value.OffsetY + ")";
        }
    }
}