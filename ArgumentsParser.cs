using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleLogger;

namespace TestServiceCloud
{
    static class ArgumentsParser
    {
        public static List<string> GetFilesPaths(string[] args)
        {
            Logger log = new Logger(Program.debug);
            List<string> paths = new List<string>();
            for (int i = 1; i < args.Length; i++) //1 потому, что args[0] всегда имя программы
            {
                if (isCorectPath(args[i]))
                {
                    paths.Add(@args[i]);
                }
                else
                {
                    log.Warn(log.Msg.FileNotFound + args[i]);
                }
            }
            return paths;
        }

        public static bool isCorectPath(string path)
        {
            return File.Exists(path) ? true : false;
        }
    }
}
