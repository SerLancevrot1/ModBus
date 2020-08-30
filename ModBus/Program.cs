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

            const string applicationName = "ModBus";
            bool createdApplicationNew;

            
            mutex = new Mutex(true, applicationName, out createdApplicationNew); 

            // Проверка на единстенную версию приложения
            if (!createdApplicationNew)
            {
                Console.WriteLine(applicationName + " Приложение уже запущено! Выход.");
                //Console.ReadKey();
                return;
            }
            Console.WriteLine("Программа выполняется");


            //DelayStart delayStart = new DelayStart();
            Task.Factory.StartNew(() => DelayStart.Start());

            Console.Read();
        }
    }

   










}