using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Collections;

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


            /* test cases */
            //string path = "E:/test/The.Flash.(2014).S01E17.SDTV.mp4";
            //string path = "E:/test/";
            //string path = "E:/test/avi.mkv.welcome.pdf";
            //string path = args[0].ToString(); //path sent in arguments from cmd line
            //string path = null;
            
            
            if(null_empty(args) && !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + "/SubIt.bat"))
            {
                create_file();
                Environment.Exit(1);
            }

            else if(!null_empty(args))
            {
                string path = args[0].ToString();

                if(Directory.Exists(path))
                {
                    /* if the input is the media folder */
                    string extensions = "*.avi;*.mpeg;*.m4v;*.mkv;*.mp4;*.mpg;*.mov;*.rm;*.vob;*.flv;*.3gp";//media extension files to be scanned

                    ArrayList files_returned = get_files(path, extensions); //search for media files

                    for(int i = 0; i < files_returned.Count; i++)
                    {
                        sub_return(path + "/" + files_returned[i].ToString());
                    }
                }
                else
                {
                    sub_return(path); // if input is only 1 video file.
                }
            }
        }

        static string hash_compute(string file_path) //compute MD5 hash of first and last 64kb of the video file
        {
            try
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.TargetSite);
                return "err";
            }
        }


        static void sub_return(string file_path)
        {
            string[] extensions = new string[] { ".avi", ".mpeg", ".m4v", ".mkv", ".mp4", ".mpg", ".mov", ".rm", ".vob", ".flv", ".3gp" }; //video file extension support
            int count = 0;
            string rem_exten = null;

            for(int i = 0; i < extensions.Length; i++)
            {
                if(file_path.EndsWith(extensions[i]))
                {
                    rem_exten = file_path.Replace(extensions[i], "");
                    count = 1;
                    break;
                }
            }


            if(count == 1 && !File.Exists(rem_exten + ".en.srt") && !File.Exists(rem_exten + ".srt"))
            {
                string final_hash = hash_compute(file_path); //compute hash function

                if(!final_hash.Equals("err"))
                {
                    using(var client = new WebClient())
                    {
                        client.Headers.Add("user-agent", "SubDB/1.0 (SubIt/0.9; https://github.com/iAmMrinal0/SubIt)"); //required header where subdb/1.0 needs to be intact
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
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Some program is accessing the file. Please close that program and try again.");
                    Console.ReadLine();
                }

            }
        }

        static ArrayList get_files(string folder, string search)
        {
            string[] extensions = search.Split(new char[] { ';' });


            ArrayList files = new ArrayList();
            DirectoryInfo file_det = new DirectoryInfo(folder);

            foreach(string ext in extensions)
            {
                files.AddRange(file_det.GetFiles(ext));
            }

            return files;
        }

        static void create_file()
        {
            FileStream fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.SendTo) + "/SubIt.bat");

            using(StreamWriter write_stream = new StreamWriter(fs)) //write stream
            {
                write_stream.WriteLine("@echo off");
                write_stream.WriteLine("cls");
                write_stream.WriteLine(":top");
                write_stream.WriteLine("IF %1==\"\" GOTO start");
                write_stream.WriteLine("  C:\\SubIt.exe %1");
                write_stream.WriteLine("  SHIFT");
                write_stream.WriteLine("  GOTO top");
                write_stream.WriteLine(":start");
            }
        }

        static bool null_empty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }
    }
}