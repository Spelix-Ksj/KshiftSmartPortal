using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Alis.ChatBot.Models;

namespace Alis.ChatBot.ViewModels
{
    public class ChatBotViewModel : INotifyPropertyChanged
    {
        private readonly GatewayApiClient _apiClient;
        private readonly DispatcherTimer _healthTimer;

        private string _inputText;
        private bool _isBusy;
        private bool _isLoggedIn;
        private string _statusText;
        private string _userId;
        private string _password;
        private string _loginError;

        public ChatBotViewModel(string apiBaseUrl)
        {
            _apiClient = new GatewayApiClient(apiBaseUrl);
            Messages = new ObservableCollection<ChatMessage>();
            _statusText = "로그인 필요";

            // Commands
            LoginCommand = new RelayCommand(async () => await OnLoginAsync(), () => !IsBusy);
            SendCommand = new RelayCommand(async () => await OnSendAsync(), () => !IsBusy && IsLoggedIn && !string.IsNullOrWhiteSpace(InputText));
            ExecuteSqlCommand = new RelayCommand<ChatMessage>(async msg => await OnExecuteSqlAsync(msg), msg => !IsBusy && msg != null);
            CopySqlCommand = new RelayCommand<ChatMessage>(OnCopySql, msg => msg != null && msg.HasSql);
            LogoutCommand = new RelayCommand(OnLogout, () => IsLoggedIn);
            SampleQuestionCommand = new RelayCommand<string>(OnSampleQuestion, q => !IsBusy && IsLoggedIn);

            // 헬스체크 타이머 (30초 간격)
            _healthTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _healthTimer.Tick += OnHealthCheck;
        }

        #region Properties

        public ObservableCollection<ChatMessage> Messages { get; }

        public string InputText
        {
            get { return _inputText; }
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoginError
        {
            get { return _loginError; }
            set
            {
                if (_loginError != value)
                {
                    _loginError = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand ExecuteSqlCommand { get; }
        public ICommand CopySqlCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SampleQuestionCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// View에서 구독하여 채팅 스크롤을 맨 아래로 이동시킵니다.
        /// </summary>
        public event Action ScrollToBottomRequested;
        public event Action LogoutRequested;

        #endregion

        #region Command Handlers

        private async System.Threading.Tasks.Task OnLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(Password))
            {
                LoginError = "사용자 ID와 비밀번호를 입력하세요.";
                return;
            }

            IsBusy = true;
            LoginError = null;

            try
            {
                var success = await _apiClient.LoginAsync(UserId, Password);

                if (success)
                {
                    IsLoggedIn = true;
                    StatusText = "연결됨";
                    LoginError = null;
                    Password = null; // 비밀번호 메모리에서 제거

                    AddMessage(new ChatMessage
                    {
                        Role = MessageRole.System,
                        Content = "로그인 성공! 질문을 입력하세요."
                    });

                    // 헬스체크 타이머 시작
                    _healthTimer.Start();
                }
                else
                {
                    LoginError = "로그인에 실패했습니다. ID와 비밀번호를 확인하세요.";
                }
            }
            catch (Exception ex)
            {
                LoginError = "로그인 오류: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async System.Threading.Tasks.Task OnSendAsync()
        {
            var question = InputText?.Trim();
            if (string.IsNullOrEmpty(question))
                return;

            IsBusy = true;
            InputText = string.Empty;

            try
            {
                // 1) User 메시지 추가
                AddMessage(new ChatMessage
                {
                    Role = MessageRole.User,
                    Content = question
                });

                // 2) 로딩 메시지 추가
                var assistantMsg = new ChatMessage
                {
                    Role = MessageRole.Assistant,
                    Content = "생각 중...",
                    IsLoading = true
                };
                AddMessage(assistantMsg);
                ScrollToBottomRequested?.Invoke();

                // 3) Ask API 호출
                var askResponse = await _apiClient.AskAsync(question);

                // 4) 로딩 메시지 업데이트
                if (!string.IsNullOrEmpty(askResponse.Error))
                {
                    // 에러 발생
                    assistantMsg.Role = MessageRole.Error;
                    assistantMsg.Content = askResponse.Error;
                    assistantMsg.IsLoading = false;
                    ScrollToBottomRequested?.Invoke();
                    return;
                }

                assistantMsg.Reasoning = askResponse.Reasoning;
                assistantMsg.Sql = askResponse.Sql;
                assistantMsg.EditableSql = askResponse.Sql;
                assistantMsg.Content = askResponse.Reasoning;
                assistantMsg.IsLoading = false;

                ScrollToBottomRequested?.Invoke();

                // 5) SQL이 있으면 자동 Execute
                if (!string.IsNullOrEmpty(askResponse.Sql))
                {
                    assistantMsg.IsLoading = true;

                    var execResponse = await _apiClient.ExecuteAsync(
                        askResponse.Sql, question, askResponse.Reasoning);

                    assistantMsg.IsLoading = false;

                    if (!string.IsNullOrEmpty(execResponse.Error))
                    {
                        assistantMsg.Report = "실행 오류: " + execResponse.Error;
                    }
                    else if (execResponse.RowCount == 0)
                    {
                        assistantMsg.RowCount = 0;
                        assistantMsg.Report = "조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요.";
                    }
                    else
                    {
                        // 6) 결과 설정
                        assistantMsg.ResultTable = GatewayApiClient.ToDataTable(execResponse);
                        assistantMsg.RowCount = execResponse.RowCount;
                        assistantMsg.Report = execResponse.Report;
                    }
                }

                ScrollToBottomRequested?.Invoke();
            }
            catch (Exception ex)
            {
                AddMessage(new ChatMessage
                {
                    Role = MessageRole.Error,
                    Content = "오류가 발생했습니다: " + ex.Message
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async System.Threading.Tasks.Task OnExecuteSqlAsync(ChatMessage msg)
        {
            if (msg == null || string.IsNullOrWhiteSpace(msg.EditableSql))
                return;

            IsBusy = true;

            try
            {
                msg.IsLoading = true;

                var execResponse = await _apiClient.ExecuteAsync(
                    msg.EditableSql, msg.Content, msg.Reasoning);

                msg.IsLoading = false;

                if (!string.IsNullOrEmpty(execResponse.Error))
                {
                    msg.Report = "실행 오류: " + execResponse.Error;
                    msg.ResultTable = null;
                    msg.RowCount = 0;
                }
                else if (execResponse.RowCount == 0)
                {
                    msg.Sql = msg.EditableSql;
                    msg.ResultTable = null;
                    msg.RowCount = 0;
                    msg.Report = "조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요.";
                }
                else
                {
                    msg.Sql = msg.EditableSql;
                    msg.ResultTable = GatewayApiClient.ToDataTable(execResponse);
                    msg.RowCount = execResponse.RowCount;
                    msg.Report = execResponse.Report;
                }

                ScrollToBottomRequested?.Invoke();
            }
            catch (Exception ex)
            {
                msg.IsLoading = false;
                msg.Report = "실행 오류: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnCopySql(ChatMessage msg)
        {
            if (msg == null || !msg.HasSql)
                return;

            try
            {
                var sqlText = !string.IsNullOrEmpty(msg.EditableSql) ? msg.EditableSql : msg.Sql;
                Clipboard.SetText(sqlText);
            }
            catch (Exception)
            {
                // 클립보드 접근 실패 무시
            }
        }

        private async void OnSampleQuestion(string question)
        {
            if (string.IsNullOrWhiteSpace(question) || IsBusy || !IsLoggedIn)
                return;

            InputText = question;
            await OnSendAsync();
        }

        private void OnLogout()
        {
            _healthTimer.Stop();
            _apiClient.Logout();

            IsLoggedIn = false;
            StatusText = "로그인 필요";
            Messages.Clear();
            InputText = string.Empty;
            UserId = null;
            Password = null;
            LoginError = null;
            LogoutRequested?.Invoke();
        }

        #endregion

        #region Health Check

        private async void OnHealthCheck(object sender, EventArgs e)
        {
            try
            {
                var healthy = await _apiClient.CheckHealthAsync();
                StatusText = healthy ? "연결됨" : "GPU 연결 끊김";
            }
            catch (Exception)
            {
                StatusText = "GPU 연결 끊김";
            }
        }

        #endregion

        #region Helpers

        private void AddMessage(ChatMessage msg)
        {
            if (Application.Current?.Dispatcher != null &&
                !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
            }
            else
            {
                Messages.Add(msg);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
