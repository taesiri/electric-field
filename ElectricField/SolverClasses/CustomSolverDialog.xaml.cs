using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ElectricField.Classes;
using ElectricField.Controls;

namespace ElectricField.SolverClasses
{
    /// <summary>
    /// Interaction logic for CustomSolverDialog.xaml
    /// </summary>
    [Guid("4AC37010-AB5A-41EF-B5AC-5EDE85CE7758")]
    public partial class CustomSolverDialog
    {
        private readonly List<Charge> _charges;
        private readonly int _imageHieght = 100;
        private readonly int _imageWidth = 100;
        private readonly List<Surface> _surfaces;


        public CustomSolverDialog(double min, double max, int hieght, int width, List<Charge> charges,
                                  List<Surface> surfaces)
        {
            InitializeComponent();
            InitColorer();

            InitializeComponent();
            _imageHieght = hieght;
            _imageWidth = width;
            _charges = charges;
            _surfaces = surfaces;
        }

        public CustomSolverDialog()
        {
            InitializeComponent();


            InitColorer();
        }

        private void InitColorer()
        {
            var typeList = new List<Type> {typeof (Solver.ColorRange)};

            collectionEditor1.NewItemTypes = typeList;


            var color00 = new Solver.ColorRange(1001, uint.MaxValue, Colors.DarkRed);
            var color0 = new Solver.ColorRange(400, 1000, Colors.DarkRed);
            var color1 = new Solver.ColorRange(100, 399, Colors.Red);
            var color2 = new Solver.ColorRange(60, 99, Colors.OrangeRed);
            var color3 = new Solver.ColorRange(30, 59, Colors.Yellow);
            var color4 = new Solver.ColorRange(10, 29, Colors.YellowGreen);
            var color5 = new Solver.ColorRange(0, 9, Colors.Black);


            collectionEditor1.Items.Add(color00);
            collectionEditor1.Items.Add(color0);
            collectionEditor1.Items.Add(color1);
            collectionEditor1.Items.Add(color2);
            collectionEditor1.Items.Add(color3);
            collectionEditor1.Items.Add(color4);
            collectionEditor1.Items.Add(color5);
        }

        private List<Solver.Colorizer> GenerateColorPalette()
        {
            return
                collectionEditor1.Items.Select(cItem => Helper.ConvertToColorizer((Solver.ColorRange) cItem)).ToList();
        }

        private void BtnSolveClick(object sender, RoutedEventArgs e)
        {
            List<Solver.Colorizer> colors = GenerateColorPalette();
            var slvr = new Solver(_charges, _surfaces, _imageHieght, _imageWidth);

            slvr.SolveIt();

            SolverOutput solvedutput;
            if (chkBoxDrawLines.IsChecked != null && chkBoxDrawLines.IsChecked == true)
            {
                BitmapSource outputData = slvr.CustomeColorizerSolverWithLines(colors,
                                                                               colorPickerLineColors.SelectedColor);

                solvedutput = new SolverOutput(outputData, colors, _charges);
            }
            else
            {
                BitmapSource outputData = slvr.CustomeColorizerSolver(colors);

                solvedutput = new SolverOutput(outputData, colors, _charges);
            }

            solvedutput.lblMarker.Visibility = Visibility.Hidden;
            solvedutput.recMarker.Visibility = Visibility.Hidden;
            solvedutput.gridMarker.Visibility = Visibility.Hidden;


            solvedutput.Show();
            Close();
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}