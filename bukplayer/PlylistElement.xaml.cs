using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BukPlayer
{
    /// <summary>
    /// Interaction logic for PlylistElement.xaml
    /// </summary>
    public partial class PlylistElement : UserControl
    {
        public Song song;
        private bool isPlaying;
        public PlylistElement(Song song)
        {
            isPlaying = false;
            InitializeComponent();
            this.song = song;
            this.Title.Content = song.Title;
            this.Artist.Content = song.Artist;
            if (!string.IsNullOrEmpty(song.CoverPath))
                image.Source = new BitmapImage(new Uri(song.CoverPath));
            image.Visibility = Visibility.Visible;
           
        }

        public void setAsPlaying()
        {
            isPlaying = true;

        }
        public void setAsNoPlaying()
        {
            isPlaying = false;
        }
    }
}
