using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using theChat.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.IO;
using System.Text;
using System.Linq;

using System.Collections.ObjectModel;

using System.Windows.Forms;
using System.Drawing;
using static theChat.Models.MySocket;
using theChat.utils;

namespace theChat.Viewmodels
{

    public class ChatViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region constructor

        public ChatViewModel()
        {
            _Socket = new MySocket();
            MyInputValidator = new InputValidator();


            // commands

            AcceptRequestCommand = new RelayCommand(AcceptRequest, param => true);
            DeclineRequestCommand = new RelayCommand(DeclineRequest, param => true);
            ShowChatCommand = new RelayCommand(ShowChat, param => true);
            RemoveImageCommand = new RelayCommand(RemoveImage, param => true);
            SendChatRequestCommand = new RelayCommand(SendChatRequest, param => true);
            SendChatMessageCommand = new RelayCommand(SendChatMessage, param => true);
            StartListeningCommand = new RelayCommand(StartListening, param => true);
            AttachImageCommand = new RelayCommand(AttachImage, param => true);
            ShowHistoryCommand = new RelayCommand(ShowHistory, param => true);
            SendBuzzCommand = new RelayCommand(SendBuzz, param => true);
            LeaveChatCommand = new RelayCommand(LeaveChat, param => true);
            StopListeningCommand = new RelayCommand(StopListening, param => true);

            _UnreadNotices = null;
            Peers = new ObservableCollection<Peer>();
            Chats = new Dictionary<Peer, Conversation>();
            CurrentHistory = new ObservableCollection<Conversation>();
            CurrentConversation = new ObservableCollection<NetworkMessage>();

            ChatInformation = "";
            ChatWindowHeader = "Chat Window";
            _OpenedChat = null;

            _Socket.PropertyChanged += Socket_PropertyChanged;
        }

        #endregion

        #region member variables private 

        private MySocket _Socket;

        private Peer _OpenedChat;

        private String _ChatInformation;

        private String _ChatWindowHeader;

        private String _Username;

        private String _UserPortNumber;

        private string? _PeerIPAddress;

        private string? _PeerPortNumber;

        private String _ChatMessage;

        private String _CurrentImage;

        private bool _IsListening;

        private String _HistorySearchString;

        private bool _IsImageSelected;

        private int? _UnreadNotices;

        private bool _ConversationVisibility;

        private bool _MessagingControlsVisibility;

        private bool _LeaveChatButtonVisibility;

        private bool _ChatInformationVisibility;

        private bool _NewRequestBtnVisibility;

        #endregion

        #region member variables public

        // First public versions of some private variables and then variables that only have public versions, for example ICommands.

        public String ChatInformation
        {
            get
            {
                return _ChatInformation;
            }

            private set
            {
                _ChatInformation = value;
                NotifyPropertyChanged();
            }
        }


        public String ChatWindowHeader
        {
            get
            {
                return _ChatWindowHeader;
            }

            private set
            {
                _ChatWindowHeader = value;
                NotifyPropertyChanged();
            }
        }


        public String Username
        {
            get
            {
                return _Username;
            }

            set
            {

                if (value != _Username)
                {
                    MyInputValidator.ValidateUsername(value);
                    _Username = value;
                }

            }
        }

        public String UserPortNumber
        {
            get
            {
                return _UserPortNumber;
            }

            set
            {
                if (value != _UserPortNumber)
                {
                    MyInputValidator.ValidatePortNumber(value, "UserPortNumber");
                    _UserPortNumber = value;
                }

            }
        }


        public string PeerIPAddress
        {
            get
            {
                return _PeerIPAddress;
            }

            set
            {

                if (value != _PeerIPAddress)
                {

                    if (value != null)
                    {
                        MyInputValidator.ValidateIPAddress(value);
                    }

                    _PeerIPAddress = value;

                    NotifyPropertyChanged();
                }

            }
        }


        public string PeerPortNumber
        {
            get
            {
                return _PeerPortNumber;
            }

            set
            {

                if (value != _PeerPortNumber)
                {

                    if (value != null)
                    {
                        MyInputValidator.ValidatePortNumber(value, "PeerPortNumber");
                    }

                    _PeerPortNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public String ChatMessage
        {
            get
            {
                return _ChatMessage;
            }

            set
            {
                _ChatMessage = value;
                NotifyPropertyChanged();
            }
        }

        public String CurrentImage
        {
            get
            {
                return _CurrentImage;
            }

            set
            {
                _CurrentImage = value;
                NotifyPropertyChanged();
            }
        }


        public bool IsListening
        {

            get
            {

                return _IsListening;
            }

            private set
            {
                if (value != _IsListening)
                {
                    _IsListening = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public String HistorySearchString
        {
            get
            {
                return _HistorySearchString;
            }

            set
            {
                if (value != _HistorySearchString)
                {
                    _HistorySearchString = value;
                    GetHistory();
                }
            }
        }


        public bool IsImageSelected
        {
            get
            {

                return _IsImageSelected;
            }

            private set
            {

                if (value != _IsImageSelected)
                {

                    _IsImageSelected = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public int? UnreadNotices
        {
            get
            {
                return _UnreadNotices;
            }
            private set
            {
           
                if (value == null)
                {
                    _UnreadNotices = 1;
                }
                else if (value == 0)
                {
                    _UnreadNotices = null;
                }
                else
                {
                    _UnreadNotices = value;
                }

                NotifyPropertyChanged();

            }
        }

        public bool NewRequestBtnVisibility
        {

            get
            {
                return _NewRequestBtnVisibility;
            }

            private set
            {
                if (value != _NewRequestBtnVisibility)
                {
                    _NewRequestBtnVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public bool ConversationVisibility
        {

            get
            {
                return _ConversationVisibility;
            }

            private set
            {
                if (value != _ConversationVisibility)
                {
                    _ConversationVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool MessagingControlsVisibility
        {

            get
            {
                return _MessagingControlsVisibility;
            }

            private set
            {
                if (value != _MessagingControlsVisibility)
                {
                    _MessagingControlsVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool ChatInformationVisibility
        {

            get
            {
                return _ChatInformationVisibility;
            }

            private set
            {
                if (value != _ChatInformationVisibility)
                {
                    _ChatInformationVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool LeaveChatButtonVisibility
        {

            get
            {
                return _LeaveChatButtonVisibility;
            }

            private set
            {
                if (value != _LeaveChatButtonVisibility)
                {
                    _LeaveChatButtonVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public InputValidator MyInputValidator { get; private set; }

        public Dictionary<Peer, Conversation> Chats { get; private set; }

        public ObservableCollection<Peer> Peers { get; private set; }

        public ObservableCollection<Conversation> CurrentHistory { get; private set; }

        public ObservableCollection<NetworkMessage> CurrentConversation { get; private set; }

        public ICommand AcceptRequestCommand { get; private set; }

        public ICommand DeclineRequestCommand { get; private set; }

        public ICommand ShowChatCommand { get; private set; }

        public ICommand RemoveImageCommand { get; private set; }

        public ICommand ShowHistoryCommand { get; private set; }

        public ICommand AttachImageCommand { get; private set; }

        public ICommand LeaveChatCommand { get; private set; }

        public ICommand SendChatMessageCommand { get; private set; }

        public ICommand SendChatRequestCommand { get; private set; }

        public ICommand StartListeningCommand { get; private set; }

        public ICommand StopListeningCommand { get; private set; }

        public ICommand SendBuzzCommand { get; private set; }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        }

        private void Socket_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {


            if (e.PropertyName == "IsListening")
            {
                IsListening = _Socket.IsListening;

            }

            else if(e.PropertyName == "CurrentException")
            {
                if(_Socket.CurrentException is PeerOfflineException)
                {
                    string caption = "Peer offline";

                    string message = "Your peer appears to be offline. It's therefore not possible to send messages to them";

                    MessageBoxButton buttonOk = MessageBoxButton.OK;

                    System.Windows.MessageBox.Show(message, caption, buttonOk);
                }
                else if(_Socket.CurrentException is UserOfflineException)
                {

                    string caption = "Network connection";

                    string message = "You appear to be offline, check your network connection";

                    MessageBoxButton buttonOk = MessageBoxButton.OK;

                    System.Windows.MessageBox.Show(message, caption, buttonOk);

                }
                else if(_Socket.CurrentException is ListeningException)
                {

                    string caption = "Error";

                    string message = "An error occured when trying to listen for chat request, please check your internet connection and try again.";

                    MessageBoxButton buttonOk = MessageBoxButton.OK;

                    System.Windows.MessageBox.Show(message, caption, buttonOk);

                }
                else
                {
                    ShowUnknowErrorDialog();
                }
            }

            else if (e.PropertyName == "PeerLatestReceived")
            {
                Peer peer = _Socket.PeerLatestReceived;

                ConnectionInfo connectionInfo = _Socket.Connections[peer];


                switch (connectionInfo.LatestMessage.Type)
                {
                    case NetworkMessage.MessageType.Accept:

                        // remove 'pending peer' and add the 'real peer'

                        Chats[peer] = new Conversation(DateTime.Now.ToString(), peer.Name, peer.IPAddress);

                        Chats.Remove(connectionInfo.PendingPeer);

                        // add new peer in 'Peers' at same position as pending peer

                        int index = Peers.IndexOf(connectionInfo.PendingPeer);

                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Peers[index] = peer;

                            NotifyPropertyChanged("Peers");

                        });


                        if (connectionInfo.PendingPeer == _OpenedChat)
                        {

                            CurrentConversation = new ObservableCollection<NetworkMessage>();

                            NotifyPropertyChanged("CurrentConversation");

                            _OpenedChat = peer;

                            ShowChatConversationWindow();
                        }
                        else
                        {
                            UpdateNotices(peer);
                        }

                        break;

                    case NetworkMessage.MessageType.Decline:

                        connectionInfo.LatestMessage.Content = "Peer with IP " + peer.IPAddress + " has declined your chat request :(";

                        Chats[peer].StatusInformation = connectionInfo.LatestMessage;

                        Peers[Peers.IndexOf(peer)].Status = "Declined :(";

                        if (peer == _OpenedChat)
                        {

                            ChatInformation = connectionInfo.LatestMessage.Content;


                        }
                        else
                        {
                            UpdateNotices(peer);
                        }


                        _Socket.CloseConnection(peer);

                        break;


                    case NetworkMessage.MessageType.LeaveChat:

                        connectionInfo.LatestMessage.Content = "Peer with IP " + peer.IPAddress + " has left the chat :(";

                        Chats[peer].StatusInformation = connectionInfo.LatestMessage;

                        Peers[Peers.IndexOf(peer)].Status = "Has left chat :(";

                        NotifyPropertyChanged("Peers");

                        if (peer == _OpenedChat)
                        {

                            ChatInformation = connectionInfo.LatestMessage.Content;
                            ConversationVisibility = false;
                            ChatInformationVisibility = true;
                            MessagingControlsVisibility = false;

                        }
                        else
                        {
                       
                            UpdateNotices(peer);
                        }

                        _Socket.CloseConnection(peer);

                        break;

                    case NetworkMessage.MessageType.Request:

                        Chats[peer] = new Conversation(DateTime.Now.ToString(), peer.Name, peer.IPAddress);

                        Chats[peer].StatusInformation = connectionInfo.LatestMessage;

                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Peers.Add(peer);
                        });

                        UpdateNotices(peer);

                        NotifyPropertyChanged("Peers");

                        break;

                    case NetworkMessage.MessageType.Message:


                        if (connectionInfo.LatestMessage.ImageByteAarray != null)
                        {

                            ImageConverter.SaveImage(connectionInfo.LatestMessage.ImageByteAarray, connectionInfo.LatestMessage.ImageFilename);

                            connectionInfo.LatestMessage.TheImage = ImageConverter.FileToImage(connectionInfo.LatestMessage.ImageFilename);
                        }

                        connectionInfo.LatestMessage.IsUser = false;

                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Chats[peer].ConversationMessages.Add(connectionInfo.LatestMessage);
                        });


                        if (peer == _OpenedChat)
                        {

                            CurrentConversation = Chats[peer].ConversationMessages;

                            NotifyPropertyChanged("CurrentConversation");

                        }
                        else
                        {
                            UpdateNotices(peer);

                        }
                        break;

                    case NetworkMessage.MessageType.Buzz:



                        if (peer != _OpenedChat)
                        {

                            index = Peers.IndexOf(peer);

                            Peers[index].IsBuzzing = true;

                            NotifyPropertyChanged("Peers");

                        }


                        Buzz();

                        break;
                }
            }
        }

        #endregion

        #region IDataErrorInfo


        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (MyInputValidator.Errors.ContainsKey(propertyName))
                {

                    return MyInputValidator.Errors[propertyName];
                }

                return null;
            }
        }


        #endregion

        #region Communication Commands


        private void StartListening(object obj)
        {
            new Thread(() =>
            {

                try
                {
                    _Socket.StartListeningToPort(Int32.Parse(UserPortNumber));
                }
                catch (Exception e)
                {

                    ShowUnknowErrorDialog();
                }

            }).Start();
        }


        private void StopListening(object obj)
        {
            _Socket.StopListening();
        }


        private void SendChatRequest(object obj)
        {

            IPAddress Ip = IPAddress.Parse(PeerIPAddress);

            int portNr = Int32.Parse(PeerPortNumber);


            Peer pendingPeer = new Peer(Ip, null, portNr, "Sending chat request...");

            NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.Request,
                "Sending chat request to peer with IP " + pendingPeer.IPAddress + "...", DateTime.Now.ToString(), Username, false);


            string jsonString = JsonSerializer.Serialize(msg);


            jsonString += "<EOF>";


            new Thread(() =>
            {
                try
                {
                    _Socket.SendConnectionRequest(pendingPeer, jsonString);

                    Chats.Add(pendingPeer, new Conversation());

                    Chats[pendingPeer].StatusInformation = msg;

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Peers.Add(pendingPeer);

                        NotifyPropertyChanged("Peers");
                    });


                    ShowSentRequestDialog();
                    
                }
                catch (PeerNotListeningException peerNotListeningException)
                {

                    
                    string caption = "Error";

                    MessageBoxButton buttonOk = MessageBoxButton.OK;

                    // Show message box
                    System.Windows.MessageBox.Show(peerNotListeningException.Message, caption, buttonOk);

                }
                catch (Exception e)
                {
                    
                    ShowUnknowErrorDialog();

                }
            }).Start();


            PeerIPAddress = null;

            PeerPortNumber = null;

        }


        private void AcceptRequest(object obj)
        {

            Peer peer = _OpenedChat;

            // makes sure that status is still Request and that other peer has not left the chat already before sending accept.

            if (Chats[peer].StatusInformation.Type == NetworkMessage.MessageType.Request)
            {

                NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.Accept, Username, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), Username, false);

                string jsonString = JsonSerializer.Serialize(msg);

                jsonString += "<EOF>";

                _Socket.Send(jsonString, peer);

                Chats[peer] = new Conversation(DateTime.Now.ToString(), peer.Name, peer.IPAddress);

                Peers[Peers.IndexOf(peer)].Status = null;

                ShowChatConversationWindow();

            }
            else
            {

              
                ChatInformation = "Too slow! " + peer.Name + " at IP address " + peer.IPAddress + " already left the chat :(";

                LeaveChatButtonVisibility = true;

                NewRequestBtnVisibility = false;

                
            }

        }


        private void DeclineRequest(object obj)
        {
            Peer peer = _OpenedChat;

            // only send decline message if the other peer is still waiting for response

            if (Chats[peer].StatusInformation.Type == NetworkMessage.MessageType.Request)
            {

                NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.Decline, "", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), Username, false);

                string jsonString = JsonSerializer.Serialize(msg);

                jsonString += "<EOF>";

                _Socket.Send(jsonString, peer);

            }

            _Socket.CloseConnection(peer);

            RemovePeer(peer);

            ClearChatWindow();
        }


        private void LeaveChat(object obj)
        {
            SendLeaveChatMessage(_OpenedChat);

            SaveChatConversation(_OpenedChat);

            RemovePeer(_OpenedChat);

            ClearChatWindow();
        }


        private void SendBuzz(object obj)
        {

            NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.Buzz, "BUZZ!", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), Username, false);

            string jsonString = JsonSerializer.Serialize(msg);

            jsonString += "<EOF>";

            _Socket.Send(jsonString, _OpenedChat);         
        }


        private void SendChatMessage(object obj)
        {

            NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.Message, ChatMessage, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), Username, true);

            if (IsImageSelected)
            {

                Image image = Image.FromFile(CurrentImage);
             
                msg.ImageByteAarray = ImageConverter.FileToByteArray(CurrentImage);

                msg.TheImage = image;
              
                string imageName = Username + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");

                msg.ImageFilename = imageName + "." + new ImageFormatConverter().ConvertToString(msg.TheImage.RawFormat).ToLower();

                RemoveImage(null);

            }

            Chats[_OpenedChat].ConversationMessages.Add(msg);

            CurrentConversation = Chats[_OpenedChat].ConversationMessages;

            NotifyPropertyChanged("CurrentConversation");

            ChatMessage = "";

            string jsonString = JsonSerializer.Serialize(msg);

            jsonString += "<EOF>";

            _Socket.Send(jsonString, _OpenedChat);
        }


        private void ShowHistory(object obj)
        {
            if (obj != null)
            {
                ConversationVisibility = true;

                Conversation conversation = (Conversation)obj;

                ObservableCollection<NetworkMessage> newConversationMessages = new ObservableCollection<NetworkMessage>();


                foreach (NetworkMessage message in conversation.ConversationMessages)
                {
                    NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.Message, message.Content, message.Date, message.Sender, message.IsUser);

                    if (message.ImageByteAarray != null)
                    {

                        Image image = ImageConverter.ByteArrayToImage(message.ImageByteAarray);

                        msg.TheImage = image;
                    }

                    newConversationMessages.Add(msg);

                }

                ShowChatHistoryConversationWindow(conversation, newConversationMessages);
            }

        }


        private void ShowChat(Object obj)
        {
            if (obj != null)
            {
                Peer peer = (Peer)obj;

                // removes attached image/text if the chatwindow is changed to another peer

                if(peer != _OpenedChat)
                {
                    ChatMessage = "";

                    RemoveImage(null);

                    _OpenedChat = peer;
                }
                else
                {
                    return;
                }


                // clears potential buzzes or notices

                if (peer.IsBuzzing)
                {
                    peer.IsBuzzing = false;

                }



                if (peer.HasUnreadNotice)
                {

                    UnreadNotices -= peer.NrUnreadNotices;

                    peer.HasUnreadNotice = false;

                    Peers[Peers.IndexOf(peer)].NrUnreadNotices = 0;
                }


                // shows appropriate chatwindow depending on if the conversation is started or if it is pending


                if (Chats[peer].StatusInformation != null)
                {
                  
                    ShowChatStatusWindow();

                }
                else
                {
                    
                    ShowChatConversationWindow();
                }
                

            }
        }


        private void AttachImage(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "C:\\";

            openFileDialog.Filter = "image files (*.BMP; *.JPG; *.GIF; *.PNG) | *.BMP; *JPG; *.GIF; *.PNG|All files (*.*)| *.*";

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                IsImageSelected = true;

                CurrentImage = openFileDialog.FileName;

            }

        }


        private void RemoveImage(object obj)
        {
            CurrentImage = null;

            IsImageSelected = false;
          
        }



        private void SendLeaveChatMessage(Peer peer)
        {
          

            if (Chats[peer].StatusInformation == null || Chats[peer].StatusInformation.Type == NetworkMessage.MessageType.Request)
            {

                NetworkMessage msg = new NetworkMessage(NetworkMessage.MessageType.LeaveChat, Username, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), Username, false);

                string jsonString = JsonSerializer.Serialize(msg);

                jsonString += "<EOF>";

                _Socket.Send(jsonString, peer);

                _Socket.CloseConnection(peer);

            }

        }


        private void SaveChatConversation(Peer peer)
        {
          
            if (Chats[peer].ConversationMessages.Count > 0)
            {

                string jsonString = JsonSerializer.Serialize(Chats[peer]);

                string convoName = peer.Name + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");

                string chatHistoryPath = "..\\..\\..\\chatHistory\\" + convoName + ".json";

                try
                {
                    using (FileStream fS = File.Create(chatHistoryPath))
                    {
                        byte[] convo = new UTF8Encoding(true).GetBytes(jsonString);

                        fS.Write(convo, 0, convo.Length);
                    }

                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }


                CurrentHistory.Insert(0, Chats[peer]);

                NotifyPropertyChanged("CurrentHistory");
            }
        }


        #endregion


        #region Communication (not command methods)

        // called from MainWindow 
        public void BeforeClosing()
        {
            if (Peers.Count > 0)
            {

                foreach (Peer peer in Peers)
                {
                    SendLeaveChatMessage(peer);

                    SaveChatConversation(peer);
                }

            }

            if (IsListening)
            {
                StopListening(null);
            }
        }


        private void GetHistory()
        {
            // searches through the chatHistory folder based on filename and the search string

            string chatHistoryFolder = "..\\..\\..\\chatHistory\\";

            DirectoryInfo dir = Directory.CreateDirectory(chatHistoryFolder);


                //new DirectoryInfo(chatHistoryFolder);
            
            IEnumerable<FileInfo> fileList = dir.GetFiles();

            IEnumerable<FileInfo> fileQuery;

            if (_HistorySearchString == "")
            {
                fileQuery =
                from file in fileList
                where file.Extension == ".json"
                orderby file.CreationTime descending
                select file;

            }
            else
            {
                fileQuery =
                from file in fileList
                where file.Extension == ".json"
                where file.Name.StartsWith(_HistorySearchString)
                orderby file.CreationTime descending
                select file;
            }


            CurrentHistory.Clear();

            // converts file to Conversation object 

            foreach (FileInfo fi in fileQuery)
            {

                string s = "";

                StreamReader sr = fi.OpenText();

                s = sr.ReadToEnd();

                Conversation convo = JsonSerializer.Deserialize<Conversation>(s);

                CurrentHistory.Add(convo);

            }

            NotifyPropertyChanged("CurrentHistory");

        }


        private void ShowUnknowErrorDialog()
        {

            string caption = "Error";

            string message = "Unknown error occured, please check your internet connection and try again.";

            MessageBoxButton buttonOk = MessageBoxButton.OK;

            System.Windows.MessageBox.Show(message, caption, buttonOk);

        }


        private void ShowSentRequestDialog()
        {

            string caption = "Sent";

            string message = "The chat request has been sent";

            MessageBoxButton buttonOk = MessageBoxButton.OK;

            System.Windows.MessageBox.Show(message, caption, buttonOk);

        }



        private void Buzz()
        {
            
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                Window mainWindow = System.Windows.Application.Current.MainWindow;

                for (int i = 0; i < 10; ++i)
                {
                    
                    mainWindow.Left += 5;

                    Thread.Sleep(50);

                    mainWindow.Left -= 10;

                    Thread.Sleep(50);

                    mainWindow.Left += 5;
                }
            }));
        }


        private void RemovePeer(Peer peer)
        {
            Peers.Remove(peer);

            Chats.Remove(peer);
        }


        private void ClearChatWindow()
        {

            ChatWindowHeader = "Chat Window";

            _OpenedChat = null;

            LeaveChatButtonVisibility = false;

            MessagingControlsVisibility = false;

            ConversationVisibility = false;

            ChatInformationVisibility = false;

            NewRequestBtnVisibility = false;

        }


        private void ShowChatConversationWindow()
        {

            ChatWindowHeader = "Chatting with " + _OpenedChat.Name;

            CurrentConversation = Chats[_OpenedChat].ConversationMessages;

            ChatInformation = "";

            LeaveChatButtonVisibility = true;

            MessagingControlsVisibility = true;

            ConversationVisibility = true;

            ChatInformationVisibility = false;

            NewRequestBtnVisibility = false;

            NotifyPropertyChanged("CurrentConversation");

        }

        private void ShowChatStatusWindow()
        {

            // shows status of a pending/received chat request

            ChatWindowHeader = "Chatting with " + _OpenedChat.IPAddress;

            ChatInformation = Chats[_OpenedChat].StatusInformation.Content;

            ChatInformationVisibility = true;

            MessagingControlsVisibility = false;

            ConversationVisibility = false;

            NewRequestBtnVisibility = false;

            LeaveChatButtonVisibility = true;

            // Show two buttons to accept or decline a request

            if (Chats[_OpenedChat].StatusInformation.Type == NetworkMessage.MessageType.Request && Chats[_OpenedChat].StatusInformation.Sender == _OpenedChat.Name)
            {
                
                NewRequestBtnVisibility = true;

                LeaveChatButtonVisibility = false;

                ChatInformation = "Do you want to chat with " + _OpenedChat.Name + " at IP address " + _OpenedChat.IPAddress + "?";
            }
        }

        private void ShowChatHistoryConversationWindow(Conversation conversation, ObservableCollection<NetworkMessage> conversationMessages)
        {
            _OpenedChat = null;

            RemoveImage(null);

            ChatWindowHeader = "Old conversation with " + conversation.PeerName + " at IP Address " + conversation.IpAddress;

            CurrentConversation = conversationMessages;

            ConversationVisibility = true;

            MessagingControlsVisibility = false;
          
            ChatInformationVisibility = false;

            LeaveChatButtonVisibility = false;

            NotifyPropertyChanged("CurrentConversation");

        }

        private void UpdateNotices(Peer peer)
        {
            Peers[Peers.IndexOf(peer)].HasUnreadNotice = true;

            ++Peers[Peers.IndexOf(peer)].NrUnreadNotices;

            ++UnreadNotices;
        }


        #endregion

    }


}
