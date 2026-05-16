using System.Net.Sockets;
using System.Text;

namespace SyncShoppingList.Mobile
{
    public class ChatService
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private bool _isConnected;

        public async Task<bool> ConnectAsync(string host, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(host, port);
                _stream = _client.GetStream();
                _isConnected = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (!_isConnected || _stream == null) return;
            
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            if (!_isConnected || _stream == null) return "";
            
            byte[] buffer = new byte[4096];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            _isConnected = false;
        }
    }
}