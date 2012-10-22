using System;
using System.Windows.Controls;
using System.Windows.Input;
using ElectricField.Classes;

namespace ElectricField.Controls
{
    /// <summary>
    /// Interaction logic for FieldPoint.xaml
    /// </summary>
    public partial class FieldPoint
    {
        private Charge mcharge;

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

        public Charge Mcharge
        {
            get { return mcharge; }
            set { mcharge = value; }
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