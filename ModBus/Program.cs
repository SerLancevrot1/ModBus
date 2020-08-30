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

            // Проверка на единстенную версию приложения
            const string applicationName = "ModBus";
            bool createdApplicationNew;
            mutex = new Mutex(true, applicationName, out createdApplicationNew); 
            if (!createdApplicationNew)
            {
                Console.WriteLine(applicationName + " Приложение уже запущено! Выход.");
                //Console.ReadKey();
                return;
            }
            Console.WriteLine("Программа выполняется");

            Task.Factory.StartNew(() => DelayStart.Start());

            Console.Read();
        }
    }

   










}