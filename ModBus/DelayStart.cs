using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModBus
{
    internal  class DelayStart
    {
        internal static void Start()
        {
            //Запуск потоков читающих счетчики каждую минуту в одно и то же время
            Console.WriteLine();

           
            Electricity electricity = new Electricity();
            Water waterPLC = new Water();
            Gas gas = new Gas();

            //секунды
            var DailyTime = "00";

            while (true)
            {
                DateTime dateNow = DateTime.Now;
                DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
                           dateNow.Hour, dateNow.Minute, int.Parse(DailyTime));
                TimeSpan ts;
                if (date > dateNow)
                    ts = date - dateNow;
                else
                {
                    date = date.AddMinutes(1);
                    ts = date - dateNow;
                }

                //линия для разграничения минут

                Console.WriteLine("---------------------------------" + dateNow.ToString() +
                   "---------------------------------");
                Task.Delay(ts).ContinueWith((x) => electricity.ReadXmlElectricity());
                Task.Delay(ts).ContinueWith((x) => waterPLC.ReadXmlWater());
                Task.Delay(ts).ContinueWith((x) => gas.ReadXmlGas());

                Thread.Sleep(60000); // 
                Log.logWriteElictricity();
                Log.logWriteElictricityTestID();
                Log.logWaterWrite();
            }
        }
    }
}