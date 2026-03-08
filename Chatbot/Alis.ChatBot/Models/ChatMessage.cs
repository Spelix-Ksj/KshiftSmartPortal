using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Alis.ChatBot.Models
{
    public enum MessageRole
    {
        User,
        Assistant,
        System,
        Error
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        private MessageRole _role;
        private string _content;
        private string _sql;
        private string _reasoning;
        private string _report;
        private DataTable _resultTable;
        private int _rowCount;
        private bool _isLoading;
        private string _editableSql;
        private DateTime _timestamp;

        public ChatMessage()
        {
            _timestamp = DateTime.Now;
        }

        public MessageRole Role
        {
            get { return _role; }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged();
                    OnPropertyChanged("HasError");
                }
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Sql
        {
            get { return _sql; }
            set
            {
                if (_sql != value)
                {
                    _sql = value;
                    OnPropertyChanged();
                    OnPropertyChanged("HasSql");

                    // EditableSql 초기값 설정
                    if (_editableSql == null && value != null)
                    {
                        _editableSql = value;
                        OnPropertyChanged("EditableSql");
                    }
                }
            }
        }

        public string Reasoning
        {
            get { return _reasoning; }
            set
            {
                if (_reasoning != value)
                {
                    _reasoning = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Report
        {
            get { return _report; }
            set
            {
                if (_report != value)
                {
                    _report = value;
                    OnPropertyChanged();
                    OnPropertyChanged("HasReport");
                }
            }
        }

        public DataTable ResultTable
        {
            get { return _resultTable; }
            set
            {
                if (_resultTable != value)
                {
                    _resultTable = value;
                    OnPropertyChanged();
                    OnPropertyChanged("HasResult");
                }
            }
        }

        public int RowCount
        {
            get { return _rowCount; }
            set
            {
                if (_rowCount != value)
                {
                    _rowCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasSql
        {
            get { return !string.IsNullOrEmpty(_sql); }
        }

        public bool HasResult
        {
            get { return _resultTable != null && _resultTable.Rows.Count > 0; }
        }

        public bool HasReport
        {
            get { return !string.IsNullOrEmpty(_report); }
        }

        public bool HasError
        {
            get { return _role == MessageRole.Error; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EditableSql
        {
            get { return _editableSql; }
            set
            {
                if (_editableSql != value)
                {
                    _editableSql = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
