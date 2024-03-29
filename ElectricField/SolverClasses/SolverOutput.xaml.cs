﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ElectricField.Classes;
using ElectricField.Controls;
using Microsoft.Win32;

namespace ElectricField.SolverClasses
{
    /// <summary>
    /// Interaction logic for SolverOutput.xaml
    /// </summary>
    public partial class SolverOutput : Window
    {
        private readonly List<Charge> _charges;
        private readonly BitmapSource _outputFieldLines;
        private readonly BitmapSource _outputdata;

        public SolverOutput(BitmapSource data, List<Solver.Colorizer> colorpalette, IEnumerable<Charge> charges)
        {
            InitializeComponent();
            _outputdata = data;
            _charges = new List<Charge>(charges);
            gridField.Background = new ImageBrush(data);

            recMapHelper.Fill = Helper.ConvertToBrush(colorpalette);

            foreach (Charge charge in _charges)
            {
                if (charge.IsActive)
                {
                    var newfPoint = new FieldPoint
                                        {
                                            Height = 6,
                                            Width = 6,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            Margin = new Thickness(0, 0, 0, 0),
                                            Mcharge = charge
                                        };
                    var tgroup = new TransformGroup();
                    var tt = new TranslateTransform(charge.Location.X + 14, charge.Location.Y + 14);
                    tgroup.Children.Add(tt);

                    gridField.Children.Add(newfPoint);
                    newfPoint.RenderTransform = tgroup;
                }
            }
        }

        public SolverOutput(BitmapSource data, BitmapSource fieldLines, Color minColor, Color maxcolor,
                            IEnumerable<Charge> charges)
        {
            InitializeComponent();
            _outputdata = data;
            _charges = new List<Charge>(charges);
            gridField.Background = new ImageBrush(data);

            var myBrush = new LinearGradientBrush();

            myBrush.GradientStops.Add(new GradientStop(maxcolor, 0.0));
            myBrush.GradientStops.Add(new GradientStop(minColor, 1.0));

            recMapHelper.Fill = myBrush;

            foreach (Charge charge in _charges)
            {
                if (charge.IsActive)
                {
                    var newfPoint = new FieldPoint
                                        {
                                            Height = 6,
                                            Width = 6,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            Margin = new Thickness(0, 0, 0, 0),
                                            Mcharge = charge
                                        };
                    var tgroup = new TransformGroup();
                    var tt = new TranslateTransform(charge.Location.X + 13, charge.Location.Y + 13);
                    tgroup.Children.Add(tt);

                    gridField.Children.Add(newfPoint);
                    newfPoint.RenderTransform = tgroup;
                }
            }
            _outputFieldLines = fieldLines;
        }

        private void MnuExportClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {Title = "Choose location ...", Filter = "JPEG Image File (*.jpg)|*.jpg"};

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var stream = new FileStream(dialog.FileName, FileMode.Create);
                var encoder = new JpegBitmapEncoder {QualityLevel = 100};
                encoder.Frames.Add(BitmapFrame.Create(_outputdata));
                encoder.Save(stream);
                stream.Close();

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

        private void MergImages()
        {
        }

        private void MnuShowHideFieldLinesClick(object sender, RoutedEventArgs e)
        {
        }
    }
}