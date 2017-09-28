//using NLog;
using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpencerGifts.MoveFiles
{
    class Program
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();


        static int Main(string[] args)
        {

            string source = string.Empty;
            string target = string.Empty;
            string filemask = string.Empty;
            string targetmask = string.Empty;
            bool timestamp = false;
            bool recursive = false;
            bool delsrc = false;

            //Prepare parameters
            try
            {

                foreach (string arg in args)
                {

                    int commandIndex = arg.IndexOf(":");
                    string command = arg.Substring(0, commandIndex).ToUpper();

                    switch (command)
                    {
                        case "SRC":
                            source = arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1);
                            break;
                        case "TARGET":
                            target = arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1);
                            break;
                        case "MASK":
                            filemask = arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1);
                            break;
                        case "TARGETMASK":
                            targetmask = arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1);
                            break;
                        case "TIMESTAMP":
                            timestamp = Convert.ToBoolean(arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1));
                            break;
                        case "RECURSIVE":
                            recursive = Convert.ToBoolean(arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1));
                            break;
                        case "DELSRC":
                            delsrc = Convert.ToBoolean(arg.Substring(commandIndex + 1, arg.Length - commandIndex - 1));
                            break;
                        default:
                            // do other stuff...
                            break;
                    }
                }
            }
            catch
            {
                //_logger.Debug(ex);
                //_logger.ErrorException("[" + DateTime.Now.ToString() + "]", ex);
            }

            //Start Moving Files
            try
            {

                // _logger.Info("Move Files Started");



                CopyDirectory(source, target, filemask, targetmask, timestamp, recursive, delsrc);


                //_logger.Info("Move Files Ended");

                return 1;
            }
            catch
            {
                //_logger.Info("Move Files Stoped");
                //_logger.ErrorException("Exception: ", ex);
                return 0;
            }


        }

        public static string AddFileVersion(int value)
        {
            string iKey = value.ToString("D3");
            return iKey;
        }


        public static string AddFileTimeStamp()
        {
            return DateTime.Now.ToString("yyyyddM-HHmmss");
        }

        public static void CopyDirectory(string source, string target, string filemask, string targetmask, bool timestamp = false, bool recursive = false, bool delsrc = true)
        {
            if (source == "" || target == "") return;

            var stack = new Stack<Folders>();

            stack.Push(new Folders(source, target));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                Directory.CreateDirectory(folders.Target);
                string[] masklist = filemask.Split(',');
                foreach (var mask in masklist)
                {
                    foreach (var file in Directory.GetFiles(folders.Source, mask))
                    {
                        string strTimeStamp = timestamp ? AddFileTimeStamp() : "";
                        string name = Path.GetFileNameWithoutExtension(file);
                        string extension = targetmask == "." ? "" : targetmask != "" ? targetmask : Path.GetExtension(file);
                        string newFileName = name + "-" + strTimeStamp + extension;
                        string fileName = Path.Combine(folders.Target, newFileName);
                        int versionId = 0;
                        //while (File.Exists(Path.Combine(folders.Target, fileName)))
                        //{
                        //string strVersion = AddFileVersion(versionId++);
                        //extension = targetmask != "" && targetmask != "." ? targetmask : extension;
                        //newFileName = name + "-" + strTimeStamp + "-" + strVersion + extension;
                        //fileName = Path.Combine(folders.Target, newFileName);
                        //}
                        File.Copy(file, fileName);

                        if (delsrc && File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                }

                if (recursive == true)
                {
                    foreach (var folder in Directory.GetDirectories(folders.Source))
                    {
                        stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                    }
                }
            }
        }

    }

    public class Folders
    {
        public string Source { get; private set; }
        public string Target { get; private set; }

        public Folders(string source, string target)
        {
            Source = source;
            Target = target;
        }
    }


}
