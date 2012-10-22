using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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
            IEnumerable<UIElement> allitems = MainWindow.Instance.GetListOfItems();
            foreach (UIElement chargeitem in allitems)
            {
                if (chargeitem.GetType() == typeof (PositiveCharge))
                {
                    string name = ((PositiveCharge) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
                else if (chargeitem.GetType() == typeof (NegativeCharge))
                {
                    string name = ((NegativeCharge) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
                else if (chargeitem.GetType() == typeof (FreeCharge))
                {
                    string name = ((FreeCharge) chargeitem).MyCharge.Name;
                    lstBoxData.Items.Add(name);
                }
                else if (chargeitem.GetType() == typeof (Surface))
                {
                    string name = ((Surface) chargeitem).MyCharge.Name;
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

            IEnumerable<UIElement> allitems = MainWindow.Instance.GetListOfItems();
            foreach (UIElement chargeitem in allitems)
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
                else if (chargeitem.GetType() == typeof (FreeCharge))
                {
                    if (lstBoxData.SelectedItem == null)
                        return;
                    name = ((FreeCharge) chargeitem).MyCharge.Name;
                    if (name == lstBoxData.SelectedItem.ToString())
                    {
                        ((FreeCharge) chargeitem).ShowSettings();
                    }
                }
                else if (chargeitem.GetType() == typeof (Surface))
                {
                    if (lstBoxData.SelectedItem == null)
                        return;
                    name = ((Surface) chargeitem).MyCharge.Name;
                    if (name == lstBoxData.SelectedItem.ToString())
                    {
                        ((Surface) chargeitem).ShowSettings();
                    }
                }
            }
        }

        private void ElementOutlineWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}