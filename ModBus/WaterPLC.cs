﻿using MongoDB.Driver;
using Sharp7;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ModBus
{
    class WaterPLC
    {
         public void ReadXmlWaterPLC()
        {
            PathXml pathXml = new PathXml();
            string way = pathXml.WaterPLC_RelativePathToXml();
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
                WaterPLC_XmlDoc parametrs = new WaterPLC_XmlDoc();
                parametrs.id = Convert.ToInt32(xnode.SelectSingleNode("id").InnerText);
                parametrs.name = xnode.SelectSingleNode("name").InnerText;
                parametrs.interview = xnode.SelectSingleNode("interview").InnerText;
                if (parametrs.interview == "-") { continue; }
                parametrs.DB = Convert.ToInt32(xnode.SelectSingleNode("db").InnerText);
                parametrs.address = Convert.ToInt32(xnode.SelectSingleNode("address").InnerText);
                parametrs.length = Convert.ToInt32(xnode.SelectSingleNode("length").InnerText);

                //Thread caller = new Thread(
                //    delegate ()
                //    {
                //        SaveDocsWaterPLC(parametrs);
                //    }
                //    );
                //caller.Start();

                Task.Factory.StartNew(() => SaveDocsWaterPLC(parametrs));
            }
        }

        public  void SaveDocsWaterPLC(WaterPLC_XmlDoc parametrs)
        {
            DateTime time = DateTime.Now;
            WaterPLC_MongoNode waterPLC_MongoNode = new WaterPLC_MongoNode();
            S7Client s7Client = new S7Client();
            s7Client.ConnectTo("192.168.219.190", 0, 2);
            float value = 0;
            byte[] Buffer = new byte[parametrs.length];

            if (s7Client.Connected)
            {
                s7Client.DBRead(parametrs.DB, parametrs.address, parametrs.length, Buffer);
                value = S7.GetRealAt(Buffer, 0);
            }
            else
            {
                string error = "WaterPLC: ID = " + parametrs.id + " " + parametrs.name + time +
                    "не удалось подключиться к адресу 192.168.219.190";

                Console.WriteLine(error);
                Log.logWaterNode(error);
                return;
            }
            
            //Console.WriteLine(parametrs.id + "   " + parametrs.name + "    " + value.ToString() + "  " + time);
            s7Client.Disconnect();
            
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            string date = new DateTime(year, month, 1).ToShortDateString();

            waterPLC_MongoNode.ID = parametrs.id;
            waterPLC_MongoNode.name = parametrs.name;
            waterPLC_MongoNode.value = value;
            waterPLC_MongoNode.dateTime = time;

            IMongoCollection<WaterPLC_MongoNode> collection = null;
            try
            {
                MongoClient client = new MongoClient("mongodb://localhost");
                IMongoDatabase DB = client.GetDatabase("Water");
                collection = DB.GetCollection<WaterPLC_MongoNode>(date);
            }
            catch (Exception e)
            {
                string error = "WaterPLC: ID = " + waterPLC_MongoNode.ID + " " + parametrs.name + time +
                    "  Не удалось подключиться к базе данных " + e.Message;

                Console.WriteLine(error);

                Log.logWaterNode(error);
                return;
            }

            try
            {
                 collection.InsertOne(waterPLC_MongoNode);
                //collection.InsertOne(post);
            }
            catch (Exception e)
            {
                string error = "WaterPLC: ID = " + waterPLC_MongoNode.ID + " " + parametrs.name +
                    "Ошибка записи в MongoDB:" + time + "  " + e.Message;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;

                Log.logWaterNode(error);
                return;
            }
            Console.WriteLine("WaterPLC: ID = " + waterPLC_MongoNode.ID + " " + waterPLC_MongoNode.name + " "
                + waterPLC_MongoNode.value + " " + "Запить произведена: " + time);
            return;
        }
    }
}