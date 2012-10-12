using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ElectricField.Classes;
using MahApps.Metro;

namespace ElectricField.SettingsPages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings 
    {
        public Settings()
        {
            InitializeComponent();
      
        }
        public Settings(Charge charge)
        {
            InitializeComponent();
            integerUDCharge.Value = charge.ElectricCharge;
            integerUDMass.Value = charge.Mass;
            lblbCName.Content = "Settings - " + charge.Name;

            cmboxType.Items.Add("Positive");
            cmboxType.Items.Add("Negative");

            cmboxType.SelectedIndex = charge.Type == Charge.ChargeType.Positive ? 0 : 1;


        }

        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


        private void WindowMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //DragMove();
            }
        }

        private void MetroWindowLoaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Light);
        }


    }
}
