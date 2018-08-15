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
    /// Interaction logic for ArtistElement.xaml
    /// </summary>
    public partial class ArtistElement : UserControl
    {
        public PlayerActionRequest request;
        public Artist artist;
        public ArtistElement(Artist artist, PlayerActionRequest r)
        {
            
            InitializeComponent();
            request += r;
            this.artist = artist;
            ArtistLabel.Content= artist.Name;
            this.Albums.Content = artist.albums.Count;
            int n=0;
            foreach (Album a in artist.albums)
            {
                foreach (Song s in a.songs)
                {
                    n++;
                }
            }
            this.Songs.Content = n;


        }

        private void image_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {

            Image img= new  Image();
            img.Source = new BitmapImage(new Uri(@"/BukPlayer;component/Resources/albumPlaymine.png",UriKind.Relative));

            ArtistGrid.Children.Add(img);
        }

        private void ArtistGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ArtistGrid.Children.Count > 1)
            {
                ArtistGrid.Children.Remove(ArtistGrid.Children[2]);
            }
        }

        private void ArtistGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                request(this, PlayerAction.Play);
            }
        }
    }
}
