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
        client.Client activeClient;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
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

            activeClient = new client.Client();
            activeClient.Init();
            activeClient.messageWrite = AddMessage;

            DispatcherTimer.Run(PollNetwork, TimeSpan.FromMilliseconds(50), DispatcherPriority.Normal);
        }
        internal void SendButton_Click(object sender, RoutedEventArgs e)
        {
        }

        internal bool PollNetwork()
        {
            activeClient.Poll();
            return true;
        }

        public void AddMessage(string text)
        {
            Messages.Text += ($"{text}\n");
        }
    }
}