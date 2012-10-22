using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ElectricField.Classes;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for FieldVector.xaml
    /// </summary>
    public partial class FieldVector : UserControl
    {
        private double _force;
        private Vector _forceVector;
        private double _rAngel;

        public FieldVector()
        {
            InitializeComponent();
        }

        //private Charge.ChargeType ChType { get; set; }
        public Vector ForceVector
        {
            get { return _forceVector; }
            set
            {
                _forceVector = value;
                _force = Helper.VectorMagnitude(_forceVector);
                CalculateAngel();
                Colorlize();
            }
        }

        public double RAngel
        {
            get { return _rAngel; }
            set
            {
                _rAngel = value;
                DisplayAngelChanged();
            }
        }

        public double Force
        {
            get { return _force; }
        }

        public void DisplayAngelChanged()
        {
            var group = new TransformGroup();
            var rotate = new RotateTransform {Angle = RAngel};
            group.Children.Add(rotate);

            VecPath.RenderTransform = group;
        }


        public void CalculateAngel()
        {
            double vatar = Helper.VectorMagnitude(_forceVector);
            double sinusValue = Math.Abs(_forceVector.Y)/vatar;
            double temp = Helper.RadianToDegree((Math.Asin(sinusValue)));

            if (_forceVector.X > 0 && _forceVector.Y > 0) //First
            {
                RAngel = 90 - temp;
            }
            else if (_forceVector.X > 0 && _forceVector.Y < 0) //Third
            {
                RAngel = temp + 90;
            }
            else if (_forceVector.X < 0 && _forceVector.Y > 0) //Second
            {
                RAngel = temp + 270;
            }
            else if (_forceVector.X < 0 && _forceVector.Y < 0) //Fourth
            {
                RAngel = (90 - temp) + 180;
            }
        }

        public void GenerateTooltipContent()
        {
            fieldtooltip.Content = "Statistics" + Environment.NewLine + "Total amount of Forces : " + _force +
                                   Environment.NewLine + "Vector : " + ForceVector.ToString() + Environment.NewLine +
                                   "rAngel : " + _rAngel;
        }

        private void UserControlMouseEnter(object sender, MouseEventArgs e)
        {
            GenerateTooltipContent();
        }

        private void Colorlize()
        {
            var mySolidColorBrush = new SolidColorBrush
                                        {
                                            Color =
                                                Color.FromRgb((byte) (Helper.Clamp(Math.Abs(_force*15), 40, 255)),
                                                              (byte) (Helper.Clamp(Math.Abs(_force*25), 150, 220)),
                                                              (byte) (Helper.Clamp(Math.Abs(_force*24), 210, 255)))
                                        };

            VecPath.Fill = mySolidColorBrush;
            if (_force < 15)
            {
                VecPath.Opacity = (_force*0.9)/10;
            }
            else
            {
                VecPath.Opacity = 100;
            }
        }
    }
}