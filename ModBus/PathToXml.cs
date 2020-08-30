using System;
using System.IO;

namespace ModBus
{
    public  class PathXml
    {
        // Пути к XML файлам

        public  string ElectricityRelativePathToXml()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string relPath = @"..\netcoreapp3.1\XML\Electricity.xml"; // Относительный путь к файлу
            string resPath = Path.Combine(exeDir, relPath); // Объединяет две строки в путь.
            resPath = Path.GetFullPath(resPath); // Возвращает для указанной строки пути абсолютный путь.
            return resPath;
        }

        public  string WaterRelativePathToXml()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string relPath = @"..\netcoreapp3.1\XML\WaterPLC.xml"; // Относительный путь к файлу
            string resPath = Path.Combine(exeDir, relPath); // Объединяет две строки в путь.
            resPath = Path.GetFullPath(resPath); // Возвращает для указанной строки пути абсолютный путь.
            return resPath;
        }

        public string GasRelativePathToXml()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string relPath = @"..\netcoreapp3.1\XML\Gas.xml"; // Относительный путь к файлу
            string resPath = Path.Combine(exeDir, relPath); // Объединяет две строки в путь.
            resPath = Path.GetFullPath(resPath); // Возвращает для указанной строки пути абсолютный путь.
            return resPath;
        }
    }
}