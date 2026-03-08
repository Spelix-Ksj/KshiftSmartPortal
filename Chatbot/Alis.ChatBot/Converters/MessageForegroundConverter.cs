using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Alis.ChatBot.Models;

namespace Alis.ChatBot.Converters
{
    /// <summary>
    /// MessageRole을 전경색 Brush로 변환합니다.
    /// User → White
    /// Assistant/System/Error → #333333 (진회색)
    /// </summary>
    public class MessageForegroundConverter : IValueConverter
    {
        private static readonly SolidColorBrush UserBrush = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        static MessageForegroundConverter()
        {
            UserBrush.Freeze();
            DefaultBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageRole role && role == MessageRole.User)
                return UserBrush;

            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
