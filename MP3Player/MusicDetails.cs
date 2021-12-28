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
    }
}
