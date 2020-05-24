using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTP
{
    class Utils
    {
        /// <summary>
        /// Проверяет, является ли указанный путь директорией.
        /// </summary>
        /// <param name="sourcePath">Исходный путь</param>
        /// <returns>Возвращает true, если путь является директорией.</returns>
        public static bool IsDirectory(string sourcePath)
        {
            FileAttributes attr = File.GetAttributes(sourcePath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) return true;
            return false;
        }

        public static void Copy(string sourcePath, string destinationPath)
        {
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = @"C:\WINDOWS\system32\xcopy.exe";
            proc.StartInfo.Arguments = String.Format("{0} {1} /I /E", sourcePath, destinationPath);
            // /E = скопировать подпапки(пустые в том числе!).
            // /I = если дестинейшн не существует, создать папку с нужным именем. 
            proc.Start();
            proc.WaitForExit();
        }


        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }


    }
}
