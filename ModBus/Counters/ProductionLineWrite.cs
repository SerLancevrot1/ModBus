using ModBus.Classes;
using MongoDB.Driver;
using Sharp7;
using System;

namespace ModBus
{
    internal class ProductionLineWrite
    {
        // поток для записи в ДБ состояния работы производственных линий

        public void SaveDocsProductionLine(int ID1, int ID2, int ID3, string nameOfMongoDB)
        {
            S7Client s7Client = new S7Client();

            bool isRun1 = false, isRun2 = false, isRun3 = false;
            byte[] BufferVM_Run = new byte[1];

            for (int i = 0; i <= 2; i++)
            {
                s7Client.ConnectTo("29.67.7.222", 0, 2);

                if (s7Client.Connected)
                {
                    // побитово считываем значения
                    // в перегрузках (Номер дб,int start, int size, buffer)
                    s7Client.DBRead(25, 0, 1, BufferVM_Run);
                    //(buffer, int pos, int bit)не знаю что значит последнее
                    isRun1 = S7.GetBitAt(BufferVM_Run, 0, 0);
                    isRun2 = S7.GetBitAt(BufferVM_Run, 0, 1);
                    isRun3 = S7.GetBitAt(BufferVM_Run, 0, 2);

                    break;
                }
                else
                {
                    string error = "ProductionLine: " + "не удалось подключиться к адресу 29.67.7.222";
                    Console.WriteLine(error);
                    return;
                }
            }
            s7Client.Disconnect();

            DateTime time = DateTime.Now;

            // присваиваем значения

            ProductionLine productionLine1 = new ProductionLine();
            productionLine1.ID = ID1;
            productionLine1.dateTime = time;
            productionLine1.IsWork = isRun1;

            ProductionLine productionLine2 = new ProductionLine();
            productionLine2.ID = ID2;
            productionLine2.dateTime = time;
            productionLine2.IsWork = isRun2;

            ProductionLine productionLine3 = new ProductionLine();
            productionLine3.ID = ID3;
            productionLine3.dateTime = time;
            productionLine3.IsWork = isRun3;

            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            string date = new DateTime(year, month, 1).ToShortDateString();

            // подключение к ДБ
            IMongoCollection<ProductionLine> collection = null;
            try
            {
                MongoClient client = new MongoClient("mongodb://localhost");
                IMongoDatabase DB = client.GetDatabase(nameOfMongoDB);
                collection = DB.GetCollection<ProductionLine>(date);
            }
            catch (Exception e)
            {
                string error = "ProductionLine: " + "Не удалось подключиться к базе данных " + time + e.Message;
                Console.WriteLine(error);
                return;
            }
            // запись в дб
            try
            {
                collection.InsertOne(productionLine1);
                collection.InsertOne(productionLine2);
                collection.InsertOne(productionLine3);
            }
            catch (Exception e)
            {
                string error = "ProductionLine: " + "Ошибка записи в MongoDB:" + time + " " + e.Message;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(error);
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.WriteLine("ProductionLine: " + productionLine1.ID + " " + productionLine1.IsWork.ToString()
                + " " + "Запить произведена: " + time);
            Console.WriteLine("ProductionLine: " + productionLine2.ID + " " + productionLine2.IsWork.ToString()
                 + " " + "Запить произведена: " + time);
            Console.WriteLine("ProductionLine: " + productionLine3.ID + " " + productionLine3.IsWork.ToString()
                 + " " + "Запить произведена: " + time);
            return;
        }
    }
}