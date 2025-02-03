using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalMonitor
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class SignalViewModel : ViewModelBase
    {
        private readonly ITcpSignalService _tcpSignalService;
        private bool _isConnected;

        public ObservableCollection<string> SignalValues { get; set; } = new();

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand { get; }
        public ICommand RequestSignalCommand { get; }
        public ICommand DisconnectCommand { get; }

        public SignalViewModel(ITcpSignalService tcpSignalService)
        {
            _tcpSignalService = tcpSignalService;

            ConnectCommand = new RelayCommand(async () => await ConnectAsync(), () => !IsConnected);
            RequestSignalCommand = new RelayCommand(async () => await RequestSignalAsync(), () => IsConnected);
            DisconnectCommand = new RelayCommand(Disconnect, () => IsConnected);
        }

        private async Task ConnectAsync()
        {
            IsConnected = await _tcpSignalService.ConnectAsync();
        }

        private async Task RequestSignalAsync()
        {
            string? signalValue = await _tcpSignalService.RequestSignalAsync();
            if (signalValue != null)
            {
                SignalValues.Add($"{DateTime.Now:T}: {signalValue}");
            }
        }

        private void Disconnect()
        {
            _tcpSignalService.Disconnect();
            IsConnected = false;
        }
    }
}
