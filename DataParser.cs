using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using SimpleLogger;

namespace TestServiceCloud
{
    public class DataParser
    {
        private Logger log = new Logger(Program.debug);

        //split separators:
        static readonly char[] Equally = { '=' };
        static readonly char[] Semicolon = { ';' };
        static readonly char[] Colon = { ':' };
        static readonly char[] Quote = { '\"' };

        private string header = string.Empty;
        private bool isPreviousHeader = false;

        public List<ConnectionString> GetAllData(List<string> paths)
        {
            List<ConnectionString> allData = new List<ConnectionString>();
            List<ConnectionString> data = new List<ConnectionString>();
            foreach (string path in paths)
            {
                allData.AddRange(GetData(path));
            }
            return allData;
        }

        private List<ConnectionString> GetData(string path)
        {
            string line;
            int counter = 0;
            FileStream file = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            List<ConnectionString> data = new List<ConnectionString>();

            while ((line = reader.ReadLine()) != null)
            {
                var connString = ParseFileLine(line);
                if (connString != null)
                    data.Add(connString);
                //data.Add(ParseFileLine(line));
                counter++;
            }
            file.Close();
            return data;
        }

        private ConnectionString ParseFileLine(string line)
        {
            bool isConnectionString = (Regex.IsMatch(line, Regexes.ConnectFileString) || Regex.IsMatch(line, Regexes.ConnectServerString));
            if (Regex.IsMatch(line, Regexes.HeaderString))
            {
                header = line;
                isPreviousHeader = true;
            }
            if (isConnectionString)
            {
                ConnectionString data = new ConnectionString();
                var parsed = ParseData(line, header);
                data = CreateConnectionString(parsed);
                if (isPreviousHeader)
                {
                    isPreviousHeader = false;
                    header = String.Empty;
                }
                else
                {
                    log.Warn("Строка подключения: " + line + "\nне имеет заголовка и будет записана в bad_data.txt");
                }
                return data;
            }
            return null;
        }

        private ConnectionString CreateConnectionString(Dictionary<string, string> parsedData)
        {
            ConnectionString data = new ConnectionString();
            parsedData.TryGetValue("Header", out data.Header);
            parsedData.TryGetValue("File", out data.Source);
            parsedData.TryGetValue("File", out data.Path);
            parsedData.TryGetValue("Srvr", out data.Host);
            parsedData.TryGetValue("Port", out data.Port);
            parsedData.TryGetValue("Ref", out data.Reference);
            parsedData.TryGetValue("Error", out data.Error);
            return data;
        }

        private Dictionary<string, string> ParseData(string line, string header)
        {
            Dictionary<string, string> parsedData = new Dictionary<string, string>();

            if (header != "")
            {
                parsedData.Add("Header", header);
            }
            else
            {
                parsedData.Add("Error", "True");
            }
            parsedData.Add("Source", line);

            try //TODO: этому методу требуется рефакторинг
            {
                string[] Connect = line.Split(Equally, 2, StringSplitOptions.RemoveEmptyEntries);
                string[] SrvrRfrn = Connect[1].Split(Semicolon, 2, StringSplitOptions.RemoveEmptyEntries);
                string[] Reference = { };
                bool isLocalFile = SrvrRfrn.Length == 1;
                if (isLocalFile)
                {
                    string[] file = SrvrRfrn[0].Split(Equally, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (file[0] == "File")
                    {
                        parsedData.Add("File", file[1].Replace("\"", ""));
                    }
                }
                else //it's remote file
                {
                    Reference = SrvrRfrn[1].Split(Equally, 2, StringSplitOptions.RemoveEmptyEntries);
                    string[] RefName = Reference[1].Split(Quote, 2, StringSplitOptions.RemoveEmptyEntries);
                    string[] name = RefName[0].Split(Semicolon, 1, StringSplitOptions.RemoveEmptyEntries);
                    parsedData.Add("Ref", name[0]);
                    string[] Srvr = SrvrRfrn[0].Split(Equally, 2, StringSplitOptions.RemoveEmptyEntries);
                    string[] Address = Srvr[1].Split(Equally, 2, StringSplitOptions.RemoveEmptyEntries);

                    string[] HostPort = Address[0].Split(Colon, 2, StringSplitOptions.RemoveEmptyEntries);
                    bool hasPort = HostPort.Length > 1;
                    if (hasPort)
                    {
                        parsedData.Add("Srvr", HostPort[0].Replace("\"", ""));
                        parsedData.Add("Port", HostPort[1].Replace("\"", ""));
                    }
                    else //remote path haven't port
                    {
                        parsedData.Add("Srvr", Address[0].Replace("\"", ""));
                    }
                }
            }
            catch
            {
                parsedData.Add("Error", "True");
                log.Warn("Невалидная строка: " + line + "\nбудет записана в bad_data.txt");
            }
            return parsedData;
        }
    }
}