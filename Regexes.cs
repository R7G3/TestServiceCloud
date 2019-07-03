using System.Text.RegularExpressions;

namespace TestServiceCloud
{
    public static class Regexes
    {
        //используется не всё, но могут пригодиться
        public static Regex Remote = new Regex(@"^(\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");
        public static Regex Local = new Regex(@"^((?:[a-zA-Z]\:){0,1}(?:[\\/][\w.]+){1,})");
        public static Regex FullPathValidate = new Regex(@"(^([a-z]|[A-Z]):(?=\\(?![\0-\37<>:/\\|?*])|\/(?![\0-\37<>:/\\|?*])|$)|^\\(?=[\\\/][^\0-\37<>:/\\|?*]+)|^(?=(\\|\/)$)|^\.(?=(\\|\/)$)|^\.\.(?=(\\|\/)$)|^(?=(\\|\/)[^\0-\37<>:/\\|?*]+)|^\.(?=(\\|\/)[^\0-\37<>:/\\|?*]+)|^\.\.(?=(\\|\/)[^\0-\37<>:/\\|?*]+))((\\|\/)[^\0-\37<>:/\\|?*]+|(\\|\/)$)*()$");
        public static Regex WorkedLocal = new Regex(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");
        //диапазон портов: 0-65535: системные 0—1023, пользовательские 1024—49151, частные 49152—65535)
        public static Regex ConnectFile = new Regex(@"^Connect=File=\S+");
        public static Regex ConnectServer = new Regex(@"^Connect=Srvr=\S+");
        //в будущем можно сделать проверку для следующих значений:
        public static Regex Host;// = new Regex(@"");
        public static Regex Port;// = new Regex(@"");
        public static Regex Reference;// = new Regex@("");
        public static string ConnectFileString = "^Connect=File=";
        public static string ConnectServerString = "^Connect=Srvr=";
        public static string HeaderString = "^(\\[).*(\\])$";
    }
}
