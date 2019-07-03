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
        static readonly char[] equally = { '=' };
        static readonly char[] semicolon = { ';' };
        static readonly char[] colon = { ':' };
        static readonly char[] quote = { '\"' };

        string header = string.Empty;
        bool isPreviousHeader = false;

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
                data.Add(ParseFileLine(line));
                counter++;
            }
            file.Close();
            return data;
        }

        private ConnectionString ParseFileLine(string line)
        {
            ConnectionString data = new ConnectionString();
            bool isConnectionString = (Regex.IsMatch(line, Regexes.ConnectFileString) || Regex.IsMatch(line, Regexes.ConnectServerString));
            if (Regex.IsMatch(line, Regexes.HeaderString))
            {
                header = line;
                isPreviousHeader = true;
            }
            if (isConnectionString)
            {
                if (isPreviousHeader)
                {
                    var parsed = ParseData(line, header);
                    data = CreateConnectionString(parsed);
                    isPreviousHeader = false;
                    header = String.Empty;
                }
                else
                {
                    var parsed = ParseData(line, header, true);
                    var created = CreateConnectionString(parsed);
                    data = CreateConnectionString(parsed);
                    log.Warn("Строка подключения: " + line + "\nне имеет заголовка и будет записана в bad_data.txt");
                }
            }
            return data;
        }

        private ConnectionString CreateConnectionString(Dictionary<string, string> parsedData)
        {
            ConnectionString data = new ConnectionString();
            if (parsedData.ContainsKey("Header"))
                parsedData.TryGetValue("Header", out data.Header);
            if (parsedData.ContainsKey("Source"))
                parsedData.TryGetValue("File", out data.Source);
            if (parsedData.ContainsKey("File"))
                parsedData.TryGetValue("File", out data.Path);
            if (parsedData.ContainsKey("Srvr"))
                parsedData.TryGetValue("Srvr", out data.Host);
            if (parsedData.ContainsKey("Port"))
                parsedData.TryGetValue("Port", out data.Port);
            if (parsedData.ContainsKey("Ref"))
                parsedData.TryGetValue("Ref", out data.Reference);
            if (parsedData.ContainsKey("Error"))
                parsedData.TryGetValue("Error", out data.Error);
            return data;
        }

        private Dictionary<string, string> ParseData(string line, string header, bool isError = false)
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

            try
            {
                string[] Connect = line.Split(equally, 2, StringSplitOptions.RemoveEmptyEntries);
                string[] SrvrRfrn = Connect[1].Split(semicolon, 2, StringSplitOptions.RemoveEmptyEntries);
                string[] Reference = { };
                bool isLocalFile = SrvrRfrn.Length == 1;
                if (isLocalFile)
                {
                    string[] file = SrvrRfrn[0].Split(equally, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (file[0] == "File")
                    {
                        parsedData.Add("File", file[1].Replace("\"", ""));
                    }
                }
                else //it's remote file
                {
                    Reference = SrvrRfrn[1].Split(equally, 2, StringSplitOptions.RemoveEmptyEntries);
                    string[] RefName = Reference[1].Split(quote, 2, StringSplitOptions.RemoveEmptyEntries);
                    string[] name = RefName[0].Split(semicolon, 1, StringSplitOptions.RemoveEmptyEntries);
                    parsedData.Add("Ref", name[0]);
                    string[] Srvr = SrvrRfrn[0].Split(equally, 2, StringSplitOptions.RemoveEmptyEntries);
                    string[] Address = Srvr[1].Split(equally, 2, StringSplitOptions.RemoveEmptyEntries);

                    string[] HostPort = Address[0].Split(colon, 2, StringSplitOptions.RemoveEmptyEntries);
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