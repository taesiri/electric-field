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
    /// Interaction logic for FieldPoint.xaml
    /// </summary>
    public partial class FieldPoint
    {
        private Charge mcharge;

        public Charge Mcharge
        {
            get { return mcharge; }
            set
            {
                mcharge = value;
            }
        }

        public FieldPoint()
        {
            InitializeComponent();
            mcharge = new Charge();
        }
        public FieldPoint(Charge charge)
        {
            InitializeComponent();
            mcharge = charge;
        }

        public void GenerateTooltipContent()
        {
            fieldtooltip.Content = "Statistics" + Environment.NewLine + "Charge Type : " + mcharge.Type +
                                   Environment.NewLine + "Electric Charge amount : " + mcharge.ElectricCharge;
                //+ Environment.NewLine 
        }

        private void FieldtooltipToolTipOpening(object sender, ToolTipEventArgs e)
        {
          
        }

        private void UserControlMouseEnter(object sender, MouseEventArgs e)
        {
            GenerateTooltipContent();
        }
    }
}
