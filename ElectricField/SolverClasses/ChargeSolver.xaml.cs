using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ElectricField.Classes;
using ElectricField.Controls;

namespace ElectricField.SolverClasses
{
    /// <summary>
    /// Interaction logic for ChargeSolver.xaml
    /// </summary>
    public partial class ChargeSolver
    {
        private readonly bool _isInitialized = false;
        private readonly int _imageHieght = 100;
        private readonly int _imageWidth = 100;
        private readonly List<Charge> _charges;
        private readonly List<Surface> _surfaces;

        public ChargeSolver()
        {
            InitializeComponent();
            lblMax.Content = 100;
            lblMin.Content = 0;
            _isInitialized = true;
        }

        public ChargeSolver(double min, double max, int hieght, int width, List<Charge> charges, List<Surface> surfaces)
        {
            InitializeComponent();
            lblMax.Content = min;
            lblMin.Content = max;
            _isInitialized = true;
            _imageHieght = hieght;
            _imageWidth = width;
            _charges = charges;
            _surfaces = surfaces;
            integerUDThreshold.Value = 75;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            colorPickerMaxColor.SelectedColor = Colors.Yellow;
            integerUDClamp.Value = 50;
        }

        private void ColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

            if (_isInitialized)
            {
                var myBrush = new LinearGradientBrush();
                myBrush.GradientStops.Add(new GradientStop(colorPickerMinColor.SelectedColor, 0.0));
                myBrush.GradientStops.Add(new GradientStop(colorPickerMaxColor.SelectedColor, 1.0));

                recColorBar.Fill = myBrush;
            }
        }

        private void BtnSolveClick(object sender, RoutedEventArgs e)
        {
            var allowMark = false;
            allowMark = chkMarker.IsChecked != null && (bool) chkMarker.IsChecked;

            int clampvalue = 100;
            if (integerUDClamp.Value.HasValue)
            {
                clampvalue = integerUDClamp.Value.Value;
            }


            var slvr = new Solver(_charges, _surfaces, _imageHieght, _imageWidth);

            slvr.SolveIt();

            if (integerUDThreshold.Value == null)
                integerUDThreshold.Value = 50;

            SolverOutput solvedutput;
            if (chkBoxDrawLines.IsChecked != null && chkBoxDrawLines.IsChecked == true)
            {
                BitmapSource outputData = slvr.GetImageRgb24WithLines(colorPickerMaxColor.SelectedColor,
                                                                      colorPickerMinColor.SelectedColor, clampvalue,
                                                                      allowMark, integerUDThreshold.Value.Value,
                                                                      colorPickerThreshold.SelectedColor);

                solvedutput = new SolverOutput(outputData, colorPickerMinColor.SelectedColor,
                                               colorPickerMaxColor.SelectedColor,
                                               _charges);
            }
            else
            {
                BitmapSource outputData = slvr.GetImageRgb24(colorPickerMaxColor.SelectedColor,
                                                             colorPickerMinColor.SelectedColor, clampvalue,
                                                             allowMark, integerUDThreshold.Value.Value,
                                                             colorPickerThreshold.SelectedColor);

                solvedutput = new SolverOutput(outputData, colorPickerMinColor.SelectedColor,
                                               colorPickerMaxColor.SelectedColor,
                                               _charges);
            }

            if (allowMark)
            {
                solvedutput.lblMarker.Visibility = Visibility.Visible;
                solvedutput.recMarker.Visibility = Visibility.Visible;
                solvedutput.gridMarker.Visibility = Visibility.Visible;
                solvedutput.recMarker.Fill = new SolidColorBrush(colorPickerThreshold.SelectedColor);
                solvedutput.lblMarker.Content = "This color is a Marker for the points which had Value : " +
                                                integerUDThreshold.Value.Value;

            }
            else
            {
                solvedutput.lblMarker.Visibility = Visibility.Hidden;
                solvedutput.recMarker.Visibility = Visibility.Hidden;
                solvedutput.gridMarker.Visibility = Visibility.Hidden;
            }

            solvedutput.Show();
            Close();

        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChkMarkerChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
                canvasMarker.IsEnabled = true;
        }


        private void ChkMarkerUnchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
                canvasMarker.IsEnabled = false;
        }
    }
}