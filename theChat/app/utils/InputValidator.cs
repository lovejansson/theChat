using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace theChat.utils
{
   public class InputValidator
    {


        public Dictionary<string, string> Errors { get; private set; } =
           new Dictionary<string, string>();

        public void ValidateUsername(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                AddError("Username", "Required field");
            }
            else if (name.Length > 16)
            {
                AddError("Username", "Max length of username is 16 characters");
            }
            else
            {
                RemoveError("Username");
            }

        }


        public void ValidatePortNumber(string port, string propertyName)
        {
            Regex rgxDigit = new Regex(@"\d");
            if (string.IsNullOrWhiteSpace(port))
            {
                AddError(propertyName, "Required field");
            }

            else if (!rgxDigit.IsMatch(port))
            {
                AddError(propertyName, "Port number must be a number between 1024 and 65535");

            }
            else if (Int32.Parse(port) < 1024 || Int32.Parse(port) > 65535)
            {
                AddError(propertyName, "Port number must be a number between 1024 and 65535");

            }
            else
            {
                RemoveError(propertyName);
            }
        }


        public void ValidateIPAddress(string ip)
        {
            Console.WriteLine("checking is valid ip");

            if (string.IsNullOrWhiteSpace(ip))
            {
                AddError("PeerIPAddress", "Required field");
            }
            else
            {
                try
                {
                    IPAddress.Parse(ip);

                    RemoveError("PeerIPAddress");

                }
                catch (FormatException formatException)
                {
                    AddError("PeerIPAddress", "Invalid IP address");

                }

            }
        }


        private void AddError(string propertyName, string error)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors[propertyName] = error;

            }
            else
            {
                Errors.Add(propertyName, error);

            }
        }


        private void RemoveError(string propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);

            }
        }

    }

}
