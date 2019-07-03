using System;
using System.Collections.Generic;
using SimpleLogger;

namespace TestServiceCloud
{
    class Program
    {
        public static bool debug = true;
        public static int quantityOfFiles = 5;

        static void Main(string[] args)
        {
            Logger log = new Logger(debug);
            string[] arguments = Environment.GetCommandLineArgs();
            List<string> paths = ArgumentsParser.GetFilesPaths(arguments);
            DataParser parser = DataParser.GetInstance();
            List<ConnectionString> dataList = parser.GetAllData(paths);
            DataSaver saver = DataSaver.GetInstance();
            saver.SaveToFiles(dataList, quantityOfFiles);
            Console.WriteLine("Приложение успешно завершило работу");
            Console.ReadKey();
        }
    }
}
