using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR7WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChatWindow chatWindow = new ChatWindow();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateChat_Click(object sender, RoutedEventArgs e)
        {
            if(NameText.Text == "")
            {
                MessageBox.Show("Введено некорректное имя пользователя");
            }
            else
            {
                AdminChatWindow a = new AdminChatWindow(NameText.Text);
                a.Show();
                this.Close();
            }
        }

        private void JoinChat_Click(object sender, RoutedEventArgs e)
        {
            if(NameText.Text == "" && IPText.Text == "")
            {
                MessageBox.Show("Не все поля заполнены");
            }
            else
            {
                chatWindow.chatName = NameText.Text;
                chatWindow.chatIP = IPText.Text;
                chatWindow.Show();
                this.Close();
            }
        }
    }
}
