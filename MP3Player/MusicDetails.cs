using System.Collections.Generic;
using System;
using NAudio.Wave;
using System.Windows.Controls;
namespace MP3Player
{
    public class MusicDetails
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public string Path { get; set; }
    }
    public class StringSep
    {
        public static string MusicTitle(string s)
        {
            string val = "";
            char[] ch = s.ToCharArray();
            int len = ch.Length;
            int l = 0;
            for (int j = len-1; j >= 0; j--)
            {
                if (ch[j] == '\\')
                    break;
                else
                {
                    l++;
                    val += ch[j];
                }
            }
            string emp = "";
            for (int i = l - 1 ; i >= 0; i--)
            {
                emp += val[i];
            }
            return emp;
        }
        public static void LoadMusic(string[] st, int n, ListView mainWindow)
        {
            Mp3FileReader wfr;
            List<MusicDetails> list = new List<MusicDetails>();
            for (int i = 0; i < n; i++)
            {
                wfr = new Mp3FileReader(st[i]);
                TimeSpan ts = wfr.TotalTime;
                list.Add(new MusicDetails() { Title = StringSep.MusicTitle(st[i]), Duration = ts.ToString(@"mm\:ss"), Path = st[i] });
            }
            mainWindow.ItemsSource = list;
        }
    }
}
