using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Collections.ObjectModel;

namespace PR7WPF
{
    /// <summary>
    /// Логика взаимодействия для AdminChatWindow.xaml
    /// </summary>

    public class MessageData
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LogEntry
    {
        public DateTime Time { get; set; }
        public string Username { get; set; }
        public string Event { get; set; }
    }
    public partial class AdminChatWindow : Window
    {
        TCPServer t = new TCPServer();

        TCPClient tp = new TCPClient();
        public AdminChatWindow(string Adminame)
        {
            InitializeComponent();
            UserList.ItemsSource = t.users;
            UserList.DisplayMemberPath = "Name";
            MessageList.ItemsSource = t.messageData;
            t.NameA = Adminame;
            t.AddUser(Adminame);
        }

        private void Logs_Click(object sender, RoutedEventArgs e)
        {

            UserList.ItemsSource = t.chatLogs;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            t.Close();
            t.RemoveUser();
            this.Close();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            tp.SendMessage(Message.Text);
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = Message.Text;
            if (text == "/disconnect" || text == "/dis" || text == "/Disconnect")
            {
                t.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            t.Close();
        }
    }

    public class TCPServer
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private CancellationTokenSource cts = new CancellationTokenSource();
        
        internal ObservableCollection<User> users = new ObservableCollection<User>();
        
        internal ObservableCollection<LogEntry> chatLogs = new ObservableCollection<LogEntry>();
        
        internal ObservableCollection<MessageData> messageData = new ObservableCollection<MessageData>();
        
        List<Socket> clients = new List<Socket>();
        
        public string NameA;
        public void Connect()
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, 6000));
            socket.Listen(100);
            Listen();
        }
        private async void Listen()
        {
            while (!cts.IsCancellationRequested)
            {
                var client = await socket.AcceptAsync(cts.Token);
                clients.Add(client);
                ReceiveMessage(client);
            }
        }
        private async void ReceiveMessage(Socket client)
        {
            while (!cts.IsCancellationRequested)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None, cts.Token);
                string message = Encoding.UTF8.GetString(bytes);
                messageData.Add(new MessageData { Username = "User", Message = message, Timestamp = DateTime.Now });
                foreach (var item in clients)
                {
                    SendMessage(message, item);
                }
            }
        }

        public async void SendMessage(string message, Socket client)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }
        public void Close()
        {
            MainWindow mainWindow = new MainWindow();
            cts.Cancel();
            socket.Close();
            mainWindow.Show();
        }
        public void AddUser(string username)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    users.Add(new User { Name = username, time = DateTime.Now });
                    chatLogs.Add(new LogEntry { Time = DateTime.Now, Username = username, Event = "Подключился" });
                });
            });
        }
        public void RemoveUser()
        {
            Task.Run(() =>
            {
                string username = NameA;
                User userToRemove = users.FirstOrDefault(u => u.Name == username);
                if (userToRemove != null)
                {
                    users.Remove(userToRemove);
                    chatLogs.Add(new LogEntry { Time = DateTime.Now, Username = username, Event = "Отключился" });
                }
            });
        }
    }
}
