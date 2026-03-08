using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Alis.ChatBot.Converters
{
    /// <summary>
    /// bool을 Visibility로 변환합니다.
    /// true → Visible, false → Collapsed.
    /// ConverterParameter에 "Invert"를 지정하면 반전됩니다.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;

            if (value is bool b)
                flag = b;
            else if (value is string str)
                flag = !string.IsNullOrEmpty(str);
            else if (value != null)
                flag = true;

            // Invert 파라미터 지원
            if (parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase))
                flag = !flag;

            // IsInverted 프로퍼티 지원
            if (IsInverted)
                flag = !flag;

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                var result = v == Visibility.Visible;

                if (parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase))
                    result = !result;

                if (IsInverted)
                    result = !result;

                return result;
            }

            return false;
        }
    }
}
