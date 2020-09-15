using Modbus.Device;
using ModBus.Classes;
using MongoDB.Driver;
using Sharp7;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml;

namespace ModBus
{
    public class Gas
    {
        public void ReadXmlGas()
        {
            
            PathXml pathXml = new PathXml();
            string way = pathXml.GasRelativePathToXml();
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
                parametrs.AddressOfRemoteSlave = Convert.ToInt32(xnode.SelectSingleNode("AddressOfRemoteSlave").InnerText);
                parametrs.address = Convert.ToInt32(xnode.SelectSingleNode("address").InnerText);
                parametrs.length = Convert.ToInt32(xnode.SelectSingleNode("length").InnerText);
                parametrs.IP = xnode.SelectSingleNode("IP").InnerText;
                parametrs.port = Convert.ToInt32(xnode.SelectSingleNode("port").InnerText);
                parametrs.rack = Convert.ToInt32(xnode.SelectSingleNode("rack").InnerText);
                parametrs.slot = Convert.ToInt32(xnode.SelectSingleNode("slot").InnerText);

                Task.Factory.StartNew(() => SaveDocsGas(parametrs));
            }

            ProductionLineWrite productionLine = new ProductionLineWrite();
            Task.Factory.StartNew(() => productionLine.SaveDocsProductionLine(5, 6, 7, "Gas"));
        }

        private void SaveDocsGas(Gas_XmlDoc parametrs)
        {
            DateTime time = DateTime.Now;
            Gas_MongoNode gas_MongoNode = new Gas_MongoNode();

            
            if (parametrs.AddressOfRemoteSlave != 0)
            {
                ModbusIpMaster master = null;
                TcpClient tcpClient = null;

                
                
                PAC3200_Power A1 = new PAC3200_Power();

                // попытка подключения

                for (int i = 1; i <= 3; i++)
                {
                    // программа время от времени не читает несколько значений электричества, если создавать
                    // новый  tcpClient при подключении, ошибки становятся минимальными. Я так не понял в чем дело
                    tcpClient = new TcpClient();

                    try
                    {
                        tcpClient.Connect(parametrs.IP, parametrs.port);

                        if (tcpClient.Connected)
                        {
                            //успех
                            master = ModbusIpMaster.CreateIp(tcpClient);
                            master.Transport.Retries = 0; //don't have to do retries
                            master.Transport.ReadTimeout = 1500;
                            A1.Registers = master.ReadHoldingRegisters((byte)parametrs.AddressOfRemoteSlave,
                                (ushort)parametrs.address, (ushort)parametrs.length);
                            Console.WriteLine("попытка конвертации");
                            A1.intConvertValue(); // конвертация значений

                            gas_MongoNode.ID = parametrs.id;
                            gas_MongoNode.name = parametrs.name;
                            gas_MongoNode.value = A1.intValue;
                            gas_MongoNode.dateTime = time;

                            break;
                        }
                    }
                    catch
                    {
                        tcpClient.Close();
                        tcpClient.Dispose();

                        //неудача
                        string error = "GasTCP: ID = " + parametrs.id + " " + parametrs.IP + " date - "
                            + DateTime.Now.ToString() + " Ошибка подключения TCP" ;

                        Console.WriteLine(error);
                        if (parametrs.id != 999)
                        {
                            // 999 это тестовый IP
                            Log.logNodeElictricityTestID(error);
                            Log.logNodeElictricity(error);
                        }
                    }
                    if (i == 2)
                    {
                        return; // если 3 итерации не помогло
                    }
                    
                }

                tcpClient.Close();
                tcpClient.Dispose();
            }
            else
            {
                S7Client s7Client = new S7Client();
                float value = 0;
                byte[] Buffer = new byte[parametrs.length];

                for (int i = 0; i <= 2; i++)
                {
                    s7Client.ConnectTo(parametrs.IP, parametrs.rack, parametrs.slot);

                    if (s7Client.Connected)
                    {
                        s7Client.DBRead(parametrs.DB, parametrs.address, parametrs.length, Buffer);
                        value = S7.GetRealAt(Buffer, 0);

                        gas_MongoNode.ID = parametrs.id;
                        gas_MongoNode.name = parametrs.name;
                        gas_MongoNode.value = value;
                        gas_MongoNode.dateTime = time;

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
            }

            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            string date = new DateTime(year, month, 1).ToShortDateString();

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