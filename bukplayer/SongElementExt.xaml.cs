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
    /// Interaction logic for SongElement.xaml
    /// </summary>
    public partial class SongElementExt : UserControl
    {

        public PlayerActionRequest request;
        public Song song;
        public SongElementExt(Song s, PlayerActionRequest r )
        {

            InitializeComponent();
            request += r;
            this.Album.Content = s.Album;
            this.song = s;
            this.Title.Content = s.Title;
            this.Artist.Content = s.Artist;
            this.track.Content = s.trackNumber;
            image.Visibility = Visibility.Hidden;
            image2.Visibility = Visibility.Hidden;
        }

        public void LoadVisuals()
        {

        }

        public void UnLoadVisuals()
        {

        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = new SolidColorBrush(Color.FromArgb(255, 32, 32, 33));


            image.Visibility = Visibility.Visible;
            image2.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            image.Visibility = Visibility.Hidden;
            image2.Visibility = Visibility.Hidden;
            this.Background = new SolidColorBrush(Color.FromArgb(255, 27, 27, 28));

        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                request(this, PlayerAction.Play);
            }
        }

        private void image2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                request(this, PlayerAction.AddToPlayList);
            }
        }
    }
}
