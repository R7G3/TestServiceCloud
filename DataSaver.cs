using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TestServiceCloud
{
    class DataSaver
    {
        private static DataSaver instance;
        private static string path = Directory.GetCurrentDirectory();
        private string badData = path + "\\" + @"bad_data.txt";
        private bool isAppend = true;

        public void SaveToFiles(List<ConnectionString> dataList, int countOfParts)
        {
            List<ConnectionString> correctData = GetCorrect(dataList);
            List<ConnectionString> invalidData = GetInvalid(dataList);
            string[] fileName = new string[countOfParts];
            for (int i = 0; i < countOfParts; i++)
            {

                fileName[i] = path + "\\" + "base_" + i + ".txt";
            }
            foreach(string name in fileName)
            {
                File.Delete(name);
            }

            int fileCount = 0;
            for (int dataCount = 0; dataCount < correctData.Count; dataCount++)
            {
                if (fileCount == countOfParts)
                {
                    fileCount = 0;
                }
                else
                {
                    AppendToFile(fileName[fileCount], correctData[dataCount].ToString() + "\r\n");
                    fileCount++;
                }
            }

            if (invalidData.Count != 0)
            {
                File.Delete(badData);
                File.Create(badData).Close();
                for (int i = 0; i < invalidData.Count; i++)
                {
                    AppendToFile(badData, invalidData[i].ToString() + "\r\n");
                }
            }
        }

        private List<ConnectionString> GetInvalid(List<ConnectionString> list)
        {
            List<ConnectionString> invalid = new List<ConnectionString>();
            foreach (ConnectionString obj in list)
            {
                if (obj.Error == "True")
                {
                    invalid.Add(obj);
                }
            }
            return invalid;
        }

        private List<ConnectionString> GetCorrect(List<ConnectionString> list)
        {
            List<ConnectionString> correct = new List<ConnectionString>();
            foreach (ConnectionString obj in list)
            {
                if (String.IsNullOrEmpty(obj.Error))
                {
                    correct.Add(obj);
                }
            }
            return correct;
        }

        private void AppendToFile(string path, string data)
        {
            using (StreamWriter writer = new StreamWriter(path, isAppend, Encoding.UTF8))
            {
                writer.WriteLine(data);
            }
        }

        private DataSaver()
        {
        }

        public static DataSaver GetInstance()
        {
            if (instance == null)
                instance = new DataSaver();
            return instance;
        }
    }
}
