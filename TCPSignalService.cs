using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalMonitor
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITcpSignalService
    {
        Task<bool> ConnectAsync();
        Task<string?> RequestSignalAsync();
        void Disconnect();
        bool IsConnected { get; }
    }

    public class TcpSignalService : ITcpSignalService
    {
        private readonly string _host = "localhost";
        private readonly int _port = 8888;
        private TcpClient? _client;
        private NetworkStream? _stream;

        public bool IsConnected => _client?.Connected ?? false;

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_host, _port);
                _stream = _client.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection Error: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> RequestSignalAsync()
        {
            if (!IsConnected || _stream == null) return null;

            try
            {
                var command = Encoding.ASCII.GetBytes("SEND GET_SIGNAL\n");
                await _stream.WriteAsync(command, 0, command.Length);

                var buffer = new byte[256];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (response.StartsWith("RECV GET_SIGNAL"))
                {
                    return response.Replace("RECV GET_SIGNAL ", "").Trim();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RequestSignalAsync: {ex.Message}");
                return null;
            }
        }

        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnection Error: {ex.Message}");
            }
        }
    }
}
