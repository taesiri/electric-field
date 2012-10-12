using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ElectricField.Classes;

namespace ElectricField
{
    /// <summary>
    /// Interaction logic for FieldMeterWindow.xaml
    /// </summary>
    public partial class FieldMeterWindow : Window
    {
        private bool _lateUpdate = false;

        public FieldMeterWindow()
        {
            InitializeComponent();
            _lateUpdate = true;
        }

        private void FieldMeterWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            if (_lateUpdate)
            {
                if (doubleUpDownXPos.Value != null && doubleUpDownYPos.Value != null)
                {
                    try
                    {
                        var forceVec = MainWindow.Instance.GetForceAtDesirePosition(doubleUpDownXPos.Value.Value,
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