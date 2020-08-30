using Modbus.Device;
using ModBus.Classes;
using MongoDB.Driver;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ModBus
{
    internal class Electricity
    {
         public void ReadXmlElectricity()
        {

            PathXml pathXml = new PathXml();
            string way = pathXml.ElectricityRelativePathToXml();
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
                ElectricityXmlDoc parametrs = new ElectricityXmlDoc();
                parametrs.id = Convert.ToInt32(xnode.SelectSingleNode("id").InnerText);
                parametrs.IP = xnode.SelectSingleNode("IP").InnerText;
                parametrs.name = xnode.SelectSingleNode("name").InnerText;
                parametrs.interview = xnode.SelectSingleNode("interview").InnerText;
                if (parametrs.interview == "-") { continue; }
                parametrs.zone = xnode.SelectSingleNode("zone").InnerText;
                parametrs.comment = xnode.SelectSingleNode("comment").InnerText;

                //Thread caller = new Thread(
                //    delegate ()
                //    {
                //        SaveDocsElectricity(parametrs);
                //    }
                //    );
                //caller.Start();


                Task.Factory.StartNew(() => SaveDocsElectricity(parametrs));
                
            }
            return;
        }

        public  void SaveDocsElectricity(ElectricityXmlDoc post)
        {
            //дата+ запустился
            
            ElectricityMongoNode mongoNode = new ElectricityMongoNode();
            ModbusIpMaster master = null;
            TcpClient tcpClient = null;

            string ipAddress = post.IP;
            int tcpPort = 502;
            PAC3200_Power A1 = new PAC3200_Power();
           


            //try
            //{
            //    IAsyncResult asyncResult = tcpClient.BeginConnect(ipAddress, tcpPort, null, null);
            //    asyncResult.AsyncWaitHandle.WaitOne(1000, true); //wait for 3 sec

            //    if (!asyncResult.IsCompleted)
            //    {
            //        for (int i = 0; i < 3; i++)
            //        {
            //            IAsyncResult asyncResult1 = tcpClient.BeginConnect(ipAddress, tcpPort, null, null);
            //            asyncResult1.AsyncWaitHandle.WaitOne(1000, true); //wait for 3 sec
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string error = "Electricity: ID = " + post.id + " " + post.IP + " date - "
            //        + DateTime.Now.ToString() + " Ошибка подключения TCP";
            //    // ":Connect process " + ex.StackTrace + "==>" + ex.Message
            //    Console.WriteLine(error);
            //    Log.logNode(error);
            //    return;
            //}
            for (int i = 0; i <= 2; i++)
            {
                tcpClient = new TcpClient();

                try
                {
                    tcpClient.Connect(ipAddress, tcpPort);

                    if (tcpClient.Connected)
                    {
                        master = ModbusIpMaster.CreateIp(tcpClient);
                        master.Transport.Retries = 0; //don't have to do retries
                        master.Transport.ReadTimeout = 1500;
                        A1.Registers = master.ReadHoldingRegisters(1, 2801, 20);
                        A1.ConvertValues();
                        
                        break;
                    }
                }
                catch
                {

                    string error = "Electricity: ID = " + post.id + " " + post.IP + " date - "
                        + DateTime.Now.ToString() + " Ошибка подключения TCP";
                    // ":Connect process " + ex.StackTrace + "==>" + ex.Message
                    Console.WriteLine(error);
                    if(post.id != 999)
                    {
                        Log.logNodeElictricityTestID(error);
                        Log.logNodeElictricity(error);
                       
                    }
                    else
                    {
                        Log.logNodeElictricity(error);
                        return;
                    }
                    
                  
                }
                if (i == 2)
                {
                    return;
                }
                tcpClient.Close();
                tcpClient.Dispose();

            }

            tcpClient.Close();
            tcpClient.Dispose();


            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            string date = new DateTime(year, month, 1).ToShortDateString();
            DateTime time = DateTime.Now;

            mongoNode.ID = post.id;
            mongoNode.wP_in = A1.Values[0];
            mongoNode.WP_out = A1.Values[1];
            mongoNode.WQ_in = A1.Values[2];
            mongoNode.WQ_oup = A1.Values[3];
            mongoNode.WQ = A1.Values[4];
            mongoNode.dateTime = time;



            IMongoCollection<ElectricityMongoNode> collection = null;

            try
            {
                MongoClient client = new MongoClient("mongodb://localhost");
                IMongoDatabase DB = client.GetDatabase("Electricity");
                collection = DB.GetCollection<ElectricityMongoNode>(date );
            }
            catch(Exception e)
            {
                string error = "Electricity: ID = " + post.id + " " + post.IP +
                    "Не удалось подключиться к базе данных " + e.Message;
                Console.WriteLine(error);
                Log.logNodeElictricity(error);
                return;
            }
            
            try
            {
                 collection.InsertOne(mongoNode);
                
            }
            catch (Exception e)
            {
                string error = "Electricity: ID = " + post.id + " " + post.IP +
                    "Ошибка записи в MongoDB:" + time + "  " + e.Message;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;
                Log.logNodeElictricity(error);
                return;
            }
            Console.WriteLine("Electricity: ID = " + post.id + " " + post.IP + " " + mongoNode.wP_in + 
                " " + mongoNode.WP_out + " " +mongoNode.WQ_in + " " + mongoNode.WQ_oup + " " 
                + mongoNode.WQ + "Запить произведена: " + time);


            return;

        }
    }
}