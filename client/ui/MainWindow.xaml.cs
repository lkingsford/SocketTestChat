using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;

namespace client.ui
{
    public class MainWindow : Window
    {
        internal DispatcherTimer networkPoller;
        client.Client activeClient;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            activeClient = new client.Client();
        }

        Button SendButton;
        TextBox Messages;
        TextBox MessageToSend;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            SendButton = this.Find<Button>("Send");
            MessageToSend = this.Find<TextBox>("MessageToSend");
            Messages = this.Find<TextBox>("Messages");

            SendButton.Click += SendButton_Click;

            activeClient.Init();
            activeClient.messageWrite = AddMessage;

            networkPoller = new DispatcherTimer();
            networkPoller.Interval = System.TimeSpan.FromMilliseconds(15);
            networkPoller.Tick += new EventHandler(PollNetwork);
            networkPoller.Start();
        }
        internal void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Messages.Text += "\n Sent button clicked";
        }

        internal void PollNetwork(object sender, EventArgs e)
        {
            activeClient.Poll();
        }

        public void AddMessage(string text)
        {
            Messages.Text += ("{}" + text);
        }
    }
}