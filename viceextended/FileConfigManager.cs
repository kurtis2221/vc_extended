using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileConfigManager
{
    class FCM
    {
        int lines = 0;

        //Less Memory when less than 1000 lines
        public int SetMaxLines(int ln)
        {
            return lines = ln;
        }

        public string ReadData(string filename, string data)
        {
            string line = "";
            if (lines != 0)
            {
                for (int i = 1; i < lines; i++)
                {
                    line = GetLine(filename, i);
                    if (line == null) continue;
                    if (line.Substring(0, data.Length) != data) continue;

                    if (line.Substring(0, data.Length) == data)
                        return line.Substring(data.Length + 1, line.Length - (data.Length + 1));
                }
            }
            else
            {
                for (int i = 1; i < 1000; i++)
                {
                    line = GetLine(filename, i);
                    if (line == null) continue;
                    if (line.Substring(0, data.Length) != data) continue;

                    if (line.Substring(0, data.Length) == data)
                        return line.Substring(data.Length + 1, line.Length - (data.Length + 1));
                }
            }
            return null;
        }

        public void DeleteData(string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
            File.Create(filename).Close();
        }

        public void WriteData(string filename, string data, string value)
        {
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(data + "=" + value);
            sw.Close();
            fs.Close();
        }

        string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName, Encoding.UTF7))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }
    }
}