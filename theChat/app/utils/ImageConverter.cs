using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theChat
{
    public static class ImageConverter
    {

        public static byte[] FileToByteArray(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }


        public static Image ByteArrayToImage(byte[] imageByteArray)
        {

            MemoryStream ms = new MemoryStream();

            for (int i = 0; i < imageByteArray.Length; ++i)
            {
                ms.WriteByte(imageByteArray[i]);
            }

            return Image.FromStream(ms);

        }


        public static void SaveImage(byte[] imageByteArray, string filename)
        {
            Directory.CreateDirectory("..\\..\\..\\images\\");

            string imagePath = "..\\..\\..\\images\\" + filename;

            File.WriteAllBytes(imagePath, imageByteArray);
        }


        public static Image FileToImage(string filename)
        {

            string imagePath = "..\\..\\..\\images\\" + filename;

            return Image.FromFile(imagePath);
        }

    }
}
