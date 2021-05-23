using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using theChat.ui;

namespace theChat.utils
{
    class chatWindowHeight : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("Nu ska den få en höjd!!!");
            if(value != null)
            {
                MyListBox myListBox = (MyListBox)value;

                string elementName = myListBox.Name;

                switch (elementName)
                {
                    case "ChatsBox":
                    case "HistoryBox":
                        return SystemParameters.FullPrimaryScreenHeight * 0.6;
                    case "chat":
                       return SystemParameters.FullPrimaryScreenHeight * 0.48;
                    case "historyConversation":
                        return SystemParameters.FullPrimaryScreenHeight * 0.8;


                }
              

               // return ((int)value) * 0.6;
            }


            return SystemParameters.FullPrimaryScreenHeight * 0.6; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
