using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace theChat.Models
{
    [Serializable]
    public class Conversation
    {
       
        public string StartDate { get; set; }
        public string PeerName { get; set; }

        public string IpAddress { get; set; } //test

        public ObservableCollection<NetworkMessage> ConversationMessages { get; set; }

        public NetworkMessage StatusInformation { get; set; }


        public Conversation(string startDate, string peerName, IPAddress ipaddress)
        {

            StartDate = startDate;
            PeerName = peerName;
            ConversationMessages = new ObservableCollection<NetworkMessage>();
            IpAddress = ipaddress.ToString(); //test

           
        }

        public Conversation()
        {
            ConversationMessages = new ObservableCollection<NetworkMessage>();

        }


    }
}
