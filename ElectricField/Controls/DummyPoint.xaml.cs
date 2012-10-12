using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for DummyPoint.xaml
    /// </summary>
    public partial class DummyPoint
    {
        public Point StartPoint { get; set; }
        public Vector LastVector { get; set; }

        public DummyPoint()
        {
            InitializeComponent();

        }

        public void Restart()
        {
            this.Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0);
        }
    }
}