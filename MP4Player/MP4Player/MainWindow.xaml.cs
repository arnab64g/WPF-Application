using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Win32;
namespace MP4Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int count = 0, index = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            VideoPanel.LoadedBehavior = MediaState.Manual;
            Browse_Button.Click += Browse_Button_Click;
            listBox.MouseDoubleClick += ListBox_MouseDoubleClick;
            Pause_Button.Click += Pause_Button_Click;
            Play_Button.Click += Play_Button_Click;
            Next_Button.Click += Next_Button_Click;
            Previous_Button.Click += Previous_Button_Click;
            Stop_Button.Click += Stop_Button_Click;
            seek.ValueChanged += Seek_ValueChanged;
            Volume.Maximum = 100;
            Volume.Value = 75;
            Volume.ValueChanged += Volume_ValueChanged;
        }
        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoPanel.Volume = Volume.Value/100;
        }
        private void Seek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoPanel.Position = TimeSpan.FromSeconds(seek.Value);   
        }
        private void Stop_Button_Click(object sender, EventArgs e)
        {
            VideoPanel.Stop();
            seek.Value = 0;
            VideoPanel.Position = TimeSpan.Zero;
        }
        private void Previous_Button_Click(object sender, RoutedEventArgs e)
        {
            VideoPanel.Stop();
            if(index != 0)
                index--;
            listBox.SelectedItem = listBox.Items[index];
            Play_Button_Click(sender, e);   
        }
        private void Next_Button_Click(object sender, EventArgs e)
        {
            VideoPanel.Stop();
            if (index < count - 1)
                index++;
            else
            {
                if (Repeat.IsChecked == true)
                    index = 0;
            }
            listBox.SelectedItem = listBox.Items[index];
            Play_Button_Click(sender, e);   
        }
        private void Play_Button_Click(object sender, EventArgs e)
        {
            if (VideoPanel.Position.TotalSeconds > 0)
            {
                VideoPanel.Play();
                return;
            }
            VideoPanel.Close();
            MusicDetails md;
            if (listBox.SelectedIndex == -1)
                md = (MusicDetails)listBox.Items.GetItemAt(index); 
            else
                md = (MusicDetails)listBox.SelectedItem;
            string? s = md.Path;
            if (s != null)
            {
                var file = TagLib.File.Create(s);
                VideoPanel.Source = new Uri(s);
                VideoPanel.Play();
                TDuration1.Content = file.Properties.Duration.ToString(@"mm\:ss");
                file.Dispose();
                timer.Start();
                timer.Tick += Timer_Tick;
                seek.Value = 0;
                seek.Maximum = file.Properties.Duration.TotalSeconds;
            }
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            Current_Time.Content = VideoPanel.Position.ToString(@"mm\:ss");
            seek.Value = VideoPanel.Position.TotalSeconds;
        }
        private void Pause_Button_Click(object sender, EventArgs e)
        {
            VideoPanel.Pause();
        }
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VideoPanel.Stop();
            Play_Button_Click(sender, e);
        }
        private void Browse_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "(*.mp4)|*.mp4";
            ofd.Title = "MP4Player";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                string[] s = ofd.FileNames;
                StringSep.LoadMusic(s, s.Length, listBox);
                count = listBox.Items.Count;       
            }
        }
    }
}
