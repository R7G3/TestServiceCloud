using System;
using System.Text;

namespace TestServiceCloud
{
    public class ConnectionString
    {
        public string Header;
        public string Source;
        public string Path;
        public string Host;
        public string Port;
        public string Reference;
        public string Error;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (!String.IsNullOrEmpty(Header))
                builder.AppendFormat($"{Header}\r\nConnect=");
            if (!String.IsNullOrEmpty(Host))
            {
                builder.AppendFormat($"Srvr=\"{Host}");
                if (!String.IsNullOrEmpty(Port))
                {
                    builder.AppendFormat($":{Port}\";");
                }
                else
                {
                    builder.Append("\";");
                }
            }
            if (!String.IsNullOrEmpty(Reference))
                builder.AppendFormat($"Ref=\"{Reference}\";");
            if (!String.IsNullOrEmpty(Path))
                builder.AppendFormat($"File=\"{Path}\";");

            return builder.ToString();
        }
    }
}