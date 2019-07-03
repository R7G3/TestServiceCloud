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
            Regexes pattern = new Regexes();
            List<ConnectionString> data = new List<ConnectionString>();
            string header = String.Empty;
            bool isPreviousHeader = false;

            while ((line = reader.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, pattern.HeaderString))
                {
                    header = line;
                    isPreviousHeader = true;
                }
                if (Regex.IsMatch(line, pattern.ConnectFileString) || Regex.IsMatch(line, pattern.ConnectServerString)) ////if (Regex.IsMatch(line, pattern.ConnectFileString))
                {
                    if(isPreviousHeader)
                    {
                        var parsed = ParseData(line, header);
                        var created = CreateConnectionString(parsed);
                        data.Add(created);
                        isPreviousHeader = false;
                        header = String.Empty;
                    }
                    else
                    {
                        var parsed = ParseData(line, header, true);
                        var created = CreateConnectionString(parsed);
                        data.Add(created);
                        log.Warn("Строка подключения: " + line + "\nне имеет заголовка и будет записана в bad_data.txt");
                    }
                }
                counter++;
            }
            file.Close();
            return data;
        }

        private ConnectionString CreateConnectionString(Dictionary<string, string> parsedData)
        {
            ConnectionString data = new ConnectionString();
            if (parsedData.ContainsKey("Header"))
                data.Header = parsedData["Header"];
            if (parsedData.ContainsKey("Source"))
                data.Source = parsedData["Source"];
            if (parsedData.ContainsKey("File"))
                data.Path = parsedData["File"];
            if (parsedData.ContainsKey("Srvr"))
                data.Host = parsedData["Srvr"];
            if (parsedData.ContainsKey("Port"))
                data.Port = parsedData["Port"];
            if (parsedData.ContainsKey("Ref"))
                data.Reference = parsedData["Ref"];
            if (parsedData.ContainsKey("Error"))
                data.Error = parsedData["Error"];
            return data;
        }

        private Dictionary<string, string> ParseData(string line, string header, bool isError = false)
        {
            //split separators:
            char[] equally = { '=' };
            char[] semicolon = { ';' };
            char[] colon = { ':' };
            char[] quote = { '\"' };

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
                if (SrvrRfrn.Length == 1) //it's local file
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
                    if (Address[0].Contains(':')) //remote path have port
                    {
                        string[] HostPort = Address[0].Split(colon, 2, StringSplitOptions.RemoveEmptyEntries);
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