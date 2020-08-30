using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModBus
{
   static class Log
    {
        static StringBuilder nodeElictricity = new StringBuilder();
        static StringBuilder nodeElictricityTestID = new StringBuilder();
        static StringBuilder nodeWater = new StringBuilder();
        static StringBuilder nodeGas = new StringBuilder();

        public static StringBuilder logNodeElictricity(string Node) //Собтраю логи со всех потоков
        {
            //все ошибки за минуту складируются здесь, при вызове logWriteElictricity() данные берутся отсюда 
            nodeElictricity.Append("\n" + Node);
            return nodeElictricity;
        }

      public  static void logWriteElictricity() // раз в минуту записываю все вместе с запуском потов
        {
           
            string newLocation = @"C:\AIT";// путь для записи   
            bool exists = System.IO.Directory.Exists(newLocation); // проверка на существования 
            bool Fexists = System.IO.File.Exists(@"\log.txt");
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(newLocation);
                if (!Fexists)
                {
                    System.IO.File.Create(newLocation + @"\log.txt");
                }
            }
            File.AppendAllTextAsync(newLocation + @"\log.txt", nodeElictricity.ToString()); // запись
            nodeElictricity.Clear(); // очистка
            return;
        }

        public static StringBuilder logNodeElictricityTestID(string Node) //Собтраю логи со всез потоков
        {

            nodeElictricityTestID.Append("\n" + Node);
            return nodeElictricityTestID;
        }

        public static void logWriteElictricityTestID() // раз в минуту записываю все
        {

            string newLocation = @"C:\AIT";
            bool exists = System.IO.Directory.Exists(newLocation);
            bool Fexists = System.IO.File.Exists(@"\log1.txt");
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(newLocation);
                if (!Fexists)
                {
                    System.IO.File.Create(newLocation + @"\log1.txt");
                }
            }
            File.AppendAllTextAsync(newLocation + @"\log1.txt", nodeElictricityTestID.ToString());
            nodeElictricityTestID.Clear();
            return;
        }


        public static StringBuilder logWaterNode(string Node) //Собтраю логи со всез потоков
        {

            nodeWater.Append("\n" + Node);
            return nodeWater;
        }

        public static void logWaterWrite() // раз в минуту записываю все
        {
            
            string newLocation = @"C:\AIT";
            bool exists = System.IO.Directory.Exists(newLocation);
            bool Fexists = System.IO.File.Exists(@"\logWater.txt");
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(newLocation);
                if (!Fexists)
                {
                    System.IO.File.Create(newLocation + @"\logWater.txt");
                }
            }
            File.AppendAllTextAsync(newLocation + @"\logWater.txt", nodeWater.ToString());
            nodeWater.Clear();
            return;
        }

        public static StringBuilder logGasNode(string Node) //Собтраю логи со всез потоков
        {

            nodeGas.Append("\n" + Node);
            return nodeGas;
        }

        public static void logGasWrite() // раз в минуту записываю все
        {

            string newLocation = @"C:\AIT";
            bool exists = System.IO.Directory.Exists(newLocation);
            bool Fexists = System.IO.File.Exists(@"\logGas.txt");
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(newLocation);
                if (!Fexists)
                {
                    System.IO.File.Create(newLocation + @"\logGas.txt");
                }
            }
            File.AppendAllTextAsync(newLocation + @"\logGas.txt", nodeGas.ToString());
            nodeGas.Clear();
            return;
        }

    }
}
