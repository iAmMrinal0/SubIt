using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mime;

namespace Subtitle_Downloader
{
    class Program
    {
        static void Main(string[] args)
        {

            // Work in progress for subtitles in addic7ed.com.


            /*string path = "e:/tv shows/the blacklist/season 2/The.Blacklist.S02E14.HDTV.x264.LOL";
            string[] split1 = path.Split('/');
            string final_name = split1[split1.Length - 1];
            var regex = new Regex(@"[^sS0-9eE0-9]");
            var reg=new Regex("([sS][0-9]{2}[eE][0-9]{2})");
            Match text;
            if(System.Text.RegularExpressions.Regex.IsMatch(final_name, "([sS][0-9]{2}[eE][0-9]{2})"))
            {
                text = Regex.Match(final_name, reg.ToString());
                Console.WriteLine(text.Value);
            }
            
            Console.WriteLine(final_name);
            Console.ReadLine();*/

            /*
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("http://addic7ed.com/serie/");*/

            //string path = "E:/TV Shows/The Flash 2014/Season 1/The.Flash.2014.S01E02.WEB-DL.x264.AAC.mp4"; //test case




            string path = args[0].ToString(); //path sent in arguments from cmd line

            string[] extensions = new string[] { ".avi", ".mpeg", ".m4v", ".mkv", ".mp4", ".mpg", ".mov", ".rm", ".vob", ".flv", ".3gp" }; //video file extension support

            string rem_exten = null;
            int count = 0;

            //remove extensions and exit program if not a video file
            for(int i = 0; i < extensions.Length; i++)
            {
                if(path.EndsWith(extensions[i]))
                {
                    rem_exten = path.Replace(extensions[i], "");
                    count = 1;
                    break;
                }
            }

            if(count == 1)
            {
                string final_hash = hash_compute(path); //compute hash function

                using(var client = new WebClient())
                {
                    client.Headers.Add("user-agent", "SubDB/1.0 (SubIt/0.5; http://github.com)"); //required header where subdb/1.0 needs to be intact
                    string URL = "http://api.thesubdb.com/?action=download&language=en&hash=" + final_hash; //URL with hash and language of subtitle required

                    try
                    {
                        Stream stream = client.OpenRead(URL);

                        StreamReader read_stream = new StreamReader(stream); //response stream


                        FileStream fs = File.Create(rem_exten + ".en.srt"); //create new file with same file name with appended extension where ".en" is for "English"
                        string line = "";

                        using(StreamWriter write_stream = new StreamWriter(fs)) //write stream
                        {

                            while((line = read_stream.ReadLine()) != null)
                            {
                                write_stream.WriteLine(line);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Subtitle not found.");
                        Console.Read();
                    }
                }
            }
        }


        static string hash_compute(string file_path) //compute MD5 hash of first and last 64kb of the video file
        {
            using(BinaryReader b = new BinaryReader(File.Open(file_path, FileMode.Open)))
            {

                int required = 64 * 1024;


                b.BaseStream.Seek(0, SeekOrigin.Begin);

                byte[] by = b.ReadBytes(required);

                b.BaseStream.Seek(-required, SeekOrigin.End);

                byte[] bye = b.ReadBytes(required);

                byte[] final = new byte[by.Length + bye.Length];

                by.CopyTo(final, 0);
                Array.Copy(bye, 0, final, by.Length, bye.Length);

                MD5CryptoServiceProvider a = new MD5CryptoServiceProvider();
                byte[] result = a.ComputeHash(final);

                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}