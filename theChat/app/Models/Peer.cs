using System.Net;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace theChat.Models
{
    public class Peer : INotifyPropertyChanged
    {

        public IPAddress IPAddress { get; set; }

        private string _Name;

        private string _Status;

        private bool _HasUnreadNotice; 

        public int PortNumber { get; set; }

        private bool _IsBuzzing;

        public event PropertyChangedEventHandler PropertyChanged;

        public int NrUnreadNotices;


        public bool HasUnreadNotice
        {

            get
            {

                return _HasUnreadNotice;

            }

            set
            {

                if (value != _HasUnreadNotice)
                {
                    _HasUnreadNotice = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {

            get
            {

                return _Name;

            }

            set
            {

                if (value != _Name)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Status
        {

            get
            {

                return _Status;

            }

            set
            {

                if(value != _Status)
                {
                    _Status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsBuzzing
        {
            get
            {
                return _IsBuzzing;
            }

            set
            {
                _IsBuzzing = value;
                NotifyPropertyChanged();
            }
        }

        public Peer(IPAddress ipAddress, string name, int portNumber, string status)
        {
            IPAddress = ipAddress;
            Name = name;
            PortNumber = portNumber;
            IsBuzzing = false;
            HasUnreadNotice = false;
            Status = status;
            NrUnreadNotices = 0;
            
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        }


        public void resetDisplayVariables()
        {
            Status = null;
            NrUnreadNotices = 0;
            HasUnreadNotice = false;
            IsBuzzing = false;
        }
    }

}
