using System.Text.RegularExpressions;

namespace TestServiceCloud
{
    public class Regexes
    {
        //используется не всё, но могут пригодиться
        public Regex Remote = new Regex(@"^(\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");
        public Regex Local = new Regex(@"^((?:[a-zA-Z]\:){0,1}(?:[\\/][\w.]+){1,})");
        public Regex FullPathValidate = new Regex(@"(^([a-z]|[A-Z]):(?=\\(?![\0-\37<>:/\\|?*])|\/(?![\0-\37<>:/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:/\\|?*]+))((\\|\/)[^\0-\37<>:/\\|?*]+|(\\|\/)$)*()$");
        public Regex WorkedLocal = new Regex(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");
        //диапазон портов: 0-65535: системные 0—1023, пользовательские 1024—49151, частные 49152—65535)
        public Regex ConnectFile = new Regex(@"^Connect=File=\S+");
        public Regex ConnectServer = new Regex(@"^Connect=Srvr=\S+");
        //в будущем можно сделать проверку для следующих значений:
        public Regex Host;// = new Regex(@"");
        public Regex Port;// = new Regex(@"");
        public Regex Reference;// = new Regex@("");
        public string ConnectFileString = "^Connect=File=";
        public string ConnectServerString = "^Connect=Srvr=";
        public string HeaderString = "^(\\[).*(\\])$";
    }
}
