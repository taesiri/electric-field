using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ElectricField.Classes;
using ElectricField.Controls;

namespace ElectricField.SolverClasses
{
    /// <summary>
    /// Interaction logic for SingleChargeSolver.xaml
    /// </summary>
    public partial class SingleChargeSolver : Window
    {
        public static SingleChargeSolver Instance;
        private readonly List<Charge> _charges = new List<Charge>();

        public SingleChargeSolver()
        {
            InitializeComponent();
            Instance = this;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var newfPoint = new FieldVector
                                        {
                                            Height = 14,
                                            Width = 8,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            Margin = new Thickness((45*i) + 21, (45*j) + 12, 0, 0)
                                        };
                    gridField.Children.Add(newfPoint);
                }
            }

            var group = new TransformGroup();

            var transfer = new TranslateTransform
                               {
                                   X = 190,
                                   Y = 118
                               };

            group.Children.Add(transfer);
            positiveCharge1.RenderTransform = group;

            _charges.Add(positiveCharge1.MyCharge);
            CalculatePositions();
        }

        public void Calculation()
        {
            if (_charges == null)
            {
            }
            else if (_charges.Count == 0)
            {
                foreach (object element in gridField.Children)
                    if (element.GetType() == typeof (FieldVector))
                        ((FieldVector) element).ForceVector = new Vector(0, 0);
            }
            else if (_charges.Count != 0)
            {
                foreach (object element in gridField.Children)
                {
                    if (element.GetType() == typeof (FieldVector))
                    {
                        // Calculate for Each Point Separately
                        var forceVec = new Vector(0, 0);
                        foreach (Charge charge in _charges)
                        {
                            if (charge.IsActive)
                            {
                                double distanceX = ((FieldVector) element).Margin.Left - charge.Location.X;
                                double distanceY = charge.Location.Y - ((FieldVector) element).Margin.Top;

                                // Calculating Height and Width of Displayed charge Object itself!
                                distanceX -= 12;
                                distanceY += 12;

                                var tempVec = new Vector(distanceX, distanceY);
                                double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                                //Consideration!
                                if (charge.Type == Charge.ChargeType.Negative)
                                {
                                    tempVec = Helper.InverseVector(tempVec);
                                }

                                tempVec = Helper.NormalizeVector(tempVec);
                                tempVec = Helper.AmountOForce(charge.ElectricCharge, amountOfDistance)*tempVec;


                                forceVec += tempVec;
                            }
                        }

                        ((FieldVector) element).ForceVector = forceVec;
                    }
                }
            }
        }

        public void CalculatePositions()
        {
            foreach (object element in gridField.Children)
            {
                if (element.GetType() == typeof (PositiveCharge))
                {
                    ((PositiveCharge) element).MyCharge.Location =
                        new Point(((PositiveCharge) element).RenderTransform.Value.OffsetX,
                                  ((PositiveCharge) element).RenderTransform.Value.OffsetY);
                }
                if (element.GetType() == typeof (NegativeCharge))
                {
                    ((NegativeCharge) element).MyCharge.Location =
                        new Point(((NegativeCharge) element).RenderTransform.Value.OffsetX,
                                  ((NegativeCharge) element).RenderTransform.Value.OffsetY);
                }
            }
            Calculation();
        }

        private void IntegerUdChargeValueValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (integerUDChargeValue.Value != null)
            {
                int? chargevalue = integerUDChargeValue.Value;
                positiveCharge1.MyCharge.ElectricCharge = (int) chargevalue;
            }
            Calculation();
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}