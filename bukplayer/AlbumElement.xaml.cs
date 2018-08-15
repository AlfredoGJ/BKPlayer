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
    /// Interaction logic for AlbumElement.xaml
    /// </summary>
    public partial class AlbumElement :UserControl
    {
        
        public Album album;
        PlayerActionRequest request;
        public AlbumElement(Album album, PlayerActionRequest r)
        
        {
           
            InitializeComponent();
            request += r;
            this.album = album;
            this.Title.Content = album.Name;
            this.Artist.Content = album.songs[0].Artist;
            foreach (Song s in album.songs)               
            {
                SongElement song = new SongElement(s);
                song.request += r;
                this.SongsList.Children.Add(song);
            }
            
        }

        public void LoadVisuals()
        {


            if (album.cover != null && album.cover != string.Empty)
            {
                this.AlbumCover.Source = new BitmapImage(new Uri(album.cover));

            }

          
           

            
        }

        public  void UnLoadVisuals()
        {
            if (this.AlbumCover.Source != null)
            {
            
            }
        }

        private void AlbumGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"/BukPlayer;component/Resources/albumPlaymine.png", UriKind.Relative));

            AlbumGrid.Children.Add(img);

        }

        private void AlbumGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AlbumGrid.Children.Count > 1)
            {
                AlbumGrid.Children.Remove(AlbumGrid.Children[2]);
            }

        }

        private void AlbumGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            request(this, PlayerAction.Play);
        }
    }
}
