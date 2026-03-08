using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Alis.ChatBot.ViewModels;

namespace Alis.ChatBot.Views
{
    public partial class ChatBotWindow : Window
    {
        private readonly ChatBotViewModel _viewModel;

        public ChatBotWindow()
        {
            InitializeComponent();
            _viewModel = new ChatBotViewModel("https://hq.spelix.co.kr");
            DataContext = _viewModel;
            _viewModel.ScrollToBottomRequested += OnScrollToBottomRequested;
            _viewModel.LogoutRequested += OnLogoutRequested;
        }

        private void OnScrollToBottomRequested()
        {
            Dispatcher.BeginInvoke((Action)(() => MessageScroller.ScrollToEnd()));
        }

        private void OnLogoutRequested()
        {
            Dispatcher.BeginInvoke((Action)(() => PasswordBox.Clear()));
        }

        /// <summary>
        /// PasswordBox는 보안상 바인딩 불가 → code-behind에서 PasswordChanged 이벤트 처리
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChatBotViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }

        /// <summary>
        /// Enter키 전송 (Shift+Enter는 무시하여 줄바꿈 허용)
        /// </summary>
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                if (_viewModel.SendCommand.CanExecute(null))
                {
                    _viewModel.SendCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Window 닫을 때 HealthCheck 타이머 정리
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.ScrollToBottomRequested -= OnScrollToBottomRequested;
            _viewModel.LogoutRequested -= OnLogoutRequested;

            if (_viewModel.LogoutCommand.CanExecute(null))
            {
                _viewModel.LogoutCommand.Execute(null);
            }
        }
    }
}
