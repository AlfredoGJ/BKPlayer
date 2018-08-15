using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace BukPlayer
{
    class Utils
    {
        //method taken from http://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        //published by user Gerret
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
            catch (Exception e)
            {
                return new BitmapImage();
            }
        }

      
    }
}
