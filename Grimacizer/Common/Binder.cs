using System.Windows;
using System.Windows.Controls;

namespace Grimacizer7.Common
{
    public static class Binder
    {
        public static string GetRealTimeText(TextBox obj)
        {
            return (string)obj.GetValue(RealTimeTextProperty);
        }

        public static void SetRealTimeText(TextBox obj, string value)
        {
            obj.SetValue(RealTimeTextProperty, value);
        }

        public static readonly DependencyProperty RealTimeTextProperty =
            DependencyProperty.RegisterAttached("RealTimeText", typeof(string), typeof(Binder), null);

        public static bool GetIsAutoUpdate(TextBox obj)
        {
            return (bool)obj.GetValue(IsAutoUpdateProperty);
        }

        public static void SetIsAutoUpdate(TextBox obj, bool value)
        {
            obj.SetValue(IsAutoUpdateProperty, value);
        }

        public static readonly DependencyProperty IsAutoUpdateProperty =
            DependencyProperty.RegisterAttached("IsAutoUpdate", typeof(bool), typeof(Binder), new PropertyMetadata(false, OnIsAutoUpdateChanged));

        private static void OnIsAutoUpdateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            if (textbox == null)
                return;

            if ((bool)e.NewValue)
                textbox.TextChanged += textbox_TextChanged;
            else
                textbox.TextChanged -= textbox_TextChanged;
        }

        private static void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.SetValue(Binder.RealTimeTextProperty, textbox.Text);
        }
    }
}
