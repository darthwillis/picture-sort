using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PictureSort
{
    class Sorter
    {
        private static Regex r = new Regex(":");
        private string m_sourceFolder;
        private string m_targetFolder;
        private List<string> oddFiles;

        public Sorter(string sourceFolder, string targetFolder)
        {
            m_sourceFolder = sourceFolder;
            m_targetFolder = targetFolder;
            oddFiles = new List<string>();
        }

        public void sort()
        {
            if (!Directory.Exists(m_sourceFolder))
            {
                throw new Exception("Invalid source folder");
            }

            if (!Directory.Exists(m_targetFolder))
            {
                Directory.CreateDirectory(m_targetFolder);
            }

            DirectoryInfo targetFolder = new DirectoryInfo(m_targetFolder);

            Console.WriteLine("Moving files from " + m_sourceFolder + " to " + m_targetFolder);

            DirectoryInfo sourceFolder = new DirectoryInfo(m_sourceFolder);
            IEnumerable<FileInfo> sourceFiles = sourceFolder.EnumerateFiles();

            foreach (var file in sourceFiles)
            {
                if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    continue;
                }

                Console.WriteLine("Found file:");
                Console.WriteLine(file.FullName);

                DateTime creation = file.CreationTime;
                DateTime modified = file.LastWriteTime;
                if (modified.Year < creation.Year)
                {
                    creation = modified;
                }

                DateTime dateTaken;
                DateTime mediaCreated;
                if (tryGetDateTaken(file, out dateTaken))
                {
                    if (dateTaken != DateTime.MinValue)
                    {
                        creation = dateTaken;
                    }
                }
                else if (tryGetMediaCreated(file, out mediaCreated))
                {
                    if (mediaCreated != DateTime.MinValue)
                    {
                        creation = mediaCreated;
                    }
                }
                else
                {
                    oddFiles.Add(file.FullName);
                }

                string year = creation.Year.ToString();
                string month = getMonthString(creation.Month);

                string newFolderPath = targetFolder.FullName + "\\" + year + "\\" + month + "\\";
                string newFilePath = newFolderPath + file.Name;

                Console.WriteLine("Moving to:");
                Console.WriteLine(newFilePath);

                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }

                file.MoveTo(newFilePath);
            }

            Console.WriteLine("Non photo or video files:");
            foreach (var fileName in oddFiles)
            {
                Console.WriteLine(fileName);
            }
        }

        private bool tryGetDateTaken(FileInfo file, out DateTime dateTaken)
        {
            try
            {
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string taken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    dateTaken = DateTime.Parse(taken);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Non-image file type found");
                Console.WriteLine(e.ToString());
            }
            dateTaken = DateTime.MinValue;
            return false;
        }

        private bool tryGetMediaCreated(FileInfo file, out DateTime mediaCreated)
        {
            try
            {
                Shell32.Folder folder = GetShell32NameSpaceFolder(file.DirectoryName);
                Shell32.FolderItem folderItem = folder.ParseName(file.Name);

                string created = folder.GetDetailsOf(folderItem, 209);
                string sanitized = "";
                foreach (char c in created.ToCharArray())
                {
                    if ((int)c < 128)
                    {
                        sanitized += c;
                    }
                }
                mediaCreated = DateTime.Parse(sanitized);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Non-video file type found");
                Console.WriteLine(e.ToString());
            }
            mediaCreated = DateTime.MinValue;
            return false;
        }

        private Shell32.Folder GetShell32NameSpaceFolder(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");

            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
            System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }

        private string getMonthString(int month)
        {
            switch (month)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return ".";
            }
        }
    }
}
