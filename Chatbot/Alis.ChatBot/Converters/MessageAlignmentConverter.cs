using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Alis.ChatBot.Models;

namespace Alis.ChatBot.Converters
{
    /// <summary>
    /// MessageRole을 HorizontalAlignment로 변환합니다.
    /// User → Right, 나머지 → Left.
    /// </summary>
    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageRole role && role == MessageRole.User)
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
