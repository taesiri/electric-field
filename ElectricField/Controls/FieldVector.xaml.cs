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
using ElectricField.Classes;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for FieldVector.xaml
    /// </summary>
    public partial class FieldVector : UserControl
    {
        private double _force = 0;
        private Vector _forceVector;
        private double _rAngel;
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

        public void DisplayAngelChanged()
        {
            var group = new TransformGroup();
            var rotate = new RotateTransform {Angle = RAngel};
            group.Children.Add(rotate);

            VecPath.RenderTransform = group;

        }

        public double Force
        {
            get { return _force; }
        }


        public FieldVector()
        {
            InitializeComponent();
        }

        public void CalculateAngel()
        {
            var vatar = Helper.VectorMagnitude(_forceVector);
            var sinusValue = Math.Abs(_forceVector.Y)/vatar;
            var temp = Helper.RadianToDegree((Math.Asin(sinusValue)));

            if (_forceVector.X >0 && _forceVector.Y > 0 ) //First
            {
                RAngel = 90 - temp;
            }
            else if (_forceVector.X >0 && _forceVector.Y < 0 ) //Third
            {
                RAngel = temp + 90;
            }
            else if (_forceVector.X < 0 && _forceVector.Y > 0)//Second
            {
                RAngel = temp + 270;
            }
            else if (_forceVector.X < 0 && _forceVector.Y < 0)//Fourth
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
                                                Color.FromRgb((byte)(Helper.Clamp(Math.Abs(_force * 15), 40, 255)), (byte)(Helper.Clamp(Math.Abs(_force * 25), 150, 220)), (byte)(Helper.Clamp(Math.Abs(_force * 24),210, 255)))
                                        };

            VecPath.Fill = mySolidColorBrush;
            if (_force < 15)
            {
                 VecPath.Opacity = (_force * 0.9) / 10;
            }
            else
            {
                VecPath.Opacity = 100;
            }
        }
    }
}