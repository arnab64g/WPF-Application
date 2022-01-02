using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using NAudio.Wave;
using Microsoft.Win32;
using System.Windows.Threading;
namespace MP3Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private Mp3FileReader wfr;
        private WaveOut wo = new WaveOut();
        private readonly MMDeviceEnumerator mmde = new MMDeviceEnumerator();
        private readonly MMDevice mmd;
        private int index = 0, count = 0;
        private byte cl = 0, cr = 0;
        private float l = 0, r = 0;
        public MainWindow()
        {
            InitializeComponent();
            mmd = mmde.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)[0];
            Browse_Button.Click += Browse_Button_Click;
            Play_Button.Click += Play_Button_Click;
            Stop_Button.Click += Stop_Button_Click;
            Pause_Button.Click += Pause_Button_Click;
            Music_list.MouseDoubleClick += Music_list_MouseDoubleClick;
            Previous_Button.Click += Previous_Button_Click;
            Next_Button.Click += Next_Button_Click;
            seek.ValueChanged +=Seek_ValueChanged;
            volume.Value = Settings.volume();
            wo.Volume = (float)volume.Value / 100;
            vol.Content = ((int)volume.Value).ToString() + "%";
            volume.ValueChanged += Volume_ValueChanged;
            string[] s = new string[100];
            count = Settings.load_Playlist(s);
            StringSep.LoadMusic(s, count, Music_list);
            count = Music_list.Items.Count;
            Settings.save_playlist(s, count);
        }
        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            wo.Volume = (float)(volume.Value) / 100;
            Settings.Update_Volume(wo.Volume * 100);
            vol.Content = ((int)volume.Value).ToString() + "%";
        }
        private void Seek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            wfr.Position = (long)(seek.Value* wfr.WaveFormat.AverageBytesPerSecond);
        }
        private void Next_Button_Click(object sender, EventArgs e)
        {
            if (index < count - 1)
            {
                index++;
                Play_Button_Click(sender, e);
            }
            else if(repeat.IsChecked == true)
            {
                index = 0;
                Play_Button_Click(sender, e);
            }
        }
        private void Previous_Button_Click(object sender, EventArgs e)
        {
            if (index != 0)
            {
                index--;
                Play_Button_Click(sender, e);
            }
        }
        private void Music_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (wo.PlaybackState != PlaybackState.Stopped)
            {
                Stop_Button_Click(sender, e);
            }
            index = Music_list.SelectedIndex;
            Play_Button_Click(sender, e);
        }
        private void Pause_Button_Click(object sender, EventArgs e)
        {
            wo.Pause();
        }
        private void Stop_Button_Click(object sender, EventArgs e)
        {
            wo.Stop();
            wo.Dispose();
            wfr.Dispose();
            dispatcherTimer.Stop();
            npl.Content = "";
            currenttime.Visibility = Visibility.Collapsed;
        }
        private void Play_Button_Click(object sender, EventArgs e)
        {
            if (wo.PlaybackState == PlaybackState.Paused)
            {
                wo.Resume();
                return;
            }
            MusicDetails fil = (MusicDetails)Music_list.Items.GetItemAt(index);
            string str = fil.Path;
            if (wo.PlaybackState == PlaybackState.Playing)
            {
                wo.Stop();
                wfr.Dispose();
                wo.Dispose();
            }
            wfr = new Mp3FileReader(str);
            wo = new WaveOut();
            seek.Value = 0;
            seek.Maximum = wfr.TotalTime.TotalSeconds;
            wo.Init(wfr);
            wo.Play();
            var f = TagLib.File.Create(str);
            var pic = f.GetTag(TagLib.TagTypes.Id3v2);
            npl.Content = fil.Title;
            if (pic.Pictures.Length > 0)
            {
                var picture = pic.Pictures[0];
                var mem = new MemoryStream(picture.Data.Data);
                var bmp = new BitmapImage();   
                bmp.BeginInit();
                bmp.StreamSource = mem;
                bmp.EndInit();
                albumart_Copy.Source = bmp;
            }
            else albumart_Copy.Source = null;
            
            currenttime.Visibility = Visibility.Visible;
            dispatcherTimer.Start();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(25);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (wo.PlaybackState == PlaybackState.Stopped)
            {
                Next_Button_Click(sender, e);
            }
            seek.Value = wfr.CurrentTime.TotalSeconds;
            currenttime.Content = wfr.CurrentTime.ToString(@"mm\:ss");
            remaintime.Content = (wfr.TotalTime - wfr.CurrentTime).ToString(@"mm\:ss");
            l = mmd.AudioMeterInformation.PeakValues[0];
            r =  mmd.AudioMeterInformation.PeakValues[1];
            left.Value = l * 100;
            right.Value = r *100;
            cl = (byte)(l*225);
            cr = (byte)(r*255);
            left.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(cl, 0, (byte)(255 - cl)));
            right.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(cr, 0, (byte)(255 - cr)));
        }
        private void Browse_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "MP3Player";
            ofd.Filter =    "(*.mp3)|*.mp3;";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                string[] s = ofd.FileNames;
                StringSep.LoadMusic(s, s.Length, Music_list);
                count = Music_list.Items.Count;
                Settings.save_playlist(s, count);
            }
        }
    }
}
