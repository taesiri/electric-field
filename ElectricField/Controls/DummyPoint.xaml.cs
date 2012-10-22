using System.Windows;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for DummyPoint.xaml
    /// </summary>
    public partial class DummyPoint
    {
        public DummyPoint()
        {
            InitializeComponent();
        }

        public Point StartPoint { get; set; }
        public Vector LastVector { get; set; }

        public void Restart()
        {
            Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0);
        }
    }
}