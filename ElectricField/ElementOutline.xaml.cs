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
using System.Windows.Shapes;
using ElectricField.Controls;

namespace ElectricField
{
    /// <summary>
    /// Interaction logic for ElementOutline.xaml
    /// </summary>
    public partial class ElementOutline : Window
    {
        public ElementOutline()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Clear();
            var allitems = MainWindow.Instance.GetListOfItems();
            foreach (var chargeitem in allitems)
            {
                if (chargeitem.GetType() == typeof (PositiveCharge))
                {
                    var name = ((PositiveCharge) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
                else if (chargeitem.GetType() == typeof (NegativeCharge))
                {
                    var name = ((NegativeCharge) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
                else if (chargeitem.GetType() == typeof (FreeCharge))
                {
                    var name = ((FreeCharge) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
                else if (chargeitem.GetType() == typeof (Surface))
                {
                    var name = ((Surface) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
            }
        }

        public void Clear()
        {
            lstBoxData.Items.Clear();
        }

        private void BtnUpdateClick(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void LstBoxDataMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstBoxData.SelectedItem == null)
                return;
            
            var allitems = MainWindow.Instance.GetListOfItems();
            foreach (var chargeitem in allitems)
            {
                string name = "";
                if (chargeitem.GetType() == typeof (PositiveCharge))
                {
                    if (lstBoxData.SelectedItem == null)
                        return;
                    name = ((PositiveCharge) chargeitem).MyCharge.Name;
                    if (name == lstBoxData.SelectedItem.ToString())
                    {
                        ((PositiveCharge) chargeitem).ShowSettings();
                    }
                }
                else if (chargeitem.GetType() == typeof (NegativeCharge))
                {
                    if (lstBoxData.SelectedItem == null)
                        return;
                    name = ((NegativeCharge) chargeitem).MyCharge.Name;
                    if (name == lstBoxData.SelectedItem.ToString())
                    {
                        ((NegativeCharge) chargeitem).ShowSettings();
                    }
                }
                else if (chargeitem.GetType() == typeof(FreeCharge))
                {
                    if (lstBoxData.SelectedItem == null)
                        return;
                    name = ((FreeCharge)chargeitem).MyCharge.Name;
                    if (name == lstBoxData.SelectedItem.ToString())
                    {
                        ((FreeCharge)chargeitem).ShowSettings();
                    }
                }
                else if (chargeitem.GetType() == typeof(Surface))
                {
                    if (lstBoxData.SelectedItem == null)
                        return;
                    name = ((Surface)chargeitem).MyCharge.Name;
                    if (name == lstBoxData.SelectedItem.ToString())
                    {
                        ((Surface)chargeitem).ShowSettings();
                    }
                }
            }
        }

        private void ElementOutlineWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

    }
}
