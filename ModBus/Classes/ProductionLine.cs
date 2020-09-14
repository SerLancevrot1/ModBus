using MongoDB.Bson;
using System;

namespace ModBus.Classes
{
    internal class ProductionLine
    {
        public ObjectId _id { get; set; } // ID для БД, нужно для корректной работы
        public int ID { get; set; } // ID в самой БД
        public DateTime dateTime { get; set; } // время записи
        public bool IsWork { get; set; }
    }
}