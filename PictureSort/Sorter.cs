using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSort
{
    class Sorter
    {
        private string m_sourceFolder;
        private string m_targetFolder;

        public Sorter(string sourceFolder, string targetFolder)
        {
            m_sourceFolder = sourceFolder;
            m_targetFolder = targetFolder;
        }

        public void sort()
        {
            if (!Directory.Exists(m_sourceFolder))
            {
                throw new Exception("Invalid source folder");
            }

            DirectoryInfo sourceFolder = new DirectoryInfo(m_sourceFolder);
            IEnumerable<FileInfo> sourceFiles = sourceFolder.EnumerateFiles();

            foreach (var file in sourceFiles)
            {
                if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    continue;
                }


            }

        }
    }
}
