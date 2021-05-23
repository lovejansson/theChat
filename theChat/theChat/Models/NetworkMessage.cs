using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace theChat.Models
{
    public class NetworkMessage
    {
        public string Content { get; set; }

        public MessageType Type{get; set;}

        public string Date { get; set; }

        public string Sender { get; set; }

        [JsonIgnore]
        public Image TheImage { get; set; }

        public string ImageFilename { get; set; }

        public byte[] ImageByteAarray { get; set; }


        public bool IsUser { get;  set; } // wether message is sent by current user or receiving peer different colors are used in conversation!


        public NetworkMessage(MessageType type, string content, string date, string sender, bool isUser)
        {
            Type = type;
            Content = content;
            Date = date;
            Sender = sender;
            TheImage = null;
            ImageByteAarray = null;
            IsUser = isUser;
        }

        

        public enum MessageType
        {
            Message, 
            Buzz,
            Decline,
            Accept,
            Request,
            LeaveChat,
            Error,
        }
       
    }

}
