using MongoDB.Bson;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModBus
{

    internal class Program
    {
        

        private static Mutex mutex = null;

        private static void Main(string[] args)
        {
            Console.SetWindowSize(200 , Console.WindowHeight);

            const string appName = "ModBus";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew); // Одна версия приложения

            if (!createdNew)
            {
                Console.WriteLine(appName + " Приложение уже запущено! Exiting t.");
                //Console.ReadKey();
                return;
            }
            Console.WriteLine("Continuing with the application");


            //DelayStart delayStart = new DelayStart();
            Task.Factory.StartNew(() => DelayStart.Start());

            Console.Read();
        }
    }

    internal class ElectricityXmlDoc
    {
        public int id { get; set; }
        public string IP { get; set; }
        public string name { get; set; }
        public string interview { get; set; }
        public string zone { get; set; }
        public string comment { get; set; }
    }

    internal class ElectricityMongoNode
    {
        public ObjectId _id { get; set; }
        public int ID { get; set; }
        public DateTime dateTime { get; set; }
        public float wP_in { get; set; }
        public float WP_out { get; set; }
        public float WQ_in { get; set; }
        public float WQ_oup { get; set; }
        public float WQ { get; set; }
    }

    internal class WaterPLC_XmlDoc
    {
        public int id { get; set; }
        public string name { get; set; }
        public string interview { get; set; }
        public int DB { get; set; }
        public int address { get; set; }
        public int length { get; set; }
        public string IP { get; set; }
        public int rack { get; set; }
        public int slot { get; set; }
    }

    internal class WaterPLC_MongoNode
    {
        public ObjectId _id { get; set; }
        public int ID { get; set; }
        public string name { get; set; }
        public float value { get; set; }
        public DateTime dateTime { get; set; }
    }

    internal class Gas_XmlDoc
    {
        public int id { get; set; }
        public string name { get; set; }
        public string interview { get; set; }
        public int DB { get; set; }
        public int address { get; set; }
        public int length { get; set; }
        public string IP { get; set; }
        public int rack { get; set; }
        public int slot { get; set; }
    }

    internal class Gas_MongoNode
    {
        public ObjectId _id { get; set; }
        public int ID { get; set; }
        public string name { get; set; }
        public float value { get; set; }
        public DateTime dateTime { get; set; }
    }
}