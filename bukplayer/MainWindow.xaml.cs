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
using System.ComponentModel;
using CSCore.CoreAudioAPI;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Interop;
using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;

namespace BukPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public enum PlayerAction { Play = 0, Pause = 1, Stop = 2, AddToPlayList=3};
    public delegate void PlayerActionRequest(object sender, PlayerAction playerAction);
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<MMDevice> devices = new ObservableCollection<MMDevice>();
        Player player;
        Library library;
        private int listIndex;
        private enum CurrentView {Artistview= 0,AlbumView= 1,SongView=2};
       
        CurrentView LibraryView = CurrentView.AlbumView;
        
        public MainWindow()
        {
            
            InitializeComponent();
            player = new Player();
            player.PlaybackStopped += Player_PlaybackStopped;
            player.PlaySecondElapsed += Player_PlaySecondElapsed1; ;
            listIndex = 0;
           
            
        }

        private void Player_PlaySecondElapsed1(object sender, EventArgs e)
        {
            if (slider.Value <=slider.Maximum)
            {
                slider.Value++;
            }
            /*else
                slider.Value = 1;*/
             Elapsed.Content = player.Position.Minutes + ":" + player.Position.Seconds;
            
        }

       

       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (
                    var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        devices.Add(device);
                    }
                }
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
          
            player.Dispose();
        }

      
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
            player.Dispose();
            this.Close();
        }

        private void AlbumViewButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "Search";
            LibraryElements.Children.Clear();
            LibraryView = CurrentView.AlbumView;
            foreach (Album a in library.Albums)
            {
                AlbumElement alb = new AlbumElement(a, playerServer);
                LibraryElements.Children.Add(alb);
            }
            SongViewButton.IsEnabled = true;
            AlbumViewButton.IsEnabled = false;
            ArtistViewButton.IsEnabled = true;


        }

        private void ArtistViewButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "Search";
            LibraryElements.Children.Clear();
            LibraryView = CurrentView.Artistview;
            foreach (Artist a in library.Artists)
            {
               ArtistElement artist = new ArtistElement(a,playerServer);
                LibraryElements.Children.Add(artist);
            }
            SongViewButton.IsEnabled = true;
            AlbumViewButton.IsEnabled = true;
            ArtistViewButton.IsEnabled = false;

        }

        private void LibraryScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (LibraryView == CurrentView.AlbumView)
            {
                if (LibraryElements.Children.Count > 0)
                {
                    foreach (AlbumElement album in LibraryElements.Children)
                    {
                        GeneralTransform childTransform = album.TransformToAncestor(this);
                        Rect rectangle = childTransform.TransformBounds(new Rect(new System.Windows.Point(0, 0), this.RenderSize));
                        Rect result = Rect.Intersect(new Rect(new System.Windows.Point(0, 0), this.RenderSize), rectangle);

                        if (result != Rect.Empty)
                        {

                            album.LoadVisuals();

                        }
                        else
                        {
                            album.UnLoadVisuals();
                        }
                    }
                }
            }
         

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LibraryView = CurrentView.AlbumView;

            if (File.Exists(Directory.GetCurrentDirectory() + "\\Library\\Library.xml"))
            {
                MessageBox.Show("Loading library from existing library file");
                library = Library.fromFile();
                MessageBox.Show("Library Succesfully loaded");
            }
                
            else
            {
                MessageBox.Show("Building library from directory");

                library = Library.FromPath("F:\\Music");
                library.serialize();
                MessageBox.Show("Library succesfully loaded");
                 /*try
                 {
                     library = Library.FromPath(Directory.GetCurrentDirectory() + "\\Music");
                     library.serialize();
                     MessageBox.Show("Library succesfully loaded");
                 }
                 catch (Exception ex)
                 {

                     MessageBox.Show("Music Directory not found");
                 }*/


            }

            AlbumViewButton_Click(sender, new RoutedEventArgs());

        }


        private void Player_PlaybackStopped(object sender, CSCore.SoundOut.PlaybackStoppedEventArgs e)
        {
            if (Playlist.Children.Count > 0 && Playlist.Children.Count >= listIndex + 1 && player.PlaybackState == CSCore.SoundOut.PlaybackState.Stopped)
            {
                PlylistElement p = (PlylistElement)Playlist.Children[listIndex];
                PlayerOpenAndUpdate(p.song, devices[0]);
                player.Play();
                listIndex += 1;
            }

        }

       

        private void playerServer(object obj, PlayerAction action)
        {
            if (obj is SongElement)
            {
                if (action == PlayerAction.Play)
                {
                    SongElement s = (SongElement)obj;
                    PlayerOpenAndUpdate(s.song, devices[0]);
                    player.Play();
                }

                if (action == PlayerAction.AddToPlayList)
                {
                    SongElement s = (SongElement)obj;
                    if(!Playlist.Children.Contains(s))
                    Playlist.Children.Add(new PlylistElement(s.song));
                }
            }
            if (obj is AlbumElement)
            {
                if (action == PlayerAction.Play)
                {
                    Playlist.Children.Clear();
                    AlbumElement a = (AlbumElement)obj;
                    foreach (Song s in a.album.songs)
                    {
                        Playlist.Children.Add(new PlylistElement(s));
                    }
                    listIndex = 1;
                    if (player.PlaybackState != CSCore.SoundOut.PlaybackState.Stopped)
                    {
                        player.Stop();
                    }
                    if (player.PlaybackState == CSCore.SoundOut.PlaybackState.Stopped)
                    {
                        PlylistElement p = (PlylistElement)Playlist.Children[0];
                        PlayerOpenAndUpdate(p.song, devices[0]);
                        player.Play();
                    }               
                }
            }

            if (obj is ArtistElement)
            {
                if (action == PlayerAction.Play)
                {
                    Playlist.Children.Clear();
                    ArtistElement a = (ArtistElement)obj;
                    foreach (Album album in a.artist.albums)
                    {
                        foreach (Song s in album.songs)
                        {
                            Playlist.Children.Add(new PlylistElement(s));
                        }
                        
                    }
                    listIndex = 1;
                    if (player.PlaybackState != CSCore.SoundOut.PlaybackState.Stopped)
                    {
                        player.Stop();
                    }
                    if (player.PlaybackState == CSCore.SoundOut.PlaybackState.Stopped)
                    {
                        PlylistElement p = (PlylistElement)Playlist.Children[0];
                        PlayerOpenAndUpdate(p.song, devices[0]);
                        player.Play();
                    }

                  




                }
            }
            if (obj is SongElementExt)
            {
                if (action == PlayerAction.Play)
                {
                    SongElementExt s = (SongElementExt)obj;
                    PlayerOpenAndUpdate(s.song, devices[0]);
                    player.Play();
                }

                if (action == PlayerAction.AddToPlayList)
                {
                    SongElementExt s = (SongElementExt)obj;

                    if (!Playlist.Children.Contains(s))
                        Playlist.Children.Add(new PlylistElement(s.song));
                }
            }

        }
        private void PlayerOpenAndUpdate(Song song, MMDevice device)
        {
           
            player.Open(song,device);           
            this.Time.Content =player.Length.Minutes+":"+player.Length.Seconds;

            slider.Maximum = player.Length.Minutes * 60 + player.Length.Seconds;
            slider.Minimum = 0;
            slider.Value = 0;

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.PlaybackState == CSCore.SoundOut.PlaybackState.Playing)
            {
                player.Pause();
                MessageBox.Show(player.PlaybackState.ToString());
            }

            else if (player.PlaybackState == CSCore.SoundOut.PlaybackState.Paused)
            {
                player.Play();
                MessageBox.Show(player.PlaybackState.ToString());
            }
            else if (player.PlaybackState == CSCore.SoundOut.PlaybackState.Stopped)
            {

                if (Playlist.Children.Count > 0)
                {
                    PlylistElement p = (PlylistElement) Playlist.Children[0];

                    PlayerOpenAndUpdate(p.song , devices[0]);
                    player.Play();
                    
                }

            
            }

            
        }

        private void SongViewButton_Click(object sender, RoutedEventArgs e)
        {

            textBox.Text = "Search";
            LibraryElements.Children.Clear();
            LibraryView = CurrentView.SongView;
            foreach (Song s in library.Songs)
            {
                SongElementExt song = new SongElementExt(s, playerServer);
                LibraryElements.Children.Add(song);
            }
            SongViewButton.IsEnabled = false;
            AlbumViewButton.IsEnabled = true;
            ArtistViewButton.IsEnabled = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
           
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (listIndex > 1)
            {
                listIndex -= 2;
                player.Stop();
            }
        }

        private void textBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textBox.Clear();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox.IsFocused)
            {

                if (LibraryView == CurrentView.Artistview && library != null)
                {
                    List<Artist> searchResult = new List<Artist>();
                    searchResult = library.searchArtists(textBox.Text);
                    LibraryElements.Children.Clear();
                    foreach (Artist a in searchResult)
                    {
                        LibraryElements.Children.Add(new ArtistElement(a, playerServer));
                    }

                }

                if (LibraryView == CurrentView.AlbumView && library != null)
                {
                    List<Album> searchResult = new List<Album>();
                    searchResult = library.searchAlbum(textBox.Text);
                    LibraryElements.Children.Clear();
                    foreach (Album a in searchResult)
                    {
                        LibraryElements.Children.Add(new AlbumElement(a, playerServer));
                    }

                }

                if (LibraryView == CurrentView.SongView && library != null)
                {
                    List<Song> searchResult = new List<Song>();
                    searchResult = library.searchSong(textBox.Text);
                    LibraryElements.Children.Clear();
                    foreach (Song a in searchResult)
                    {
                        LibraryElements.Children.Add(new SongElementExt(a, playerServer));
                    }

                }
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(textBox.Text== string.Empty)
            textBox.Text = "Search";
            
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
            
        }

        private void MinimizeButton_GotFocus(object sender, RoutedEventArgs e)
        {
            MinimizeButton.Background.Opacity = 0;
        }

        private void slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
          //  MessageBox.Show("hello");
        }

        private void slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("hello");
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
           // MessageBox.Show(e.Source.ToString());
        }

        private void slider_DragLeave(object sender, DragEventArgs e)
        {
            
        }

        private void slider_ManipulationCompleted(object sender, DragCompletedEventArgs e)
        {
         
        }

        private void slider_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
           
        }

        private void slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (player.PlaybackState == CSCore.SoundOut.PlaybackState.Playing)
            {
                double time = slider.Value;
                int minutes = (int)time / 60;
                int seconds = (int)time % 60;
                player.Position = new TimeSpan(0, minutes, seconds);
            }
        }
    }
    
}
