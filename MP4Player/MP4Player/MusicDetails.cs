using System.Collections.Generic;
using System.Windows.Controls;
namespace MP4Player
{
    public class MusicDetails
    {
        public string? Title { get; set; }
        public string? Path { get; set; }
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
        public static void LoadMusic(string[] st, int n, ListBox mainWindow)
        {
           
            List<MusicDetails> list = new List<MusicDetails>();
            for (int i = 0; i < n; i++)
            {
                list.Add(new MusicDetails() { Title = StringSep.MusicTitle(st[i]), Path = st[i] });
            }
            mainWindow.ItemsSource = list;
        }
    }
}
