using System;

namespace SimpleLogger
{
    public class Logger
    {
        //////////////// IT IS VERY(!) SIMPLE LOGGER (don't rate it please) ////////////////
        private string CRLF = "\n";
        private bool _debug;
        public Messages Msg = new Messages();

        public class Messages
        {
            public string InvalidString = "Некорректная строка:\n";
            public string FileNotFound = "Файл не был найден: \n";
        }
        
        public Logger(bool Debug)
        {
            _debug = Debug;
        }

        public void Info(string s)
        {
            if (_debug)
            {
                Console.WriteLine("Info: " + s + CRLF);
            }
        }

        public void Debug(string s)
        {
            if (_debug)
            {
                Console.WriteLine("Debug: " + s + CRLF);
            }
        }

        public void Warn(string s)
        {
            if (_debug)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Warn: " + s + CRLF);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Error(Exception ex)
        {
            if (_debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error: " + ex.Message + CRLF + ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Error(string s, Exception ex)
        {
            if (_debug)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error: " + s + CRLF + ex.Message + CRLF + ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Fatal(Exception ex)
        {
            if (_debug)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Fatal: " + ex.Message + CRLF + ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Fatal(string s, Exception ex)
        {
            if (_debug)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Fatal: " + s + CRLF + ex.Message + CRLF + ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}