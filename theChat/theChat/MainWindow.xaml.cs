
using System;
using System.Windows;
using System.Windows.Controls;

using theChat.Models;
using theChat.Viewmodels;


namespace theChat
{
    public partial class MainWindow 
    {
        public ChatViewModel _ChatViewModel { get; set; }

     

        public MainWindow()
        {
         
            InitializeComponent();
        }

     

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChatViewModel chatViewModel = (ChatViewModel)DataContext;
            chatViewModel.BeforeClosing();
        }

      
    }
}
