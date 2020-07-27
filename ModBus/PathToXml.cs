using System;
using System.IO;

namespace ModBus
{
    public  class PathXml
    {
        public  string ElectricityRelativePathToXml()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            //Console.WriteLine("exe  " + exeDir);
            string relPath = @"..\netcoreapp3.1\XML\Electricity.xml"; // Относительный путь к файлу
            string resPath = Path.Combine(exeDir, relPath); // Объединяет две строки в путь.
            //Console.WriteLine(resPath);
            resPath = Path.GetFullPath(resPath); // Возвращает для указанной строки пути абсолютный путь.
            //Console.WriteLine(resPath);
            return resPath;
        }

        public  string Water_RelativePathToXml()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            //Console.WriteLine("exe  " + exeDir);
            string relPath = @"..\netcoreapp3.1\XML\WaterPLC.xml"; // Относительный путь к файлу
            string resPath = Path.Combine(exeDir, relPath); // Объединяет две строки в путь.
            //Console.WriteLine(resPath);
            resPath = Path.GetFullPath(resPath); // Возвращает для указанной строки пути абсолютный путь.
            //Console.WriteLine(resPath);
            return resPath;
        }

        public string Gas_RelativePathToXml()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            //Console.WriteLine("exe  " + exeDir);
            string relPath = @"..\netcoreapp3.1\XML\Gas.xml"; // Относительный путь к файлу
            string resPath = Path.Combine(exeDir, relPath); // Объединяет две строки в путь.
            //Console.WriteLine(resPath);
            resPath = Path.GetFullPath(resPath); // Возвращает для указанной строки пути абсолютный путь.
            //Console.WriteLine(resPath);
            return resPath;
        }
    }
}