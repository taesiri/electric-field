using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ElectricField.Classes;
using ElectricField.Controls;
using ElectricField.SolverClasses;
using MahApps.Metro;

namespace ElectricField.Graph
{
    /// <summary>
    /// Interaction logic for ChargeChargeGraphDialog.xaml
    /// </summary>
    public partial class ChargeChargeGraphDialog
    {
        private const int ImageHieght = 628;
        private const int ImageWidth = 776;
        private readonly List<Charge> _charges;
        private readonly IEnumerable<UIElement> _data;
        private readonly List<Surface> _surfaces;


        public ChargeChargeGraphDialog(List<Charge> charges, List<Surface> surfaces)
        {
            InitializeComponent();
            _data = MainWindow.Instance.GetListOfItems();
            _charges = charges;
            _surfaces = surfaces;
        }

        private void BtnDrawClick(object sender, RoutedEventArgs e)
        {
            if (cmbFloat.SelectedIndex == -1 || cmbStatic.SelectedIndex == -1)
            {
                MessageBox.Show("Fill all fields!!");
                return;
            }

            int duration = 10;
            var startloc = new Point((double) integerUDStartX.Value, (double) integerUDStartY.Value);

            if (integerUDDuration.Value != null)
            {
                duration = (int) integerUDDuration.Value;
            }

            var slver = new Solver(_charges, _surfaces, ImageHieght, ImageWidth);
            slver.SolveIt();


            FreeCharge freeCharge = null;
            PositiveCharge positiveCharge = null;
            NegativeCharge negativeCharge = null;

            IEnumerable<UIElement> allitems = MainWindow.Instance.GetListOfItems();
            foreach (UIElement chargeitem in allitems)
            {
                if (chargeitem.GetType() == typeof (FreeCharge))
                {
                    string name = ((FreeCharge) chargeitem).MyCharge.Name;
                    if ((string) cmbFloat.SelectedValue == name)
                    {
                        freeCharge = ((FreeCharge) chargeitem);
                    }
                }
                else if (chargeitem.GetType() == typeof (PositiveCharge))
                {
                    string name = ((PositiveCharge) chargeitem).MyCharge.Name;
                    if ((string) cmbStatic.SelectedValue == name)
                    {
                        positiveCharge = ((PositiveCharge) chargeitem);
                    }
                }
                else if (chargeitem.GetType() == typeof (NegativeCharge))
                {
                    string name = ((NegativeCharge) chargeitem).MyCharge.Name;
                    if ((string) cmbStatic.SelectedValue == name)
                    {
                        negativeCharge = ((NegativeCharge) chargeitem);
                    }
                }
            }


            if (freeCharge != null)
            {
                var Orgins = new Point();
                if (positiveCharge != null)
                {
                    Orgins = new Point(positiveCharge.RenderTransform.Value.OffsetX,
                                       positiveCharge.RenderTransform.Value.OffsetY);
                }
                else if (negativeCharge != null)
                {
                    Orgins = new Point(negativeCharge.RenderTransform.Value.OffsetX,
                                       negativeCharge.RenderTransform.Value.OffsetY);
                }

                var returnedData = (List<Point>) slver.ChargeChargeDistance(freeCharge, Orgins, duration, startloc);

                var graphOutput = new GraphWindow("Distance", "Time");
                graphOutput.DrawThis(returnedData, Brushes.DarkRed, Brushes.DarkMagenta, "Distance");

                if (returnedData != null)
                {
                    IEnumerable<Point> velcocity = Helper.CalculateVelocity(returnedData);
                    graphOutput.DrawThis(velcocity, Brushes.DodgerBlue, Brushes.LightSkyBlue, "Velocity");
                }


                graphOutput.Show();
            }


            Close();
        }

        private void MetroWindowLoaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Light);

            if (_data == null)
            {
                MessageBox.Show("You haven't Anything in the field!");
                Close();
            }
            Debug.Assert(_data != null, "data != null");
            foreach (UIElement uiElement in _data)
            {
                if (uiElement.GetType() == typeof (FreeCharge))
                {
                    string name = ((FreeCharge) uiElement).MyCharge.Name;
                    cmbFloat.Items.Add(name);
                }
                else if (uiElement.GetType() == typeof (PositiveCharge))
                {
                    string name = ((PositiveCharge) uiElement).MyCharge.Name;
                    cmbStatic.Items.Add(name);
                }
                else if (uiElement.GetType() == typeof (NegativeCharge))
                {
                    string name = ((NegativeCharge) uiElement).MyCharge.Name;
                    cmbStatic.Items.Add(name);
                }
            }

            integerUDDuration.Value = 10;
        }

        private void CmbFloatSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (UIElement chargeitem in _data)
            {
                if (chargeitem.GetType() == typeof (FreeCharge))
                {
                    string name = ((FreeCharge) chargeitem).MyCharge.Name;
                    if (name == cmbFloat.SelectedItem.ToString())
                    {
                        integerUDStartX.Value = (int) chargeitem.RenderTransform.Value.OffsetX;
                        integerUDStartY.Value = (int) chargeitem.RenderTransform.Value.OffsetY;
                    }
                }
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}