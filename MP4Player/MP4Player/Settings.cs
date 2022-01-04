using System.IO;
namespace MP4Player
{
    internal class Settings
    {
        static string path = @"C:\\Users\arnab\\Documents\\MP3Player\\MP3Player.set";
        static string playlist = @"C:\\Users\arnab\\Documents\\MP3Player\\MP3Player.pl";
        static void checkfile()
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory("C:\\Users\\arnab\\Documents\\MP3Player");
                using (FileStream fs = File.Create(path))
                {
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(67);
                    bw.Close();
                    fs.Close();
                }
            }
        }
        public static float volume()
        {
            float volume = 0;
            checkfile();
            if (File.Exists(path))
            {
                FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                int v = br.ReadInt32();
                volume = v ;
                br.Close();
                fs.Close();
            }
            return volume;
        }
        public static void Update_Volume(double n)
        {
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write((int)n);
            bw.Close();
            fs.Close();
        }
        static void playlistcheck()
        {
            if (!File.Exists(playlist))
            {
                Directory.CreateDirectory("C:\\Users\\arnab\\Documents\\MP3Player");
                using (FileStream fs = File.Create(playlist))
                {
                }
            }
        }
        static public int load_Playlist(string[] dtl)
        {
            int count = 0;
            playlistcheck();
            if (File.Exists(playlist))
            {
                StreamReader sr = new StreamReader(playlist);
                string str;
                while ((str = sr.ReadLine())!=null)
                {
                    dtl[count++] = str;
                }
                sr.Close();
            }
            return count;
        }
        static public void save_playlist(string[] s, int n)
        {
            if (File.Exists(playlist))
                File.Delete(playlist);
            playlistcheck();
            StreamWriter sw = new StreamWriter(playlist);
            for (int i = 0; i < n; i++)
            {
                sw.WriteLine(s[i]);
            }
            sw.Close();
        }
    }
}
