using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using ElectricField.Classes;

namespace ElectricField
{
    /// <summary>
    /// Interaction logic for FieldMeterWindow.xaml
    /// </summary>
    public partial class FieldMeterWindow : Window
    {
        private readonly bool _lateUpdate;

        public FieldMeterWindow()
        {
            InitializeComponent();
            _lateUpdate = true;
        }

        private void FieldMeterWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            if (_lateUpdate)
            {
                if (doubleUpDownXPos.Value != null && doubleUpDownYPos.Value != null)
                {
                    try
                    {
                        Vector forceVec = MainWindow.Instance.GetForceAtDesirePosition(doubleUpDownXPos.Value.Value,
                                                                                       doubleUpDownYPos.Value.Value,
                                                                                       0);

                        txtForceX.Text = forceVec.X.ToString(CultureInfo.InvariantCulture);
                        txtForceY.Text = forceVec.Y.ToString(CultureInfo.InvariantCulture);
                        txtForceMag.Text = Helper.VectorMagnitude(forceVec).ToString(CultureInfo.InvariantCulture);
                    }
                    catch (Exception exp)
                    {
                        Debug.WriteLine(exp.Message);
                    }
                }
            }
        }

        private void DoubleUpDownPosesValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Update();
        }
    }
}