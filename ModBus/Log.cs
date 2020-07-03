using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModBus
{
   static class Log
    {
       static StringBuilder node = new StringBuilder();
        static StringBuilder node1 = new StringBuilder();
        static StringBuilder nodeWater = new StringBuilder();
        static StringBuilder nodeGas = new StringBuilder();
        public static StringBuilder logNode(string Node) //Собтраю логи со всез потоков
        {
            node.Append("\n" + Node);
            return node;
        }

      public  static void logWrite() // раз в минуту записываю все
        {
           
            string newLocation = @"C:\AIT";       
            bool exists = System.IO.Directory.Exists(newLocation);
            bool Fexists = System.IO.File.Exists(@"\log.txt");
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(newLocation);
                if (!Fexists)
                {
                    System.IO.File.Create(newLocation + @"\log.txt");
                }
            }
            File.AppendAllTextAsync(newLocation + @"\log.txt", node.ToString());
            node.Clear();
            return;
        }

        public static StringBuilder logNode1(string Node) //Собтраю логи со всез потоков
        {

            node1.Append("\n" + Node);
            return node1;
        }

        public static void logWrite1() // раз в минуту записываю все
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
            File.AppendAllTextAsync(newLocation + @"\log1.txt", node1.ToString());
            node1.Clear();
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
