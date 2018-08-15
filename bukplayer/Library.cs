using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.Tags.ID3;
using CSCore.Tags.ID3.Frames;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.Xml.Serialization;



namespace BukPlayer
{
   
    public class Artist
    {
        public Artist()
        { }
        public Artist(string name)
        {
            this.Name = name;
            this. albums = new List<Album>();
            this.Banner = "";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Artist  a = (Artist)obj;
            return (Name == a.Name);

        }
        public string Banner;
        public string Name;
        public  List<Album> albums;
    }
    
    public class Album
    {
        public int AlbumId;
        public string cover;
        public string Name;
        public List<Song> songs;

        public Album()
        { }
        public Album(string name)
        {
            this.Name = name;
            songs = new List<Song>();
            cover = "";
            AlbumId = 0;
          
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Album a = (Album)obj;
            return (Name == a.Name);

        }
      

    
       
    }
   
    public class Song
    {
        public Song()
        {
            
        }
        public  TimeSpan Duration;
        public string Title;
        public string Path;
        public string Album;
        public string Artist;
        public int trackNumber;
        public string Genre;
        public int Year;
        public string CoverPath;
       
      
        public override string ToString()
        {
            return Title;
        }
        public void setAlbum(string  coverPath)
        {
            CoverPath = coverPath;
        }
       


    }

    [Serializable]
    public class Library
    {
        private int AlbumIDCounter = 0;
        public  List<Artist> Artists;
        public List<Album> Albums;
        public List<Song> Songs;

        public Library()
        {
            Artists = new List<Artist>();
            Albums = new List<Album>();
            Songs = new List<Song>();
        }
        

        public static Library FromPath(string path)
        {
            Library lib = new Library();

            string[] directories;
            directories=lib.getDirectoriesAddSongs(path);

            foreach (string dir in directories)
            {   
               
                    string[] dirAux;
                    dirAux = lib.getDirectoriesAddSongs(dir);
                    foreach (string dir2 in dirAux)
                    {
                        string[] dirAux2;
                        dirAux2 = lib.getDirectoriesAddSongs(dir2);
                    }
           }


            foreach (Artist a in lib.Artists)
            {
                lib.Albums.AddRange(a.albums);
                           
            }
            foreach (Album a in lib.Albums)
            {
               
                foreach(Song s in a.songs)
                {
                    s.setAlbum(a.cover);
                    lib.Songs.Add(s);
                }

            }

            lib.Artists.Sort((p, q) => string.Compare(p.Name,q.Name));
            lib.Albums.Sort((p, q) => string.Compare(p.Name, q.Name));
            lib.Songs.Sort((p, q) => string.Compare(p.Title, q.Title));
            return lib;

        }



        private  string[] getDirectoriesAddSongs(string path)
        {
          
            string[] directories;
            string[] files;

            try
            {
                directories = Directory.GetDirectories(path);
                files = Directory.GetFiles(path, "*.mp3");
            }
            catch (Exception e)
            {
                directories = new string[0];
                files = new string[0];
                throw e;
            }


            foreach (string s in files)
            {
                try
                {
                    addToLibrary(getSongInfo(s));
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Error en la lectura de archivos Ruta nula o no valida\n"+"Archivo: "+s);
                   
                }
            }

            return directories;
           
        }

        public void serialize(Library lib)
        {
            FileStream stream = new FileStream(Directory.GetCurrentDirectory() + "\\Library\\Library.xml", FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(lib.GetType());
            serializer.Serialize(stream, lib);
            stream.Close();
        }

        public static Library fromFile()
        {
            Library lib = new Library();
            FileStream stream = new FileStream(Directory.GetCurrentDirectory() + "\\Library\\Library.xml", FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(lib.GetType());
            lib= (Library)serializer.Deserialize(stream);
            stream.Close();
            return lib;

        }

        private  void addToLibrary(Song song)
        {
            ID3v2 songTag = ID3v2.FromFile(song.Path);
            Album album = new Album(song.Album);
            Artist artist = new Artist(song.Artist);
         

            if (Artists.Count == 0)
            {
                    album.songs.Add(song);
                    album.AlbumId = AlbumIDCounter;
                    AlbumIDCounter++;
                    

                if (songTag.QuickInfo.Image != null)
                {

                    album.cover = Directory.GetCurrentDirectory() + "/Library/AlbumThumbs/" + album.AlbumId+".jpg";
                    createAlbumThumb(song, album.AlbumId, songTag);
                  
                }

                //album.artist = artist;
                //song.album = album;

                artist.albums.Add(album);
                createArtistBanner(song, songTag, artist);
                Artists.Add(artist);


            }
            else
            {

                if (!Artists.Contains(artist))
                {

                    album.AlbumId = AlbumIDCounter;
                    AlbumIDCounter++;
                    album.songs.Add(song);

                    if (songTag.QuickInfo.Image != null)
                    {
                        album.cover = Directory.GetCurrentDirectory() + "/Library/AlbumThumbs/" + album.AlbumId+".jpg";
                        createAlbumThumb(song, album.AlbumId,songTag);
                 
                    }


                   // album.artist = artist;
                    //song.album = album;

                    artist.albums.Add(album);
                    createArtistBanner(song, songTag, artist);
                    Artists.Add(artist);

                }
                else
                {
                    if (!Artists[Artists.IndexOf(artist)].albums.Contains(album))
                    {
                        album.AlbumId = AlbumIDCounter;
                        AlbumIDCounter++;
                        album.songs.Add(song);
                        if (songTag.QuickInfo.Image != null)
                        {

                            album.cover = Directory.GetCurrentDirectory() + "/Library/AlbumThumbs/" + album.AlbumId+".jpg";
                            createAlbumThumb(song, album.AlbumId, songTag);
                      
                        }
                        

                       // album.artist = artist;
                        //song.album = album;

                        Artists[Artists.IndexOf(artist)].albums.Add(album);

                    }

                    else
                    {
                      //  album.artist = artist;
                        //song.album = album;

                        List<Album> lAlbumAux= Artists[Artists.IndexOf(artist)].albums;
                        Artists[Artists.IndexOf(artist)].albums[lAlbumAux.IndexOf(album)].songs.Add(song);
                    }
                }
            }

          
        }

       


        private  Song getSongInfo(string s)
        {
           
            Song song = new Song();
            ID3v2 tag = ID3v2.FromFile(s);
            if (tag != null)
            {


                foreach (Frame f in tag)
                {

                    if (f.FrameId == "TRCK")
                    {
                        TextFrame t = (TextFrame)f;
                        int.TryParse(t.Text, out song.trackNumber);
                    }

                    if (f.FrameId == "TCON")
                    {
                        TextFrame t = (TextFrame)f;
                        if (t.Text != null && t.Text != string.Empty)
                            song.Genre = t.Text;
                        else
                            song.Genre = "Unknown";
                    }
                    if (f.FrameId == "TLEN")
                    {
                        TextFrame t = (TextFrame)f;
                        if (!string.IsNullOrEmpty(t.Text))
                        {
                            song.Duration = TimeSpan.Parse(t.Text);
                        }
                    }
                    

                }

                
                song.Path = s;
             

                if (tag.QuickInfo.Title != null && tag.QuickInfo.Title != string.Empty)
                    song.Title = tag.QuickInfo.Title;
                else
                    song.Title = "Unknown";

                if (tag.QuickInfo.Album != null)
                    song.Album = tag.QuickInfo.Album;
                else
                    song.Album = "Unknown";

                if (tag.QuickInfo.Artist != null && tag.QuickInfo.Artist != string.Empty)
                    song.Artist = tag.QuickInfo.Artist;
                else
                {
                    if (tag.QuickInfo.LeadPerformers != null && tag.QuickInfo.LeadPerformers != string.Empty)
                        song.Artist = tag.QuickInfo.LeadPerformers;
                    else
                        song.Artist = "Unknown";
                }

                if (tag.QuickInfo.Year != null)
                    song.Year = (int)tag.QuickInfo.Year;
                else
                    song.Year = 0;
            }
            
            return song;
      }

        private void createAlbumThumb(Song song ,int albumId, ID3v2 Tag)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Library\\AlbumThumbs"))
            {
                try
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Library\\AlbumThumbs");
                }

                catch (Exception e)
                {

                }

            }
           
            Tag.QuickInfo.Image.GetThumbnailImage(150, 150, null, IntPtr.Zero).Save(Directory.GetCurrentDirectory() + "\\Library\\AlbumThumbs\\" + albumId + ".jpg");
         

        }

        private void createArtistBanner(Song song, ID3v2 tag, Artist artist)
        {

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Library\\ArtistBanners"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Library\\ArtistBanners");
               
            }
           
                foreach (Frame f in tag)
                {
                    if (f.FrameId == "APIC")
                    {
                        PictureFrame pic = (PictureFrame)f;
                        if (pic.Format == PictureFormat.Artist)
                        {
                            pic.Image.Save(Directory.GetCurrentDirectory() + "\\Library\\ArtistBanners\\" + song.Artist + ".jpg");
                          
                        }
                    }
                }
            
        }


        public void serialize()
        {
            FileStream stream = new FileStream(Directory.GetCurrentDirectory() + "\\Library\\Library.xml", FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stream, this);
            stream.Close();
        }


        public List<Artist> searchArtists(string searchString)
        {
            List<Artist> searchResult = new List<Artist>();
            foreach (Artist a in Artists)
            {
                if (a.Name.StartsWith(searchString,StringComparison.InvariantCultureIgnoreCase))
                {
                    searchResult.Add(a);
                }
            }

            return searchResult;
        }

        public List<Album> searchAlbum(string searchString)
        {
            List<Album> searchResult = new List<Album>();
            foreach (Album a in Albums)
            {
                if (a.Name.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))
                {
                    searchResult.Add(a);
                }
            }

            return searchResult;
        }

        public List<Song> searchSong(string searchString)
        {
            List<Song> searchResult = new List<Song>();
            foreach (Song a in Songs)
            {
                if (a.Title.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))
                {
                    searchResult.Add(a);
                }
            }

            return searchResult;
        }








    }
}
