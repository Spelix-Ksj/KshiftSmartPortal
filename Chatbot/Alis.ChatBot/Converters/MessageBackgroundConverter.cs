using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Alis.ChatBot.Models;

namespace Alis.ChatBot.Converters
{
    /// <summary>
    /// MessageRole을 배경색 Brush로 변환합니다.
    /// User → #0F4C81 (ALIS 블루)
    /// Assistant → #F0F0F0 (연회색)
    /// System → #E8F5E9 (연녹색)
    /// Error → #FFEBEE (연빨강)
    /// </summary>
    public class MessageBackgroundConverter : IValueConverter
    {
        private static readonly SolidColorBrush UserBrush = new SolidColorBrush(Color.FromRgb(0x0F, 0x4C, 0x81));
        private static readonly SolidColorBrush AssistantBrush = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
        private static readonly SolidColorBrush SystemBrush = new SolidColorBrush(Color.FromRgb(0xE8, 0xF5, 0xE9));
        private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xEB, 0xEE));

        static MessageBackgroundConverter()
        {
            UserBrush.Freeze();
            AssistantBrush.Freeze();
            SystemBrush.Freeze();
            ErrorBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageRole role)
            {
                switch (role)
                {
                    case MessageRole.User:
                        return UserBrush;
                    case MessageRole.Assistant:
                        return AssistantBrush;
                    case MessageRole.System:
                        return SystemBrush;
                    case MessageRole.Error:
                        return ErrorBrush;
                }
            }

            return AssistantBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
