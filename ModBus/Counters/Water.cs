using ModBus.Classes;
using MongoDB.Driver;
using Sharp7;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ModBus
{
    class Water
    {
         public void ReadXmlWater()
        {
            PathXml pathXml = new PathXml();
            string way = pathXml.WaterRelativePathToXml();
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(way);
            }
            catch (Exception e)
            {
                Console.WriteLine("Неверный путь к XML файлу:" + e.Message);
                Console.ReadLine();
            }
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList nodeList = xDoc.DocumentElement.SelectNodes("/counters/counter");


            foreach (XmlNode xnode in nodeList)
            {
                Water_XmlDoc parametrs = new Water_XmlDoc();
                parametrs.id = Convert.ToInt32(xnode.SelectSingleNode("id").InnerText);
                parametrs.name = xnode.SelectSingleNode("name").InnerText;
                parametrs.interview = xnode.SelectSingleNode("interview").InnerText;
                if (parametrs.interview == "-") { continue; }
                parametrs.DB = Convert.ToInt32(xnode.SelectSingleNode("db").InnerText);
                parametrs.address = Convert.ToInt32(xnode.SelectSingleNode("address").InnerText);
                parametrs.length = Convert.ToInt32(xnode.SelectSingleNode("length").InnerText);
                parametrs.IP = xnode.SelectSingleNode("IP").InnerText;
                parametrs.rack = Convert.ToInt32(xnode.SelectSingleNode("rack").InnerText);
                parametrs.slot = Convert.ToInt32(xnode.SelectSingleNode("slot").InnerText);

                //Thread caller = new Thread(
                //    delegate ()
                //    {
                //        SaveDocsWaterPLC(parametrs);
                //    }
                //    );
                //caller.Start();

                Task.Factory.StartNew(() => SaveDocsWater(parametrs));
            }
        }

        public  void SaveDocsWater(Water_XmlDoc parametrs)
        {
            DateTime time = DateTime.Now;
            Water_MongoNode water_MongoNode = new Water_MongoNode();
            S7Client s7Client = new S7Client();
            float value = 0;
            byte[] Buffer = new byte[parametrs.length];

            

            for(int i=0; i <=2; i++)
            {
                s7Client.ConnectTo(parametrs.IP, parametrs.rack, parametrs.slot);

                if (s7Client.Connected)
                {
                    s7Client.DBRead(parametrs.DB, parametrs.address, parametrs.length, Buffer);
                    value = S7.GetRealAt(Buffer, 0);
                    break;
                }
                else
                {
                    string error = "Water: ID = " + parametrs.id + " " + parametrs.name + " " + time + " " +
                        "не удалось подключиться к адресу " + parametrs.IP;

                    Console.WriteLine(error);
                    Log.logWaterNode(error);
                    return;
                }
            }

           
            
            //Console.WriteLine(parametrs.id + "   " + parametrs.name + "    " + value.ToString() + "  " + time);
            s7Client.Disconnect();
            
            
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            string date = new DateTime(year, month, 1).ToShortDateString();

            water_MongoNode.ID = parametrs.id;
            water_MongoNode.name = parametrs.name;
            water_MongoNode.value = value;
            water_MongoNode.dateTime = time;

            IMongoCollection<Water_MongoNode> collection = null;
            try
            {
                MongoClient client = new MongoClient("mongodb://localhost");
                IMongoDatabase DB = client.GetDatabase("Water");
                collection = DB.GetCollection<Water_MongoNode>(date);
            }
            catch (Exception e)
            {
                string error = "Water: ID = " + parametrs.id + " " + parametrs.name + time +
                    "  Не удалось подключиться к базе данных " + e.Message;

                Console.WriteLine(error);

                Log.logWaterNode(error);
                return;
            }

            try
            {
                 collection.InsertOne(water_MongoNode);
                //collection.InsertOne(post);
            }
            catch (Exception e)
            {
                string error = "Water: ID = " + parametrs.id + " " + parametrs.name +
                    "Ошибка записи в MongoDB:" + time + "  " + e.Message;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;

                Log.logWaterNode(error);
                return;
            }
            Console.WriteLine("Water: ID = " + parametrs.id + " " + water_MongoNode.name + " "
                + water_MongoNode.value + " " + "Запить произведена: " + time);
            return;
        }
    }
}