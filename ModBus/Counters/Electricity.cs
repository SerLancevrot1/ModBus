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
            // во всех классах счетчиков примерно одно и то же
            // Подгрузка пути
            PathXml pathXml = new PathXml();
            string way = pathXml.ElectricityRelativePathToXml();
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(way); // проверка существования документа XML
            }
            catch (Exception e)
            {
                Console.WriteLine("Неверный путь к XML файлу:" + e.Message);
                Console.ReadLine();
            }

            //Чтение XML
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList nodeList = xDoc.DocumentElement.SelectNodes("/counters/counter");

            foreach (XmlNode xnode in nodeList)
            {
                // запись настроек счетчика в экземляр класса parametrs
                ElectricityXmlDoc parametrs = new ElectricityXmlDoc();
                parametrs.id = Convert.ToInt32(xnode.SelectSingleNode("id").InnerText);
                parametrs.IP = xnode.SelectSingleNode("IP").InnerText;
                parametrs.name = xnode.SelectSingleNode("name").InnerText;
                parametrs.interview = xnode.SelectSingleNode("interview").InnerText;
                if (parametrs.interview == "-") { continue; } // если - пропуск итерации
                parametrs.zone = xnode.SelectSingleNode("zone").InnerText;
                parametrs.comment = xnode.SelectSingleNode("comment").InnerText;

                Task.Factory.StartNew(() => SaveDocsElectricity(parametrs));
                
            }
            return;
        }

        public  void SaveDocsElectricity(ElectricityXmlDoc post)
        {
            
            ElectricityMongoNode mongoNode = new ElectricityMongoNode();
            ModbusIpMaster master = null;
            TcpClient tcpClient = null;

            string ipAddress = post.IP;
            int tcpPort = 502;
            PAC3200_Power A1 = new PAC3200_Power();
           
            // попытка подключения

            for (int i = 0; i <= 2; i++)
            {
                // программа время от времени не читает несколько значений электричества, если создавать
                // новый  tcpClient при подключении, ошибки становятся минимальными. Я так не понял в чем дело
                tcpClient = new TcpClient();

                try
                {
                    tcpClient.Connect(ipAddress, tcpPort);

                    if (tcpClient.Connected)
                    {
                        //успех
                        master = ModbusIpMaster.CreateIp(tcpClient);
                        master.Transport.Retries = 0; //don't have to do retries
                        master.Transport.ReadTimeout = 1500;
                        A1.Registers = master.ReadHoldingRegisters(1, 2801, 20);
                        A1.ConvertValues(); // конвертация значений
                        
                        break;
                    }
                }
                catch
                {
                    //неудача
                    string error = "Electricity: ID = " + post.id + " " + post.IP + " date - "
                        + DateTime.Now.ToString() + " Ошибка подключения TCP";
                    
                    Console.WriteLine(error);
                    if(post.id != 999)
                    {
                        // 999 это тестовый IP 
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
                    return; // если 3 итерации не помогло
                }
                tcpClient.Close();
                tcpClient.Dispose();

            }

            tcpClient.Close();
            tcpClient.Dispose();

            /* Хитрая система во избежания перегрузки БД. Создаем новую коллекцию в базе данных каждый месяц
             * первого числа. Иначе запрос к БД будет выполняться бесконечность.
            */
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            string date = new DateTime(year, month, 1).ToShortDateString();
            DateTime time = DateTime.Now;

            // передаем значения классу записи
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
                // попытка подключения к ДБ
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
                // запись в БД
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