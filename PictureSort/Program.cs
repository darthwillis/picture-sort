using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input source folder:");
            string sourceFolder = Console.ReadLine();

            Console.WriteLine("Input target folder:");
            string targetFolder = Console.ReadLine();

            Sorter sorter = new Sorter(sourceFolder, targetFolder);
            sorter.sort();
        }
    }
}
