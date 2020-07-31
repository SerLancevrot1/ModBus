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

   










}