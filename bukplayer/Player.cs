using System;
using System.ComponentModel;
using CSCore;
using CSCore.Codecs;
using CSCore.Tags.ID3;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using System.Timers;
using System.Windows.Threading;
namespace BukPlayer
{
   
    public class Player : Component
    {
      
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;
        private DispatcherTimer timer;
        public Song currentPlayingSong { get; set; } 
        public Player()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,0,0,1000);
            timer.IsEnabled = true;
        }

        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;
        public event  EventHandler PlaySecondElapsed;

        public PlaybackState PlaybackState
        {
            get
            {
                if (_soundOut != null)
                    return _soundOut.PlaybackState;
                return PlaybackState.Stopped;
            }
        }

        public void nada()
        {
            
        }

        public ID3v2 getFileTag(string filename)

        {
            return ID3v2.FromFile(filename);
        }

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetPosition();
                return TimeSpan.Zero;
                           }
            set
            {
                if (_waveSource != null)
                    _waveSource.SetPosition(value);
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }

        public int Volume
        {
            get
            {
                if (_soundOut != null)
                    return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
                return 100;
            }
            set
            {
                if (_soundOut != null)
                {
                    _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                }
            }
        }

        public void Open(Song song, MMDevice device)
        {
            currentPlayingSong = song;
            CleanupPlayback();

            _waveSource = CodecFactory.Instance.GetCodec(song.Path)
                    .ToSampleSource()
                    .ToMono()
                    .ToWaveSource();
            _soundOut = new WasapiOut() { Latency = 100, Device = device };
            _soundOut.Initialize(_waveSource);
            if (PlaybackStopped != null) _soundOut.Stopped += PlaybackStopped;
            if (PlaySecondElapsed != null) { timer.Tick -= PlaySecondElapsed; timer.Tick += PlaySecondElapsed; }
        }

        public void Play()
        {
            if (_soundOut != null)
            {
                _soundOut.Play();
                timer.Start();
            }
               
        }

        public void Pause()
        {
            if (_soundOut != null)
            {
                _soundOut.Pause();
                timer.Stop();
            }
               

        }

        public void Stop()
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
                timer.Stop();
            }
                

        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
           
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupPlayback();
        }
    }
}
