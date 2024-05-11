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
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    /// 
    class User
    {
        public string Name { get; set; }
        public DateTime time { get; set; }
    }
    public partial class ChatWindow : Window
    {
        TCPClient t = new TCPClient();

        public string chatName;

        public string chatIP;

        public ChatWindow()
        {
            InitializeComponent();
            UserList.ItemsSource = t.users;
            MessageList.ItemsSource = t.messageDataU;
            t.Name = chatName;
            t.IP = chatIP;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            t.Close();
            t.RemoveUser();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            t.SendMessage(Message.Text);
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = Message.Text;
            if (text == "/disconnect" || text == "/dis" || text == "/Disconnect")
            {
                t.Close();
            }
        }

        private void ChatWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            t.Close();
        }
    }

    public class TCPClient
    {
        Socket socket;

        private CancellationTokenSource cts = new CancellationTokenSource();

        internal ObservableCollection<User> users = new ObservableCollection<User>();

        internal ObservableCollection<MessageData> messageDataU = new ObservableCollection<MessageData>();

        public string IP;

        public string Name;

        public TCPClient()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connect();
        }

        public void Connect()
        {
            socket.Connect(IP, 6000);
            Task.Run(() => ReceiveMessage(cts.Token));
        }

        public async void SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(bytes, SocketFlags.None);
        }

        private async void ReceiveMessage(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                byte[] bytes = new byte[1024];
                await socket.ReceiveAsync(bytes, SocketFlags.None, token);
                string message = Encoding.UTF8.GetString(bytes);
                messageDataU.Add(new MessageData { Username = Name, Message = message, Timestamp = DateTime.Now });
            }
        }
        public void Close()
        {
            MainWindow mainWindow = new MainWindow();
            cts.Cancel();
            socket.Close();
            mainWindow.Show();
        }

        public void AddUser()
        {
            Task.Run(() =>
            {
                users.Add(new User { Name = Name, time = DateTime.Now });
            });
        }

        public void RemoveUser()
        {
            Task.Run(() =>
            {
                User userToRemove = users.FirstOrDefault(u => u.Name == Name);
                if (userToRemove != null)
                {
                    users.Remove(userToRemove);
                }
            });
        }
    }
}
