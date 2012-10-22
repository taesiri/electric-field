using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ElectricField.Classes;
using ElectricField.Controls;
using ElectricField.Graph;
using ElectricField.SolverClasses;
using Microsoft.Win32;

namespace ElectricField
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;
        private readonly List<Charge> _charges = new List<Charge>();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        public FieldMeterWindow FieldMeter = new FieldMeterWindow();
        public ElementOutline FieldOutline = new ElementOutline();
        private bool _isTimerStarted;
        private int _time;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    var newfPoint = new FieldVector
                                        {
                                            Height = 16,
                                            Width = 12,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            Margin = new Thickness((45*i) + 20, (45*j) + 12, 0, 0)
                                        };
                    gridField.Children.Add(newfPoint);
                }
            }
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Tick += TimerTick;

            CalculatePositions();
            //var x = Window.GetWindow(this).
        }

        public Vector GetForceAtDesirePosition(double xPos, double yPos, int fault)
        {
            if (_charges == null && TotalSurfaces() == 0)
            {
                return new Vector(0, 0);
            }
            if (_charges != null && (_charges.Count == 0 && TotalSurfaces() == 0))
            {
                return new Vector(0, 0);
            }

            var totalForceVec = new Vector(0, 0);

            //for Charges
            if (_charges != null)
                foreach (Charge charge in _charges)
                {
                    if (charge.IsActive)
                    {
                        double distanceX = xPos - charge.Location.X;
                        double distanceY = charge.Location.Y - yPos;

                        // Calculating Height and Width of Displayed charge Object itself!
                        distanceX -= fault;
                        distanceY += fault;

                        var tempVec = new Vector(distanceX, distanceY);
                        double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                        //Consideration!
                        if (charge.Type == Charge.ChargeType.Negative)
                        {
                            tempVec = Helper.InverseVector(tempVec);
                        }

                        tempVec = Helper.NormalizeVector(tempVec);
                        tempVec = AmountOForce(charge.ElectricCharge, amountOfDistance)*tempVec;

                        totalForceVec += tempVec;
                    }
                }

            //for Surfaces
            foreach (object uiElement in gridField.Children)
            {
                if (uiElement.GetType() == typeof (Surface))
                {
                    if (((Surface) uiElement).MyCharge.IsActive)
                    {
                        double amountOfChargePerPoint = ((Surface) uiElement).MyCharge.ElectricCharge/10;
                        var surfaceLocation = new Point(((Surface) uiElement).RenderTransform.Value.OffsetX,
                                                        ((Surface) uiElement).RenderTransform.Value.OffsetY);
                        double parresh = ((Surface) uiElement).Width/10;
                        double taVasat = ((Surface) uiElement).Height/2;

                        for (int i = 0; i < 10; i++)
                        {
                            var here = new Point(surfaceLocation.X + (i*parresh), surfaceLocation.Y + taVasat);

                            double distanceX = xPos - here.X;
                            double distanceY = here.Y - yPos;

                            var tempVec = new Vector(distanceX, distanceY);
                            double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                            if (((Surface) uiElement).MyCharge.Type == Charge.ChargeType.Negative)
                            {
                                tempVec = Helper.InverseVector(tempVec);
                            }
                            tempVec = Helper.NormalizeVector(tempVec);
                            tempVec = AmountOForce(amountOfChargePerPoint, amountOfDistance)*tempVec;

                            totalForceVec += tempVec;
                        }
                    }
                }
            }


            return totalForceVec;
        }

        public void Calculation()
        {
            if (_charges == null && TotalSurfaces() == 0)
            {
            }
            else if (_charges != null && (_charges.Count == 0 && TotalSurfaces() == 0))
            {
                foreach (object element in gridField.Children)
                    if (element.GetType() == typeof (FieldVector))
                        ((FieldVector) element).ForceVector = new Vector(0, 0);
            }
            else if (_charges != null && (_charges.Count != 0 || TotalSurfaces() != 0))
            {
                foreach (object element in gridField.Children)
                {
                    if (element.GetType() == typeof (FieldVector))
                    {
                        // Calculate for Each Point Separately
                        var totalForceVec = new Vector(0, 0);

                        //Charges
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
                                tempVec = AmountOForce(charge.ElectricCharge, amountOfDistance)*tempVec;


                                totalForceVec += tempVec;
                            }
                        }
                        //Surfaces
                        foreach (object uiElement in gridField.Children)
                        {
                            if (uiElement.GetType() == typeof (Surface))
                            {
                                if (((Surface) uiElement).MyCharge.IsActive)
                                {
                                    double amountOfChargePerPoint = ((Surface) uiElement).MyCharge.ElectricCharge/10;
                                    var surfaceLocation = new Point(((Surface) uiElement).RenderTransform.Value.OffsetX,
                                                                    ((Surface) uiElement).RenderTransform.Value.OffsetY);
                                    double parresh = ((Surface) uiElement).Width/10;
                                    double taVasat = ((Surface) uiElement).Height/2;

                                    for (int i = 0; i < 10; i++)
                                    {
                                        var here = new Point(surfaceLocation.X + (i*parresh),
                                                             surfaceLocation.Y + taVasat);

                                        double distanceX = ((FieldVector) element).Margin.Left - here.X;
                                        double distanceY = here.Y - ((FieldVector) element).Margin.Top;

                                        var tempVec = new Vector(distanceX, distanceY);
                                        double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                                        if (((Surface) uiElement).MyCharge.Type == Charge.ChargeType.Negative)
                                        {
                                            tempVec = Helper.InverseVector(tempVec);
                                        }
                                        tempVec = Helper.NormalizeVector(tempVec);
                                        tempVec = AmountOForce(amountOfChargePerPoint, amountOfDistance)*tempVec;

                                        totalForceVec += tempVec;
                                    }
                                }
                            }
                        }

                        ((FieldVector) element).ForceVector = totalForceVec;
                    }
                }
                FieldMeter.Update();
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
                else if (element.GetType() == typeof (NegativeCharge))
                {
                    ((NegativeCharge) element).MyCharge.Location =
                        new Point(((NegativeCharge) element).RenderTransform.Value.OffsetX,
                                  ((NegativeCharge) element).RenderTransform.Value.OffsetY);
                }
                else if (element.GetType() == typeof (Surface))
                {
                }
            }
            Calculation();
        }

        public double AmountOForce(int electricCharge, double distance)
        {
            return ((9*electricCharge)/(distance*distance));
        }

        public double AmountOForce(double electricCharge, double distance)
        {
            return ((9*electricCharge)/(distance*distance));
        }

        public int TotalPositives()
        {
            int value = 0;
            foreach (object element in gridField.Children)
            {
                if (element.GetType() == typeof (PositiveCharge))
                {
                    value++;
                }
            }
            return value;
        }

        public int TotalNegativs()
        {
            int value = 0;
            foreach (object element in gridField.Children)
            {
                if (element.GetType() == typeof (NegativeCharge))
                {
                    value++;
                }
            }
            return value;
        }

        public int TotalFrees()
        {
            int value = 0;
            foreach (object element in gridField.Children)
            {
                if (element.GetType() == typeof (FreeCharge))
                {
                    value++;
                }
            }
            return value;
        }

        public int TotalSurfaces()
        {
            int value = 0;
            foreach (object element in gridField.Children)
            {
                if (element.GetType() == typeof (Surface))
                {
                    value++;
                }
            }
            return value;
        }


        public void AddNewCharge(Charge charge)
        {
            if (charge.Type == Charge.ChargeType.Positive)
            {
                var positiveCharge = new PositiveCharge();
                positiveCharge.Margin = new Thickness(0, 0, 0, 0);
                positiveCharge.VerticalAlignment = VerticalAlignment.Top;
                positiveCharge.HorizontalAlignment = HorizontalAlignment.Left;
                positiveCharge.MyCharge = charge;

                gridField.Children.Add(positiveCharge);

                var group = new TransformGroup();

                var transfer = new TranslateTransform
                                   {
                                       X = Helper.RandomNumber(10, (int) gridField.Width - 45),
                                       Y = Helper.Clamp(Helper.RandomNumber(10, ((int) gridField.Height - 35)/2), 0,
                                                        (int) gridField.Height - 65)
                                   };
                group.Children.Add(transfer);
                positiveCharge.RenderTransform = group;

                _charges.Add(positiveCharge.MyCharge);
            }
            else
            {
                var negativeCharge = new NegativeCharge();
                negativeCharge.Margin = new Thickness(0, 0, 0, 0);
                negativeCharge.VerticalAlignment = VerticalAlignment.Top;
                negativeCharge.HorizontalAlignment = HorizontalAlignment.Left;
                negativeCharge.MyCharge = charge;

                gridField.Children.Add(negativeCharge);

                var group = new TransformGroup();

                var transfer = new TranslateTransform
                                   {
                                       X = Helper.RandomNumber(10, (int) gridField.Width - 45),
                                       Y = Helper.RandomNumber(10, ((int) gridField.Height - 35))
                                   };
                group.Children.Add(transfer);
                negativeCharge.RenderTransform = group;

                _charges.Add(negativeCharge.MyCharge);
            }
            CalculatePositions();
            Calculation();
            FieldOutline.Update();
        }

        public void RemoveExistingCharge(Charge charge, UIElement element)
        {
            _charges.Remove(charge);
            gridField.Children.Remove(element);
            Calculation();
            FieldOutline.Update();
        }

        public void RemoveExistingCharge(ChargeDensity charge, UIElement element)
        {
            gridField.Children.Remove(element);
            Calculation();
            FieldOutline.Update();
        }

        private void GenerateBitmapRgb24Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {Title = "Choose location ...", Filter = "JPEG Image File (*.jpg)|*.jpg"};

            List<Surface> surfacelist =
                gridField.Children.Cast<object>().Where(uiElement => uiElement.GetType() == typeof (Surface)).Cast
                    <Surface>().ToList();

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var slvr = new Solver(_charges, surfacelist, (int) gridField.Height, (int) gridField.Width);

                slvr.SolveIt();
                //slvr.DrawRgb24Image(dialog.FileName);
                slvr.DrawRgb24Image(dialog.FileName, Color.FromRgb(255, 255, 255));
                if (
                    MessageBox.Show(
                        "Image file has been generated and saved Successfully!" + Environment.NewLine +
                        "Do you want to open it now ?", "Write file Completed", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    Process.Start(dialog.FileName);
                }
            }
        }

        private void GenerateGray8BitmapClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {Title = "Choose location ...", Filter = "JPEG Image File (*.jpg)|*.jpg"};

            List<Surface> surfacelist =
                gridField.Children.Cast<object>().Where(uiElement => uiElement.GetType() == typeof (Surface)).Cast
                    <Surface>().ToList();

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var slvr = new Solver(_charges, surfacelist, (int) gridField.Height, (int) gridField.Width);

                slvr.SolveIt();
                slvr.DrawGray8Image(dialog.FileName);

                if (
                    MessageBox.Show(
                        "Image file has been generated and saved Successfully!" + Environment.NewLine +
                        "Do you want to open it now ?", "Write file Completed", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    Process.Start(dialog.FileName);
                }
            }
        }

        private void MenuItemExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool CheckExistingChargeWithName(string name)
        {
            foreach (object element in gridField.Children)
            {
                if (element.GetType() == typeof (PositiveCharge))
                {
                    if (((PositiveCharge) element).MyCharge.Name == name)
                    {
                        return true;
                    }
                }
                else if (element.GetType() == typeof (NegativeCharge))
                {
                    if (((NegativeCharge) element).MyCharge.Name == name)
                    {
                        return true;
                    }
                }
                else if (element.GetType() == typeof (FreeCharge))
                {
                    if (((FreeCharge) element).MyCharge.Name == name)
                    {
                        return true;
                    }
                }
                else if (element.GetType() == typeof (Surface))
                {
                    if (((Surface) element).MyCharge.Name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddPositive(object sender, RoutedEventArgs e)
        {
            if (_charges.Count > 50)
            {
                MessageBox.Show("What are you planning to do ?", "Error!");
                return;
            }

            string chargename = "positive-charge " + (TotalPositives() + 1).ToString(CultureInfo.InvariantCulture);
            int counter = 1;
            while (CheckExistingChargeWithName(chargename))
            {
                counter++;
                chargename = "positive-charge " + (TotalPositives() + counter).ToString(CultureInfo.InvariantCulture);
            }
            if (_charges.Count > 15)
            {
                if (MessageBox.Show("Too Many Charges Already! Are you Sure ?", "Warning", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                    AddNewCharge(new Charge(chargename, Charge.ChargeType.Positive));
            }
            else
            {
                AddNewCharge(new Charge(chargename, Charge.ChargeType.Positive));
            }
        }

        private void AddNegative(object sender, RoutedEventArgs e)
        {
            if (_charges.Count > 50)
            {
                MessageBox.Show("What are you planning to do ?", "Error!");
                return;
            }
            string chargename = "negative-charge " + (TotalNegativs() + 1).ToString(CultureInfo.InvariantCulture);
            int counter = 1;
            while (CheckExistingChargeWithName(chargename))
            {
                counter++;
                chargename = "negative-charge " + (TotalPositives() + counter).ToString(CultureInfo.InvariantCulture);
            }

            if (_charges.Count > 15)
            {
                if (MessageBox.Show("Too Many Charges Already! Are you Sure ?", "Warning", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                    AddNewCharge(new Charge(chargename, Charge.ChargeType.Negative));
            }
            else
            {
                AddNewCharge(new Charge(chargename, Charge.ChargeType.Negative));
            }
        }

        private void AddNewFreeChargeClick(object sender, RoutedEventArgs e)
        {
            var freecharge = new FreeCharge();
            freecharge.Margin = new Thickness(0, 0, 0, 0);
            freecharge.VerticalAlignment = VerticalAlignment.Top;
            freecharge.HorizontalAlignment = HorizontalAlignment.Left;
            freecharge.MyCharge = new Charge();
            freecharge.Height = 12;
            freecharge.Width = 12;

            string chargename = "Free-charge " + (TotalFrees() + 1).ToString(CultureInfo.InvariantCulture);
            int counter = 1;
            while (CheckExistingChargeWithName(chargename))
            {
                counter++;
                chargename = "Free-charge " + (TotalFrees() + counter).ToString(CultureInfo.InvariantCulture);
            }
            freecharge.MyCharge.Name = chargename;

            var group = new TransformGroup();

            var transfer = new TranslateTransform
                               {
                                   X = Helper.RandomNumber(10, (int) gridField.Width - 45),
                                   Y = Helper.Clamp(Helper.RandomNumber(10, ((int) gridField.Height - 35)/2), 0,
                                                    (int) gridField.Height - 65)
                               };
            gridField.Children.Add(freecharge);
            group.Children.Add(transfer);
            freecharge.StartPoint = new Point(transfer.X, transfer.Y);
            freecharge.RenderTransform = group;

            FieldOutline.Update();
        }

        private void BtnAddSurfaceClick(object sender, RoutedEventArgs e)
        {
            var surface = new Surface(new ChargeDensity());
            surface.Margin = new Thickness(0, 0, 0, 0);
            surface.VerticalAlignment = VerticalAlignment.Top;
            surface.HorizontalAlignment = HorizontalAlignment.Left;
            surface.MyCharge = new ChargeDensity();
            surface.Height = 32;
            surface.Width = 200;

            string chargename = "Surface" + (TotalSurfaces() + 1).ToString(CultureInfo.InvariantCulture);
            int counter = 1;
            while (CheckExistingChargeWithName(chargename))
            {
                counter++;
                chargename = "Surface " + (TotalSurfaces() + counter).ToString(CultureInfo.InvariantCulture);
            }
            surface.MyCharge.Name = chargename;

            var group = new TransformGroup();

            var transfer = new TranslateTransform
                               {
                                   X = Helper.RandomNumber(10, (int) gridField.Width - 45),
                                   Y = Helper.Clamp(Helper.RandomNumber(10, ((int) gridField.Height - 35)/2), 0,
                                                    (int) gridField.Height - 65)
                               };
            gridField.Children.Add(surface);
            group.Children.Add(transfer);
            surface.RenderTransform = group;

            CalculatePositions();
            FieldOutline.Update();
        }

        public IEnumerable<UIElement> GetListOfItems()
        {
            return
                gridField.Children.Cast<object>().Where(
                    element =>
                    element.GetType() == typeof (PositiveCharge) || element.GetType() == typeof (NegativeCharge) ||
                    element.GetType() == typeof (Surface) ||
                    element.GetType() == typeof (FreeCharge)).Cast
                    <UIElement>().ToList();
        }

        private void MnuItemRestartClick(object sender, RoutedEventArgs e)
        {
            //_timer

            var elems = new List<UIElement>();
            foreach (object element in gridField.Children)
                if (element.GetType() == typeof (PositiveCharge) || element.GetType() == typeof (NegativeCharge) ||
                    element.GetType() == typeof (DummyPoint) || element.GetType() == typeof (FreeCharge) ||
                    element.GetType() == typeof (Surface))
                    elems.Add((UIElement) element);

            foreach (UIElement uiElement in elems)
            {
                gridField.Children.Remove(uiElement);
            }
            _charges.Clear();

            _timer.Stop();

            gridField.Background = Brushes.Black;
            CalculatePositions();
            Calculation();

            _isTimerStarted = false;
            MnuDeleteDummy.IsEnabled = false;
        }


        private void MnuDummyDeleterClick(object sender, RoutedEventArgs e)
        {
            var elems = new List<UIElement>();
            foreach (object element in gridField.Children)
                if (element.GetType() == typeof (DummyPoint) || element.GetType() == typeof (FreeCharge))
                    elems.Add((UIElement) element);

            foreach (UIElement uiElement in elems)
            {
                gridField.Children.Remove(uiElement);
            }

            _timer.Stop();
            _isTimerStarted = false;
            MnuDeleteDummy.IsEnabled = false;
        }

        private void MunItemSolverClick(object sender, RoutedEventArgs e)
        {
            //if (_charges.Count == 0 )
            //{
            //    MessageBox.Show("The field is Empty! Please add some Charges!", "Error!");
            //    return;
            //}

            List<Surface> surfacelist = gridField.Children.Cast<object>().Where(
                uiElement => uiElement.GetType() == typeof (Surface)).Cast
                <Surface>().ToList();

            //var csolver = new ChargeSolver(0, 100, (int) gridField.Height, (int) gridField.Width, _charges, surfacelist);
            //csolver.ShowDialog();

            var csolver = new CustomSolverDialog(0, 100, (int) gridField.Height, (int) gridField.Width, _charges,
                                                 surfacelist);
            csolver.ShowDialog();
        }

        private void MunItemSingleSolverClick(object sender, RoutedEventArgs e)
        {
            var scs = new SingleChargeSolver();
            scs.Show();
        }

        private void BtnQuickSolveClick(object sender, RoutedEventArgs e)
        {
            List<Surface> surfacelist =
                gridField.Children.Cast<object>().Where(uiElement => uiElement.GetType() == typeof (Surface)).Cast
                    <Surface>().ToList();
            var slvr = new Solver(_charges, surfacelist, (int) gridField.Height, (int) gridField.Width);

            slvr.SolveIt();
            var imagebrush = new ImageBrush();
            //imagebrush.ImageSource = slvr.GetImageRgb24();

            var ListofColors = new List<Solver.Colorizer>();

            var color00 = new Solver.Colorizer();
            var color0 = new Solver.Colorizer();
            var color1 = new Solver.Colorizer();
            var color2 = new Solver.Colorizer();
            var color3 = new Solver.Colorizer();
            var color4 = new Solver.Colorizer();
            var color5 = new Solver.Colorizer();


            color00.Color = Colors.DarkRed;
            color00.Min = 1001;
            color00.Max = int.MaxValue;

            color0.Color = Colors.DarkRed;
            color0.Min = 400;
            color0.Max = 1000;

            color1.Color = Colors.Red;
            color1.Min = 100;
            color1.Max = 399;

            color2.Color = Colors.OrangeRed;
            color2.Min = 60;
            color2.Max = 99;

            color3.Color = Colors.Yellow;
            color3.Min = 30;
            color3.Max = 59;

            color4.Color = Colors.YellowGreen;
            color4.Min = 10;
            color4.Max = 29;

            color5.Color = Colors.Black;
            color5.Min = 0;
            color5.Max = 9;


            ListofColors.Add(color00);
            ListofColors.Add(color0);
            ListofColors.Add(color1);
            ListofColors.Add(color2);
            ListofColors.Add(color3);
            ListofColors.Add(color4);
            ListofColors.Add(color5);

            ListofColors.Add(new Solver.Colorizer());
            imagebrush.ImageSource = slvr.CustomeColorizerSolver(ListofColors);
            gridField.Background = imagebrush;
        }

        private void BtnDrawFieldLinesClick(object sender, RoutedEventArgs e)
        {
            List<Surface> surfacelist =
                gridField.Children.Cast<object>().Where(uiElement => uiElement.GetType() == typeof (Surface)).Cast
                    <Surface>().ToList();
            var slvr = new Solver(_charges, surfacelist, (int) gridField.Height, (int) gridField.Width);

            slvr.SolveIt();
            var imagebrush = new ImageBrush();
            imagebrush.ImageSource = slvr.GenerateFieldLines(false);
            gridField.Background = imagebrush;
        }

        public int NearestDistancetoNegative(FreeCharge freeCharge)
        {
            int nearestDistance = 100000;
            var location1 = new Point((int) freeCharge.RenderTransform.Value.OffsetX,
                                      (int) freeCharge.RenderTransform.Value.OffsetY);

            foreach (object uiElement in gridField.Children)
            {
                if (uiElement.GetType() == typeof (NegativeCharge))
                {
                    var location2 = new Point((int) ((NegativeCharge) uiElement).RenderTransform.Value.OffsetX + 16,
                                              (int) ((NegativeCharge) uiElement).RenderTransform.Value.OffsetY + 16);

                    var distance = (int) Helper.Distance(location1, location2);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                    }
                }
            }


            return nearestDistance;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _time++;
            var deleteList = new List<UIElement>();
            foreach (object child in gridField.Children)
            {
                if (child.GetType() == typeof (FreeCharge))
                {
                    double fcX = ((FreeCharge) child).RenderTransform.Value.OffsetX;
                    double fcY = ((FreeCharge) child).RenderTransform.Value.OffsetY;
                    Vector moveVector = GetForceAtDesirePosition(fcX, fcY, 0);

                    MoveIt(moveVector, ((FreeCharge) child).LastVector, child);

                    int nDist = NearestDistancetoNegative((FreeCharge) child);
                    double mag = Helper.VectorMagnitude(moveVector);
                    if (Math.Abs(nDist - mag) < 40)
                    {
                        deleteList.Add((FreeCharge) child);
                    }
                    else if (Math.Abs((int) ((FreeCharge) child).RenderTransform.Value.OffsetX) >= 800 ||
                             Math.Abs((int) ((FreeCharge) child).RenderTransform.Value.OffsetY) >= 800)
                    {
                        deleteList.Add((FreeCharge) child);
                    }
                    ((FreeCharge) child).LastVector += moveVector;
                }
            }
            if (deleteList.Count != 0)
            {
                foreach (UIElement uiElement in deleteList)
                {
                    gridField.Children.Remove(uiElement);
                }
                FieldOutline.Update();
            }
        }

        public void MoveIt(Vector accelerationVec, Vector lastVelocityVec, object element)
        {
            const double deltaTime = 0.01;
            //F=MA
            //amounttoMove = at + v0;


            double fcX = ((FreeCharge) element).RenderTransform.Value.OffsetX;
            double fcY = ((FreeCharge) element).RenderTransform.Value.OffsetY;

            var group = new TransformGroup();

            var transfer = new TranslateTransform
                               {
                                   X = fcX + (accelerationVec.X + lastVelocityVec.X)*deltaTime,
                                   Y = fcY - (accelerationVec.Y*deltaTime + lastVelocityVec.Y)*deltaTime
                               };
            group.Children.Add(transfer);

            ((FreeCharge) element).VisitedPoints.Add(new PointStatistics(new Point(transfer.X, transfer.Y), _time, 0, 0,
                                                                         0));
            ((FreeCharge) element).RenderTransform = group;
        }


        private void BtnstartSimulationClick(object sender, RoutedEventArgs e)
        {
            if (!_isTimerStarted)
            {
                _timer.Start();
                _isTimerStarted = true;
                MnuDeleteDummy.IsEnabled = true;
            }
        }

        private void BtnStopSimulationClik(object sender, RoutedEventArgs e)
        {
            if (_isTimerStarted)
            {
                _timer.Stop();
                _isTimerStarted = false;
                MnuDeleteDummy.IsEnabled = false;
            }
        }

        private void WindowMotherLoaded(object sender, RoutedEventArgs e)
        {
            FieldOutline.Show();
            FieldOutline.Update();
            FieldMeter.Show();
            FieldMeter.Update();
        }

        private void MnuItemFieldOutlineClick(object sender, RoutedEventArgs e)
        {
            FieldOutline.Show();
        }

        private void MainAppWindowClosing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MnuChargeChargeGraphClick(object sender, RoutedEventArgs e)
        {
            List<Surface> surfacelist = gridField.Children.Cast<object>().Where(
                uiElement => uiElement.GetType() == typeof (Surface)).Cast
                <Surface>().ToList();
            var chdialog = new ChargeChargeGraphDialog(_charges, surfacelist);
            chdialog.ShowDialog();
        }

        private void MnuItemForeAtDesiredPositionClick(object sender, RoutedEventArgs e)
        {
            FieldMeter.Show();
        }

        private void BtnQuckDrawFieldLinesClick(object sender, RoutedEventArgs e)
        {
            List<Surface> surfacelist =
                gridField.Children.Cast<object>().Where(uiElement => uiElement.GetType() == typeof (Surface)).Cast
                    <Surface>().ToList();
            var slvr = new Solver(_charges, surfacelist, (int) gridField.Height, (int) gridField.Width);

            slvr.SolveIt();
            var imagebrush = new ImageBrush();
            imagebrush.ImageSource = slvr.GenerateFieldLines(true);
            gridField.Background = imagebrush;
        }
    }
}