using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModBus
{
    internal  class DelayStart
    {
        internal  void Start()
        {
            Console.WriteLine();

            Electricity electricity = new Electricity();
            WaterPLC waterPLC = new WaterPLC();
            Gas gas = new Gas();

            //Time when method needs to be called
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

                //waits certan time and run the code

                Console.WriteLine("---------------------------------" + dateNow.ToString() +
                   "---------------------------------");
                Task.Delay(ts).ContinueWith((x) => electricity.ReadXmlElectricity());
                Task.Delay(ts).ContinueWith((x) => waterPLC.ReadXmlWaterPLC());
                Task.Delay(ts).ContinueWith((x) => gas.ReadXmlGas());

                Thread.Sleep(60000);
                Log.logWrite();
                Log.logWrite1();
                Log.logWaterWrite();
            }
        }
    }
}