using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CardManager
{
    class NightMode
    {
        MainWindow mw;       

        public NightMode(MainWindow mw)
        {
            this.mw = mw;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public void NightModeOn()
        {
            mw.nightMode.Content = "ON";
            mw.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            mw.nightMode.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            Style style1 = mw.FindResource("toggleButtonStyle") as Style;
            mw.nightMode.Style = style1;
            mw.abuseProgress.Background = new SolidColorBrush(Color.FromRgb(25, 25, 25));

            foreach (Button tb in FindVisualChildren<Button>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                Style style = mw.FindResource("buttonStyle") as Style;
                tb.Style = style;
            }
            foreach (Label tb in FindVisualChildren<Label>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            }
            foreach (ListBox tb in FindVisualChildren<ListBox>(mw.myWindow))
            {
                tb.Background = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            }
            foreach (Menu tb in FindVisualChildren<Menu>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                tb.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            }
            foreach (MenuItem mi in FindVisualChildren<MenuItem>(mw.myWindow))
            {
                mi.Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            }
            foreach (TextBox tb in FindVisualChildren<TextBox>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                tb.Background = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            }
            foreach (TextBlock tb in FindVisualChildren<TextBlock>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            }
            //if (checkboxlist.CB_Llist != null)
            //{
            //    foreach (var item in checkboxlist.CB_Llist)
            //    {
            //        item.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            //    }
            //}
        }

        public void NightModeOff()
        {
            mw.nightMode.Content = "OFF";
            mw.ClearValue(Control.BackgroundProperty);
            mw.nightMode.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            mw.nightMode.Style = null;
            mw.abuseProgress.ClearValue(Control.BackgroundProperty);

            foreach (Label lb in FindVisualChildren<Label>(mw.myWindow))
            {
                lb.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            foreach (Button tb in FindVisualChildren<Button>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                tb.ClearValue(Control.BackgroundProperty);
                tb.Style = null;
            }
            foreach (ListBox lb in FindVisualChildren<ListBox>(mw.myWindow))
            {
                lb.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                lb.ClearValue(Control.BackgroundProperty);
            }
            foreach (Menu m in FindVisualChildren<Menu>(mw.myWindow))
            {
                m.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            foreach (TextBox tb in FindVisualChildren<TextBox>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                tb.ClearValue(Control.BackgroundProperty);
            }
            foreach (ComboBox tb in FindVisualChildren<ComboBox>(mw.myWindow))
            {
                tb.Resources.Clear();
            }
            foreach (TextBlock tb in FindVisualChildren<TextBlock>(mw.myWindow))
            {
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            //if (checkboxlist.CB_Llist != null)
            //{
            //    foreach (var item in checkboxlist.CB_Llist)
            //    {
            //        item.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            //    }
            //}         
        }
    }
}
