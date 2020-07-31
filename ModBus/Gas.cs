using ModBus.Classes;
using MongoDB.Driver;
using Sharp7;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ModBus
{
   public class Gas
    {
         public void ReadXmlGas()
        {
            PathXml pathXml = new PathXml();
            string way = pathXml.Gas_RelativePathToXml();
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
                Gas_XmlDoc parametrs = new Gas_XmlDoc();
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

                Task.Factory.StartNew(() => SaveDocsGas(parametrs));
            }
        }

         void SaveDocsGas(Gas_XmlDoc parametrs)
         {
            DateTime time = DateTime.Now;
            Gas_MongoNode gas_MongoNode = new Gas_MongoNode();
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
                    string error = "Gas: ID = " + parametrs.id + " " + parametrs.name + " " + time + " " +
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

            gas_MongoNode.ID = parametrs.id;
            gas_MongoNode.name = parametrs.name;
            gas_MongoNode.value = value;
            gas_MongoNode.dateTime = time;

            IMongoCollection<Gas_MongoNode> collection = null;
            try
            {
                MongoClient client = new MongoClient("mongodb://localhost");
                IMongoDatabase DB = client.GetDatabase("Gas");
                collection = DB.GetCollection<Gas_MongoNode>(date);
            }
            catch (Exception e)
            {
                string error = "Gas: ID = " + gas_MongoNode.ID + " " + parametrs.name + time +
                    "  Не удалось подключиться к базе данных " + e.Message;

                Console.WriteLine(error);

                Log.logWaterNode(error);
                return;
            }

            try
            {
                 collection.InsertOne(gas_MongoNode);
                //collection.InsertOne(post);
            }
            catch (Exception e)
            {
                string error = "Gas: ID = " + gas_MongoNode.ID + " " + parametrs.name +
                    "Ошибка записи в MongoDB:" + time + "  " + e.Message;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;

                Log.logWaterNode(error);
                return;
            }
            Console.WriteLine("Gas: ID = " + gas_MongoNode.ID + " " + gas_MongoNode.name + " "
                + gas_MongoNode.value + " " + "Запить произведена: " + time);
            return;
         }
    }
}